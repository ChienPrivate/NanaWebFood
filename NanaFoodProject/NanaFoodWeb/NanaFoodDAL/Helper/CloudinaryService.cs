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
    }
}
