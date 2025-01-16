using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SultanSklep.Business.Utilites
{
    public static class FileExtensions
    {
        public static async Task<string> SaveFileAsync(this IFormFile file, string rootPath, string folder, string subfolder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Fayl mövcud deyil və ya boşdur.");
            }

            string directoryPath = Path.Combine(rootPath, folder, subfolder);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(directoryPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(folder, subfolder, uniqueFileName).Replace("\\", "/");
        }

        public static bool CheckFileType(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }

        public static bool CheckFileSize(this IFormFile file, int size)
        {
            return file.Length / 1024 <= size;
        }
    }
}
