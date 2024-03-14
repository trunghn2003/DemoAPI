using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspApi.Models;
using aspApi.Data;
using static aspApi.Controllers.TeamUsersController;
using aspApi.DTO;

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
        
        // GET: api/TeamUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamUser>>> GetTeamUsers()
        {
            return await _context.TeamUsers.ToListAsync();
        }

        // GET: api/TeamUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamUser>> GetTeamUser(int id)
        {
            var teamUser = await _context.TeamUsers.FindAsync(id);

            if (teamUser == null)
            {
                return NotFound();
            }

            return teamUser;
        }

        // POST: api/TeamUsers
        [HttpPost]
        public async Task<ActionResult<TeamUser>> PostTeamUser(TeamUserDto teamUserDto)
        {
            var team = await _context.Teams.FindAsync(teamUserDto.TeamId);
            if (team == null)
            {
                ModelState.AddModelError("TeamId", "Team not found.");
            }

            var user = await _context.User.FindAsync(teamUserDto.UserId);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "User not found.");
            }

            // If either team or user is not found, return bad request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map TeamUserDTO to TeamUser entity
            var teamUser = new TeamUser
            {
                TeamId = teamUserDto.TeamId,
                UserId = teamUserDto.UserId,
                Role = teamUserDto.Role
            };

            // Add and save the new TeamUser entity
            _context.TeamUsers.Add(teamUser);
            await _context.SaveChangesAsync();

            // Return the created TeamUserDTO
            return CreatedAtAction(nameof(GetTeamUser), new { id = teamUser.TeamId }, teamUserDto);
        }

        // DELETE: api/TeamUsers/5
        [HttpDelete("{id}")]
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

        private bool TeamUserExists(int id)
        {
            return _context.TeamUsers.Any(e => e.TeamId == id);
        }
    }
}
