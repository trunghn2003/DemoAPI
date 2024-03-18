using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspApi.Data;
using aspApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using aspApi.DTO;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace aspApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
    private readonly IHttpContextAccessor contextAccesser;
        private readonly MyDbContext _context;

        public UsersController(MyDbContext context, IHttpContextAccessor contextAccesser)
        {
            _context = context;
            this.contextAccesser = contextAccesser; 
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        /*[HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]

        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
*/
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        /*[Authorize(Roles = "Admin")]*/
        public async Task<IActionResult> PutUser(UserDTO userDTO)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            if(userId.Value != userDTO.UserId) {
                return BadRequest("u dont have permission");
            }
            

            var user = await _context.User.FindAsync(userDTO.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties with values from userDTO
            user.Name = userDTO.Name;
            user.UserName = userDTO.UserName;
            user.Password = userDTO.Password;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userDTO.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Put successfully");
        }

        
    

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
       
        public async Task<ActionResult<User>> PostUser(UserDTO createUserDTO)
        {


            var user = new User
            {
                Name = createUserDTO.Name,
                UserName = createUserDTO.UserName,
                Password = createUserDTO.Password
             
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            //// Thêm TeamUser cho user
            //if (createUserDTO.TeamIds != null && createUserDTO.TeamIds.Any())
            //{
            //    foreach (var teamId in createUserDTO.TeamIds)
            //    {
            //        var teamUser = new TeamUser
            //        {
            //            UserId = user.UserId,
            //            TeamId = teamId,
            //            Role = "User" 
            //        };
            //        _context.TeamUsers.Add(teamUser);
            //    }
            //    await _context.SaveChangesAsync();
            //}

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
 
        public async Task<IActionResult> DeleteUser(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            if (userId.Value != id)
            {
                return BadRequest("u dont have permission");
            }


            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Delete success");
        }
        /* [HttpPost("login")]
         public IActionResult Validate(LoginModel model)
         {
             var user = _context.User.SingleOrDefault(p => p.UserName == model.Username && model.Password == p.Password);

             if (user == null)
             {
                 return Ok(new ApiReponse
                 {
                     IsSuccess = false,
                     Message = "Invalid username or password"
                 });
             }

             var teamUser = _context.TeamUsers
                                 .Include(tu => tu.Team)
                                 .FirstOrDefault(tu => tu.UserId == user.UserId);
             var teamUserCount = _context.TeamUsers.Count(tu => tu.UserId == user.UserId);
             Console.WriteLine(teamUserCount);
             if (teamUser == null)
             {
                 // Handle the case where the user is not associated with any team
                 // You might want to return an error response or handle it according to your business logic.
             }

             return Ok(new ApiReponse
             {
                 IsSuccess = true,
                 Message = "Authen success" + teamUser.Role,
                 //Data = teamUser.Role
                 Data = GenerateToken(user, teamUser.Role)
             }) ;  
         }*/
        [HttpPost("login")]
        public IActionResult Validate(LoginModel model)
        {
            var user = _context.User.SingleOrDefault(p => p.UserName == model.Username && model.Password == p.Password);

            if (user == null)
            {
                return Ok(new ApiReponse
                {
                    IsSuccess = false,
                    Message = "Invalid username or password"
                });
            }
            HttpContext.Session.SetInt32("UserId", user.UserId);
           

            return Ok(new ApiReponse
            {
                IsSuccess = true,
                Message = "Login success",
                Data = Ok(user)
                
             
            }); 
        }
        private string GenerateToken(User user, string userRole)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes("FreeCourseDemoASPNETCoreWebAPI22");

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim("UserName", user.UserName),
            new Claim("Id", user.UserId.ToString()),
            new Claim(ClaimTypes.Role, userRole),
            new Claim("TokenId", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())

        }),
                Expires = DateTime.UtcNow.AddMinutes(100),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
