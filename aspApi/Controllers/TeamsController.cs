﻿using System;
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

        [HttpGet("id")]
        public async Task<ActionResult<User>> GetTeam(int id)
        {
            var team = await _context.Teams
             .Include(t => t.TeamUsers)
             .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == id).ToListAsync();
            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {

                foreach (var teamUser in teamUsers)
                {
                    if (teamUser.UserId == userId && teamUser.TeamId == id)
                    {

                        return Ok(team);
                        
                    }
                }
            }
            return BadRequest("Yon dont belong to this team");

        }

            // PUT: api/Teams/5
            // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(int id, TeamDTO teamDTO)
        {
            var team = await _context.Teams
                .Include(t => t.TeamUsers)
                .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == id).ToListAsync();
            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {

                foreach (var teamUser in teamUsers)
                {
                    if (teamUser.UserId == userId && teamUser.TeamId == id)
                    {
                        if (teamUser.Role == "Admin")
                        {
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
                            await _context.SaveChangesAsync();

                            return Ok(new { message = "Team updated successfully" });
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
            return team;

        }




        // DELETE: api/Teams/5
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams
             .Include(t => t.TeamUsers)
             .FirstOrDefaultAsync(t => t.TeamId == id);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == id).ToListAsync();
            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {

                foreach (var teamUser in teamUsers)
                {
                    if (teamUser.UserId == userId && teamUser.TeamId == id)
                    {
                        if (teamUser.Role == "Admin")
                        {
                            _context.Teams.Remove(team);
                            await _context.SaveChangesAsync();

                            return Ok($"Da xoa thanh cong teamId: {id}");
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
        public class UserInTeamDto
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Role { get; set; }

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

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return NotFound("User not login");
            }
            var teamUsers = await _context.TeamUsers.Where(t => t.TeamId == id).ToListAsync();
            if (teamUsers == null || !teamUsers.Any())
            {
                Console.WriteLine("No team users found for the team");
            }
            else
            {

                foreach (var teamUser in teamUsers)
                {
                    if (teamUser.UserId == userId && teamUser.TeamId == id)
                    {
                        if (teamUser.Role == "Admin")
                        {
                            //Console.WriteLine("ko dc");
                            team.IsPublic = isPublic;
                            await _context.SaveChangesAsync();

                            return Ok(new { message = "Team privacy updated successfully" });
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

        private bool IsUserAuthorizedForTeam(Team team)
        {

            var userId = HttpContext.Session.GetInt32("UserId").Value;
            var teamId = team.TeamId;
            Console.WriteLine(userId + " " + teamId);
            var teamUser = team.TeamUsers;
            foreach (var i in teamUser)
            {
                if (i.UserId == userId && i.TeamId == teamId)
                {
                    Console.WriteLine(teamId + " " + userId + " " + i.Role);
                    return true;
                }

            }
            return false;
        }
        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.TeamId == id);
        }
    }   
}
