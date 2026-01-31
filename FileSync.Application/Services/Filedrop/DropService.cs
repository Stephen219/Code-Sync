using FileSync.Application.DTOS.FileDrop;
using FileSync.Application.Helpers;
using FileSync.Domain.entities;
using FileSync.Domain.enums;
using FileSync.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.Services.Filedrop
{


    public class DropService : IDropService
    {
        private readonly IFileDropRepository _dropRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IEmailService _emailService;
        
        // bycrypt.net
        //private const int BcryptWorkFactor = 12;



        public DropService(
            IFileDropRepository dropRepository,
            IFileStorageService fileStorageService,
            IEmailService emailService)
        {
            _dropRepository = dropRepository;
            _fileStorageService = fileStorageService;
            _emailService = emailService;
        }

        public async Task<CreateDropResponse> CreateDropAsync(CreateDropRequest request)
        {
            var drop = new Domain.entities.FileDrop
            {
                Id = Guid.NewGuid(),
                Code = CodeGenerator.GenerateDropCode(),

                // recipients email is a list
                RecipientEmails = request.RecipientEmails,

                PasscodeHash = string.IsNullOrEmpty(request.Passcode)
                    ? null
                    : BCrypt.Net.BCrypt.HashPassword(request.Passcode),
                Message = request.Message,
                Status = DropStatus.Active,
                DownloadCount = 0,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(request.ExpiresInDays),
                Files = new List<DropFile>()
            };

            await _dropRepository.AddAsync(drop);

            return new CreateDropResponse
            {
                DropId = drop.Id,
                Code = drop.Code,
                ExpiresAt = drop.ExpiresAt
            };
        }

        public async Task<DropFileDto?> AddFileAsync(Guid dropId, Stream fileStream, string fileName, string contentType)
        {
            var drop = await _dropRepository.GetByIdAsync(dropId);

            if (drop == null || drop.Status != DropStatus.Active)
                return null;

            // Get file size BEFORE reading the stream
            long fileSize = fileStream.Length;

            // Save file to storage
            var folder = $"drops/{drop.Id}";
            var storagePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, folder);

            // Create file record
            var dropFile = new DropFile
            {
                Id = Guid.NewGuid(),
                DropId = drop.Id,
                FileName = fileName,
                FileSize = fileSize,  // Use saved size
                ContentType = contentType,
                StoragePath = storagePath,
                UploadedAt = DateTime.UtcNow
            };

            // Add file directly (not through collection)
            await _dropRepository.AddFileAsync(dropFile);

            return new DropFileDto
            {
                Id = dropFile.Id,
                FileName = dropFile.FileName,
                FileSize = dropFile.FileSize,
                ContentType = dropFile.ContentType
            };
        }
        public async Task<bool> FinalizeDropAsync(Guid dropId)
        {
            var drop = await _dropRepository.GetByIdAsync(dropId);

            if (drop == null || drop.Status != DropStatus.Active)
                return false;

            // Send email if recipients provided
            if (drop.RecipientEmails != null && drop.RecipientEmails.Any())
            {
                //await _emailService.SendDropNotificationAsync(
                //    drop.RecipientEmails,
                //    drop.Code,
                //    drop.Message);send email method accepts list of emails
                await _emailService.SendDropNotificationAsync(
                    drop.RecipientEmails,
                    drop.Code,
                    drop.Message);

                // the emails are a list
                // send email method accepts list of emails



            }

            return true;
        }

        public async Task<DropDto?> GetDropByCodeAsync(string code)
        {
            var drop = await _dropRepository.GetByCodeAsync(code);

            if (drop == null || drop.Status != DropStatus.Active)
                return null;

            // Check if expired
            if (drop.ExpiresAt < DateTime.UtcNow)
            {
                drop.Status = DropStatus.Expired;
                await _dropRepository.UpdateAsync(drop);
                return null;
            }

            return MapToDto(drop);
        }

        public async Task<bool> VerifyPasscodeAsync(string code, string passcode)
        {
            var drop = await _dropRepository.GetByCodeAsync(code);

            if (drop == null || drop.Status != DropStatus.Active)
                return false;

            // No passcode required
            if (string.IsNullOrEmpty(drop.PasscodeHash))
                return true;

            // Verify passcode
            return BCrypt.Net.BCrypt.Verify(passcode, drop.PasscodeHash);
        }

        public async Task<Stream?> GetFileAsync(string code, Guid fileId)
        {
            var drop = await _dropRepository.GetByCodeAsync(code);

            if (drop == null || drop.Status != DropStatus.Active)
                return null;

            var file = drop.Files.FirstOrDefault(f => f.Id == fileId);

            if (file == null)
                return null;

            // Increment download count
            drop.DownloadCount++;
            await _dropRepository.UpdateAsync(drop);

            return await _fileStorageService.GetFileAsync(file.StoragePath);
        }

        private static DropDto MapToDto(Domain.entities.FileDrop drop)
        {
            return new DropDto
            {
                Code = drop.Code,
                Message = drop.Message,
                HasPasscode = !string.IsNullOrEmpty(drop.PasscodeHash),
                ExpiresAt = drop.ExpiresAt,
                DownloadCount = drop.DownloadCount,
                Files = drop.Files.Select(f => new DropFileDto
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    ContentType = f.ContentType
                }).ToList()
            };
        }
    }
}
