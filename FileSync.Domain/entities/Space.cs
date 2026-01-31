using FileSync.Domain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync.Domain.entities
{
   public class Space
    {

        public Guid Id { get; set; }
      public string Code { get; set; }
      public string Content { get; set; }
    public SpaceStatus Status { get; set; }                               
    public DateTime CreatedAt { get; set; }                               
    public DateTime ExpiresAt { get; set; }                              
                                                                             
    public List<SpaceFile> Files { get; set; }

    }
}
