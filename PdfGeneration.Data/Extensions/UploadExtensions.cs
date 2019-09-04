using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PdfGeneration.Core.Extensions;
using PdfGeneration.Data.Entities;

namespace PdfGeneration.Data.Extensions
{
    public static class UploadExtensions
    {
        private static IQueryable<Upload> SetUploadIncludes(this DbSet<Upload> uploads) =>
            uploads.Include(x => x.User);

        public static async Task<List<Upload>> GetUploads(this AppDbContext db, bool isDeleted = false)
        {
            var uploads = await db.Uploads
                .SetUploadIncludes()
                .Where(x => x.IsDeleted == isDeleted)
                .OrderByDescending(x => x.UploadDate)
                .ToListAsync();

            return uploads;
        }

        public static async Task<List<Upload>> SearchUploads(this AppDbContext db, string search, bool isDeleted = false)
        {
            search = search.ToLower();
            var uploads = await db.Uploads
                .SetUploadIncludes()
                .Where(x => x.IsDeleted == isDeleted)
                .Where(x => x.File.ToLower().Contains(search))
                .OrderByDescending(x => x.UploadDate)
                .ToListAsync();

            return uploads;
        }

        public static async Task<Upload> GetUpload(this AppDbContext db, int uploadId) => 
            await db.Uploads
                .SetUploadIncludes()
                .FirstOrDefaultAsync(x => x.Id == uploadId);

        public static async Task<Upload> GetUploadByName(this AppDbContext db, string file) => 
            await db.Uploads
                .SetUploadIncludes()
                .FirstOrDefaultAsync(x => x.File.ToLower() == file.ToLower());

        public static async Task<List<Upload>> UploadFiles(this AppDbContext db, IFormFileCollection files, string path, string url, int userId)
        {
            if (files.Count < 1)
            {
                throw new Exception("No files provided for upload");
            }

            List<Upload> uploads = new List<Upload>();

            foreach (var file in files)
            {
                uploads.Add(await db.AddUpload(file, path, url, userId));
            }

            return uploads;
        }

        public static async Task ToggleUploadDeleted(this AppDbContext db, Upload upload)
        {
            db.Uploads.Attach(upload);
            upload.IsDeleted = !upload.IsDeleted;
            await db.SaveChangesAsync();
        }

        public static async Task RemoveUpload(this AppDbContext db, Upload upload)
        {
            await upload.DeleteFile();
            db.Uploads.Remove(upload);
            await db.SaveChangesAsync();
        }

        static async Task<Upload> AddUpload(this AppDbContext db, IFormFile file, string path, string url, int userId)
        {
            var upload = await file.WriteFile(path, url, userId);
            upload.UploadDate = DateTime.Now;
            await db.Uploads.AddAsync(upload);
            await db.SaveChangesAsync();
            return upload;
        }

        static async Task<Upload> WriteFile(this IFormFile file, string path, string url, int userId)
        {
            if (!(Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
            }

            var upload = await file.CreateUpload(path, url, userId);

            using (var stream = new FileStream(upload.Path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return upload;
        }

        static Task<Upload> CreateUpload(this IFormFile file, string path, string url, int userId) => Task.Run(() =>
        {
            var f = file.CreateSafeName(path);

            var upload = new Upload
            {
                UserId = userId,
                File = f,
                Name = file.Name,
                Path = $"{path}{f}",
                Url = $"{url}{f}",
                FileType = file.ContentType,
                Size = file.Length,
               // UploadDate = DateTime.Now,
               // IsDeleted = false
            };

            return upload;
        });

        static string CreateSafeName(this IFormFile file, string path)
        {
            var increment = 0;
            var fileName = file.FileName.UrlEncode();
            var newName = fileName;

            while (File.Exists(path + newName))
            {
                var extension = fileName.Split('.').Last();
                newName = $"{fileName.Replace($".{extension}", "")}_{++increment}.{extension}";
            }

            return newName;
        }

        static Task DeleteFile(this Upload upload) => Task.Run(() =>
        {
            try
            {
                if (File.Exists(upload.Path))
                {
                    File.Delete(upload.Path);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.GetExceptionChain());
            }
        });
    }
}