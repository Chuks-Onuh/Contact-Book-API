using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Linq;
using ContactBook.Data;
using ContactBook.DTOs;
using ContactBook.Models;
using ContactBook.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ContactBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class AppUserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly ContactBookContext _ctx;
        private readonly Cloudinary _cloudinary;

        public AppUserController(UserManager<AppUser> userManager, ContactBookContext context, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
            _ctx = context;
            Account account = new Account
            {
                Cloud = _config.GetSection("CloudinarySettings:CloudName").Value,
                ApiKey = _config.GetSection("CloudinarySettings:Apikey").Value,
                ApiSecret = _config.GetSection("CloudinarySettings:ApiSecret").Value,
            };
            _cloudinary = new Cloudinary(account);
        }
        // GET: api/<AppUserController>
        [HttpGet("all-users")]
        [Authorize(Roles = "Admin")]
        public IEnumerable<AppUserDTO> GetAll([FromQuery] PaginParameter usersParameter)
        {
            var utility = new Utilities(_ctx);
            var data = new List<AppUserDTO>();
            foreach (var userdata in utility.GetAllUsers(usersParameter))
            {
                data.Add(new AppUserDTO
                {
                    FirstName = userdata.FirstName,
                    LastName = userdata.LastName,
                    Email = userdata.Email,
                    PhoneNumber = userdata.PhoneNumber,
                    ImageUrl = userdata.ImageUrl,
                    FacebookUrl = userdata.FacebookUrl,
                    TwitterUrl = userdata.TwitterUrl,
                    City = userdata.City,
                    State = userdata.State,
                    Country = userdata.Country
                });
            }
            return data;
          
        }

        // GET api/<AppUserController>/5
        [HttpGet("get-user/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(string Id)
        {

            var result = await _ctx.Users.FindAsync(Id);
            if (result == null)
            {
                return NotFound();
            }
            var data = new AppUserDTO
            {
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                PhoneNumber = result.PhoneNumber,
                ImageUrl = result.ImageUrl,
                FacebookUrl = result.FacebookUrl,
                TwitterUrl = result.TwitterUrl,
                City = result.City,
                State = result.State,
                Country = result.Country
            };
            return Ok(data);
        }
        // GET api/<AppUserController>/email
        [HttpGet]
        [Route("email")]
        [Authorize(Roles = "Admin, Regular")]
        public async Task<IActionResult> GetByEmail([FromQuery]string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
            return BadRequest();
            }
            var data = new AppUserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                FacebookUrl = user.FacebookUrl,
                TwitterUrl = user.TwitterUrl,
                City = user.City,
                State = user.State,
                Country = user.Country
            };
            return Ok(data);
        }

        //POST api/<AppUserController>
        [HttpPost]
        [Route("add-new")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] AppUserDTO appUser)
        {
            var user = await _userManager.FindByEmailAsync(appUser.Email);
            if (user == null)
            {
                var data = new AppUser()
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    UserName = appUser.Email,
                    Email = appUser.Email,
                    ImageUrl = appUser.ImageUrl,
                    PhoneNumber = appUser.PhoneNumber,
                    FacebookUrl = appUser.FacebookUrl,
                    TwitterUrl = appUser.TwitterUrl,
                    City = appUser.City,
                    State = appUser.State,
                    Country = appUser.Country
                };
                var res = await _userManager.CreateAsync(data, appUser.Password);
                if (res.Succeeded)
                {
                    await _userManager.AddToRoleAsync(data, "Regular");
                    return Ok("User created successfully");
                }
            }
            return BadRequest("User already exist");
        }

        // PUT api/<AppUserController>/5
        [HttpPut]
        [Route("update/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(String id, [FromBody] AppUserDTO appUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var pas = _userManager.PasswordHasher.HashPassword(user, appUser.Password);
                user.FirstName = appUser.FirstName;
                user.LastName = appUser.LastName;
                user.UserName = appUser.Email;
                user.Email = appUser.Email;
                user.ImageUrl = appUser.ImageUrl;
                user.PasswordHash = pas;
                user.PhoneNumber = appUser.PhoneNumber;
                user.FacebookUrl = appUser.FacebookUrl;
                user.TwitterUrl = appUser.TwitterUrl;
                user.City = appUser.City;
                user.State = appUser.State;
                user.Country = appUser.Country;
                var res = await _userManager.UpdateAsync(user);
                return Ok(res);
            }
            return BadRequest("User not found");

        }

        //DELETE api/<AppUserController>/email
        [HttpDelete("delete/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            await _userManager.DeleteAsync(user);
            _ctx.SaveChanges();
            return Ok("User successfully removed");
        }
        //[HttpDelete("{search}")]
        //public async Task<ActionResult<IEnumerable<AppUser>>> Search(string fname, string? lname)
        //{

        //}
        [HttpPatch("photo/{Id}")]
        [Authorize(Roles = "Admin, Regular")]
        public async Task<IActionResult> AddUserPhotoAsync(string Id, [FromForm] PhotoToAddDTO model)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (Id != user.Id)
                return Unauthorized();
            var file = model.PhotoFile;
            if (file.Length <= 0)
                return BadRequest("Invalid file size");

            var imageUploadeResult = new ImageUploadResult();
            using (var fs = file.OpenReadStream())
            {

                var imageUploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.Name, fs),
                    Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")

                };
                imageUploadeResult = _cloudinary.Upload(imageUploadParams);
            }
            var publicId = imageUploadeResult.PublicId;
            var Url = imageUploadeResult.Url;
            user.ImageUrl = Url.AbsolutePath;
            await _userManager.UpdateAsync(user);
            return Ok(new { Id = publicId, Url });
        }
        [HttpGet("search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
                return BadRequest("Search term cannot be Empty");
            var users = await _userManager.Users
                                      .Where(u => u.Email.Contains(term)
                                      || u.FirstName.Contains(term)
                                      || u.LastName.Contains(term)
                                      || u.City.Contains(term)
                                      || u.State.Contains(term)
                                      || u.Country.Contains(term)
                                      ).ToListAsync();
            if (users == null)
            {
                return NotFound("Search Result Empty");
            }
            List<AppUserDTO> appUserDTOs = new List<AppUserDTO>();
            foreach (var item in users)
            {
                var data = new AppUserDTO()
                {
                   FirstName = item.FirstName,
                   LastName = item.LastName,
                   Username = item.UserName,
                   Email = item.Email,
                   ImageUrl = item.ImageUrl,
                   PhoneNumber = item.PhoneNumber,
                   FacebookUrl = item.FacebookUrl,
                   TwitterUrl = item.TwitterUrl,
                   City = item.City,
                   State = item.State,
                   Country = item.Country
                };
                appUserDTOs.Add(data);
            }
            return Ok(appUserDTOs);
        }

    }
}