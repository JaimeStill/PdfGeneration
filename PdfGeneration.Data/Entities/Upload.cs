using System;
using System.Collections.Generic;

namespace PdfGeneration.Data.Entities
{
    public class Upload
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public string File { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public long Size { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsDeleted { get; set; }

        public User User { get; set; }
    }
}