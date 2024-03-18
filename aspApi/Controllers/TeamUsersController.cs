using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspApi.Models;
using aspApi.Data;
using static aspApi.Controllers.TeamUsersController;
using aspApi.DTO;
using Microsoft.AspNetCore.Authorization;

namespace aspApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamUsersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TeamUsersController(MyDbContext context)
        {
            _context = context;
        }



        [HttpPost]
        public async Task<ActionResult<TeamUser>> PostTeamUser(int TeamId, TeamUserDTO teamUserDto)
        {
            var team = await _context.Teams
                .Include(t => t.TeamUsers)
                .FirstOrDefaultAsync(t => t.TeamId == TeamId);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == TeamId).ToListAsync();
            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {

                foreach (var teamUser in teamUsers)
                {
                    if (teamUser.UserId == userId && teamUser.TeamId == TeamId)
                    {
                        if (teamUser.Role == "Admin")
                        {
                            var newTeamUser = new TeamUser
                            {
                                TeamId = TeamId,
                                UserId = teamUserDto.UserId,
                                Role = teamUserDto.Role,

                            };
                            _context.TeamUsers.Add(newTeamUser);
                            await _context.SaveChangesAsync();

                            return Ok(newTeamUser);
                        }
                        else
                        {
                            return BadRequest("You arent Admin");
                        }
                    }
                }
            }
            return BadRequest("Yon dont belong to this team");

        }
        /*[HttpPut("{teamId}/{userId}")]
        public async Task<ActionResult<TeamUser>> PutTeamUser(int teamId, int userId, TeamUserDTO teamUserDto)
        {
            var team = await _context.Teams
                 .Include(t => t.TeamUsers)
                 .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            int? userIdCur = HttpContext.Session.GetInt32("UserId");
            if (!userIdCur.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUser = await _context.TeamUsers.FindAsync(teamId, userId);

            if (teamUser == null)
            {
                return NotFound("TeamUser not found");
            }
            teamUser.Role = teamUserDto.Role;

          
            await _context.SaveChangesAsync();

            
            return Ok(teamUser);

           
        }
*/
        /* // DELETE: api/TeamUsers/5
         [HttpDelete("{id}")]
         [Authorize(Roles = "Admin")]

         public async Task<IActionResult> DeleteTeamUser(int id)
         {
             var teamUser = await _context.TeamUsers.FindAsync(id);
             if (teamUser == null)
             {
                 return NotFound();
             }

             _context.TeamUsers.Remove(teamUser);
             await _context.SaveChangesAsync();

             return NoContent();
         }
 */
        [HttpPut("{teamId}")]
        public async Task<IActionResult> PutTeamUser(int teamId, TeamUserDTO teamUserDTO)
        {
            var team = await _context.Teams
               .Include(t => t.TeamUsers)
               .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == teamId).ToListAsync();
            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {

                foreach (var teamUser in teamUsers)
                {
                    if (teamUser.UserId == userId && teamUser.TeamId == teamId)
                    {
                        if (teamUser.Role == "Admin")
                        {
                            var teamUserLo = await _context.TeamUsers
             .FirstOrDefaultAsync(tu => tu.UserId == teamUserDTO.UserId && tu.TeamId == teamId);

                            if (teamUserLo == null)
                            {
                                return NotFound("TeamUser not found");
                            }
                            Console.WriteLine(teamUserLo.UserId + " " + teamUserLo.TeamId + " " + teamUserLo.Role);
                            teamUserLo.Role = teamUserDTO.Role;


                            try
                            {
                                await _context.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                if (!TeamUserExists(teamId))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            return Ok("Put success");
                        }
                        else
                        {
                            return BadRequest("You arent Admin");
                        }
                    }
                }
            }
            return BadRequest("Yon dont belong to this team");
            
        }

        [HttpDelete("{teamId}/{idUser}")]
        public async Task<IActionResult> DeleteTeamUser(int teamId, int idUser)
        {
            var team = await _context.Teams
               .Include(t => t.TeamUsers)
               .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == teamId).ToListAsync();
            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {

                foreach (var teamUser in teamUsers)
                {
                    if (teamUser.UserId == userId && teamUser.TeamId == teamId)
                    {
                        if (teamUser.Role == "Admin")
                        {
                            var teamUserDelete = await _context.TeamUsers.FirstOrDefaultAsync(
                                tu => tu.UserId == idUser && tu.TeamId == teamId);
                            if (teamUserDelete == null)
                            {
                                return NotFound();
                            }
                            _context.TeamUsers.Remove(teamUserDelete);
                            await _context.SaveChangesAsync();
                            return Ok("Delete succes");
                        }
                        else
                        {
                            return BadRequest("You arent Admin");
                        }
                    }
                }
            }
            return BadRequest("Yon dont belong to this team");

        }
        private bool TeamUserExists(int id)
        {
            return _context.TeamUsers.Any(e => e.TeamId == id);
        }
    }
}
