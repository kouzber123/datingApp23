using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        //this is how we access to strongly typed class
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            //set our api accouint > address
            var acc = new Account
             (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            //declare this is our cloudinary account
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                //access to file stream
                using var stream = file.OpenReadStream();
                var uploadParams = new  ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    //change dimension before upload
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "da-net7"
                };
                //upload to clouidinary
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
