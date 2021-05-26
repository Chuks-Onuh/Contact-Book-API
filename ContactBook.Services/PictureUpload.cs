using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;

namespace ContactBook.Core
{
    public class PictureUpload
    {
        private readonly Cloudinary _cloudinary;
        public PictureUpload(IConfiguration config)
        {
            Account account = new Account
            {
                Cloud = config.GetSection("CloudinarySettings: CloudName").Value,
                ApiKey = config.GetSection("CloudinarySettings: Apikey").Value,
                ApiSecret = config.GetSection("CloudinarySettings: ApiSecret").Value,
            };
            _cloudinary = new Cloudinary(account);
        }
        public Cloudinary GetPicture()
        {
            return _cloudinary;
        }

    }

}
