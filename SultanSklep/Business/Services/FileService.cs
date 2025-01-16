using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SultanSklep.Business.Services
{
    public static class FileService
    {
        public static async Task<string> SaveFileAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Fayl boşdur.");
            }

            // Yolu yaradın
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);

            // Əgər qovluq mövcud deyilsə, yaradın
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Faylın adı
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Tam fayl yolu
            string filePath = Path.Combine(uploadPath, uniqueFileName);

            // Faylı yazın
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Fayl yolunu qaytarın (relativ yol)
            return Path.Combine(folderPath, uniqueFileName).Replace("\\", "/");
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
