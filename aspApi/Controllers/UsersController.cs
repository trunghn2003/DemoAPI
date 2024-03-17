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
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            if (id != userDTO.UserId)
            {
                return BadRequest();
            }

            var user = await _context.User.FindAsync(id);
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
                if (!UserExists(id))
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
        //[Authorize(Roles = "Admin")]
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
   /*     [Authorize(Roles = "Admin")]*/
        public async Task<IActionResult> DeleteUser(int id)
        {
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
           

            // Lấy danh sách các team mà người dùng thuộc về
           /* var teamUsers = _context.TeamUsers
                                .Include(tu => tu.Team)
                                .Where(tu => tu.UserId == user.UserId)
                                .ToList();

            if (teamUsers.Count == 0)
            {
                return Ok(new ApiReponse
                {
                    IsSuccess = true,
                    Message = "User is not associated with any team",
                }) ;
            }*/
            // Trong action xử lý đăng nhập


            // Tạo danh sách các vai trò dựa trên các team mà người dùng thuộc về
           /* var roles = teamUsers.Select(tu => tu.Role).Distinct().ToList();*/

            // Trả về danh sách các vai trò cho người dùng chọn
            return Ok(new ApiReponse
            {
                IsSuccess = true,
                Message = "",
                Data = Ok(user)
                
               /* Data = new { Roles = roles } */
            }); 
        }
        /*[HttpPost("login/with-role")]
        public IActionResult ValidateWithRole(int teamId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {

                return Ok(new ApiReponse
                {
                    IsSuccess = false,
                    Message = "User is not authenticated"
                });
            }
            // Tìm vai trò hiện tại của người dùng cho team đã chọn
            var teamUser = _context.TeamUsers
                                .FirstOrDefault(tu => tu.UserId == userId && tu.TeamId == teamId);

            if (teamUser == null)
            {
                return Ok(new ApiReponse
                {
                    IsSuccess = false,
                    Message = "User is not associated with the selected team"
                });
            }

            // Lấy vai trò của người dùng cho team đã chọn
            string userRole = teamUser.Role;

            // Tạo token với vai trò của người dùng cho team đã chọn
         
           *//* if (user == null)
            {
                return Ok(new ApiReponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }*//*
            var token = GenerateToken(_context.User.FirstOrDefault(t => t.UserId == userId), userRole);

            return Ok(new ApiReponse
            {
                IsSuccess = true,
                Message = "Authentication successful " + userRole + " " + " with TeamId: " + teamId  ,
                Data = new { Token = token }
            });
        }*/


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
