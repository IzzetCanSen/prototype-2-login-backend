using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prototype_2_login_backend.Context;
using prototype_2_login_backend.Models;

namespace prototype_2_login_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInUser([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(x => x.Email == userObj.Email && x.Password == userObj.Password);
            if (user == null)
                return NotFound(new { Message = "User not found." });

            return Ok(new {
                Message = "Login sucess!"
            });
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpUser([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            userObj.Role = "User";
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new { Message = "User signed up!" });
        }
    }
}
