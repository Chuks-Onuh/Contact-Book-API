using ContactBook.Auth;
using ContactBook.DTOs;
using ContactBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ContactBook.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class AuthController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        public AuthController(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginDTO model)
        {
            var user =  await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                return BadRequest("User not found");
            }
            var Password = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!Password)
            {
                return BadRequest("Invalid credentials");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var UserRoles = roles.ToArray();

            var token = TokenGenerator.GenerateToken(model.Email, user.Id, model.Password, _config, UserRoles);

            return Ok(token);
        }
    }
}