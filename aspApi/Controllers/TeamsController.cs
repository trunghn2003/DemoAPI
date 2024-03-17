using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aspApi.Data;
using aspApi.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using aspApi.DTO;
using System.Security.Claims;
using System.Numerics;

namespace aspApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TeamsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Teams
        [HttpGet]
        [Authorize(Roles = "Admin, User")]

        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _context.Teams.ToListAsync();
        }

        // GET: api/Teams/5
       /* [HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }*/

        // PUT: api/Teams/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutTeam(int id, TeamDTO teamDTO)
        {
            if (id != teamDTO.TeamId)
            {
                return BadRequest("Id mismatch between URL and request body.");
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            // Update team properties with values from teamDTO
            team.Name = teamDTO.Name;
            team.IsPublic = teamDTO.IsPublic;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(team);
        }

        // POST: api/Teams
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //[Authorize(Roles = "Admin")]

        //public async Task<ActionResult<Team>> PostTeam(TeamDTO teamDTO)
        //{
        //    var team= new Team
        //    {
        //        Name = teamDTO.Name
        //    };
        //    _context.Teams.Add(team);
        //    await _context.SaveChangesAsync();  
        //    return CreatedAtAction(nameof(Team), new {id = team.TeamId}, team);
        ////    _context.Teams.Add(team);
        ////    await _context.SaveChangesAsync();

        ////    return CreatedAtAction("GetTeam", new { id = team.TeamId }, team);
        //}
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult<Team>> PostTeam(TeamDTO teamDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map TeamDTO to Team entity
            var team = new Team
            {
                Name = teamDTO.Name,
                IsPublic = teamDTO.IsPublic

            };

            // Add and save the new Team entity
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            // Return the created Team
            return CreatedAtAction(nameof(GetTeams), new { id = team.TeamId }, team);

        }




        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound("not found TeamId");
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return Ok($"Da xoa thanh cong teamId: {id}");
        }
        public class UserInTeamDto
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Role { get; set; }
            
        }


         
        //get user from teamId
        [HttpGet("{id}/getUser")]
        public ActionResult<IEnumerable<UserInTeamDto>> GetUsersForTeamId(int id)
        {
            var currentUser = HttpContext.User;
            var team = _context.Teams
                                .Include(t => t.TeamUsers)
                                    .ThenInclude(tu => tu.User)
                                .FirstOrDefault(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound("Team not found");
            }


            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not found");  
            }
            if (!team.IsPublic)
            {
                if (!IsUserAuthorizedForTeam(currentUser, team))
                {
                    return Forbid("This is private, you dont belong to it"); // Hoặc NotFound() tùy thuộc vào logic ứng dụng của bạn
                }

            }
            var userInTeam = team.TeamUsers.Select(tu => new UserInTeamDto
            {
                UserId = tu.User.UserId,
                UserName = tu.User.UserName,
                Role = tu.Role
            }).ToList();


            return Ok(userInTeam);

        }
        [HttpPut("{id}/privacy")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTeamPrivacy(int id, bool isPublic)
        {
            var team = await _context.Teams
                .Include(t => t.TeamUsers)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            var userId = GetUserIdFromClaims(HttpContext.User);
            //var userId = GetUserIdFromClaims(HttpContext.User);
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == id).ToListAsync();

            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {
                
                foreach (var teamUser in teamUsers)
                {
                    if(teamUser.UserId == userId && teamUser.TeamId ==id) {
                        if(teamUser.Role != "Admin")
                        {
                            //Console.WriteLine("ko dc");
                            return BadRequest("You don't have Permission");
                        }
                    }
                }
            }


            //Console.WriteLine(userId);

            if(IsUserAuthorizedForTeam(HttpContext.User, team) == false)
            {
                return BadRequest("it doesnt belong to this team");
            }

            

            team.IsPublic = isPublic;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Team privacy updated successfully" });
        }


        private bool IsUserAuthorizedForTeam(ClaimsPrincipal user, Team team)
        {
           
            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var teamId = team.TeamId;
            Console.WriteLine(userId + " " + teamId);
            var teamUser = team.TeamUsers;
            foreach (var i in teamUser)
            {
                if (i.UserId == userId && i.TeamId == teamId)
                {
                    Console.WriteLine(teamId + " " + userId +" " + i.Role);   
                    return true;
                }
             
            }
            return false;
        }

        private int GetUserIdFromClaims(ClaimsPrincipal user)
        {
            // Lấy UserId từ Claims của người dùng, bạn cần điều chỉnh logic này tùy thuộc vào cách bạn thực hiện xác thực
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return -1; // hoặc ném ra một exception, tùy thuộc vào yêu cầu của ứng dụng
        }




        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.TeamId == id);
        }
    }
}
