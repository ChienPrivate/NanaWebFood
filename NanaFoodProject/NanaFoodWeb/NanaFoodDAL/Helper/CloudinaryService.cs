using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Forms;

namespace NanaFoodDAL.Helper
{
    public class CloudinaryService
    {
        // kích thước tối đa hình ảnh 10 * 1024 * 1024 = 10 mb
        private readonly Cloudinary _cloudinary;
        public CloudinaryService()
        {
            var account = new Account(
                "dlfvbe9bi",
                "132196749226247",
                "QVy6ptdQ34UnKtjpx1weqvmOlj4"
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var imageStream  = file.OpenReadStream();
            string fileName = file.Name;

            var UploadParam = new ImageUploadParams
            {
                File = new FileDescription(fileName, imageStream),
                Overwrite = true
            };
            var result = await _cloudinary.UploadAsync(UploadParam);

            return result?.SecureUri.AbsoluteUri;
        }

        public async Task<bool> DeleteImage(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                var segments = uri.Segments;

                string publicIdWithExtension = segments[^1];
                string publicId = Path.GetFileNameWithoutExtension(publicIdWithExtension);

                Console.WriteLine($"Attempting to delete image with PublicId: {publicId}");

                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result?.Result == "not found")
                {
                    Console.WriteLine($"Image with PublicId '{publicId}' was not found.");
                    return false; // Hoặc xử lý tùy theo nhu cầu
                }

                return result?.Result == "ok";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");
                return false;
            }
        }
    }
}
