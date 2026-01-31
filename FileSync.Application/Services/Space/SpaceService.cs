using FileSync.Application.DTOS.Space;
using FileSync.Application.Helpers;
using FileSync.Domain.entities;
using FileSync.Domain.enums;
using FileSync.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Application.Services.Space
{

    public class SpaceService : ISpaceService
    {
        private readonly ISpaceRepository _spaceRepository;
        private readonly IFileStorageService _fileStorageService;

        public SpaceService(ISpaceRepository spaceRepository, IFileStorageService fileStorageService)
        {
            _spaceRepository = spaceRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<CreateSpaceResponse> CreateSpaceAsync()
        {
            var space = new Domain.entities.Space
            {
                Id = Guid.NewGuid(),
                Code = CodeGenerator.GenerateSpaceCode(),
                Content = string.Empty,
                Status = SpaceStatus.Active,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(12),
                Files = new List<SpaceFile>()
            };

            await _spaceRepository.AddAsync(space);

            return new CreateSpaceResponse
            {
                Code = space.Code,
                ExpiresAt = space.ExpiresAt
            };
        }

        public async Task<SpaceDto?> GetSpaceByCodeAsync(string code)
        {
            var space = await _spaceRepository.GetByCodeAsync(code);

            if (space == null || space.Status != SpaceStatus.Active)
                return null;

            // Check if expired
            if (space.ExpiresAt < DateTime.UtcNow)
            {
                space.Status = SpaceStatus.Expired;
                await _spaceRepository.UpdateAsync(space);
                return null;
            }

            return MapToDto(space);
        }

        public async Task<bool> UpdateContentAsync(string code, string content)
        {
            var space = await _spaceRepository.GetByCodeAsync(code);

            if (space == null || space.Status != SpaceStatus.Active)
                return false;

            space.Content = content;
            await _spaceRepository.UpdateAsync(space);

            return true;
        }

        //public async Task<SpaceFileDto?> AddFileAsync(string code, Stream fileStream, string fileName, string contentType)
        //{
        //    var space = await _spaceRepository.GetByCodeAsync(code);

        //    if (space == null || space.Status != SpaceStatus.Active)
        //        return null;

        //    // Save file to storage
        //    var folder = $"spaces/{space.Id}";
        //    var storagePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, folder);

        //    // Create file record
        //    var spaceFile = new SpaceFile
        //    {
        //        Id = Guid.NewGuid(),
        //        SpaceId = space.Id,
        //        FileName = fileName,
        //        FileSize = fileStream.Length,
        //        FileType = contentType,
        //        StoragePath = storagePath,
        //        UploadedAt = DateTime.UtcNow

        //    };

        //    space.Files.Add(spaceFile);
        //    await _spaceRepository.UpdateAsync(space);

        //    return new SpaceFileDto
        //    {
        //        Id = spaceFile.Id,
        //        FileName = spaceFile.FileName,
        //        FileSize = spaceFile.FileSize,
        //        FileType = spaceFile.FileType
        //    };
        //}







        public async Task<SpaceFileDto?> AddFileAsync(string code, Stream fileStream, string fileName, string contentType)
        {
            var space = await _spaceRepository.GetByCodeAsync(code);

            if (space == null || space.Status != SpaceStatus.Active)
                return null;

            // Get file size BEFORE reading the stream
            long fileSize = fileStream.Length;

            // Save file to storage
            var folder = $"spaces/{space.Id}";
            var storagePath = await _fileStorageService.SaveFileAsync(fileStream, fileName, folder);

            // Create file record
            var spaceFile = new SpaceFile
            {
                Id = Guid.NewGuid(),
                SpaceId = space.Id,
                FileName = fileName,
                FileSize = fileSize,  // Use saved size
                FileType = contentType,
                StoragePath = storagePath,
                UploadedAt = DateTime.UtcNow
            };

            // Add file directly (not through collection)
            await _spaceRepository.AddFileAsync(spaceFile);

            return new SpaceFileDto
            {
                Id = spaceFile.Id,
                FileName = spaceFile.FileName,
                FileSize = spaceFile.FileSize,
                FileType = spaceFile.FileType
            };
        }

        public async Task<Stream?> GetFileAsync(string code, Guid fileId)
        {
            var space = await _spaceRepository.GetByCodeAsync(code);

            if (space == null || space.Status != SpaceStatus.Active)
                return null;

            var file = space.Files.FirstOrDefault(f => f.Id == fileId);

            if (file == null)
                return null;

            return await _fileStorageService.GetFileAsync(file.StoragePath);
        }






        public async Task<bool> DeleteSpaceAsync(string code)
        {
            var space = await _spaceRepository.GetByCodeAsync(code);

            if (space == null)
                return false;

            // Delete files from storage
            var folder = $"spaces/{space.Id}";
            await _fileStorageService.DeleteFolderAsync(folder);

            // Delete from database
            await _spaceRepository.DeleteAsync(space.Id);

            return true;
        }

        private static SpaceDto MapToDto(Domain.entities.Space space)
        {
            return new SpaceDto
            {
                Code = space.Code,
                Content = space.Content,
                ExpiresAt = space.ExpiresAt,
                Files = space.Files.Select(f => new SpaceFileDto
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    FileType = f.FileType
                }).ToList()
            };
        }
    }
}
