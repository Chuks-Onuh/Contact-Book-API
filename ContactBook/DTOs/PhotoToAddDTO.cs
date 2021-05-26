using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.DTOs
{
    public class PhotoToAddDTO
    {
        public IFormFile PhotoFile { get; set; }
    }
}
