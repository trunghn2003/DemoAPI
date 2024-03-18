using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using aspApi.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using aspApi.Models;
using NuGet.Protocol;
using Microsoft.CodeAnalysis;

namespace aspApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public TodoItemsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        /* [HttpGet]
         [Authorize(Roles = "Admin, User")]

         public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
         {
             return await _context.TodoItems.ToListAsync();
         }*/

        // GET: api/TodoItems/5
        /*[HttpGet("{id}")]
        [Authorize(Roles = "Admin, User")]

        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }
*/
        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /*[HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin, User")]


        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.TodoItemId)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }*/

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{teamId}")]
        public async Task<ActionResult<TodoItem>> PostTodoItemForTeam(int teamId, TodoItem todoItem)
        {
            var team = await _context.Teams
                .Include(t => t.TeamUsers)
                .FirstOrDefaultAsync(t => t.TeamId ==teamId);

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
                                var team1 = await _context.Teams
                    .Include(t => t.TodoItems)
                    .FirstOrDefaultAsync(t => t.TeamId == teamId);

                            if (team1 == null)
                            {
                                return NotFound("Team not found");
                            }

                            /*todoItem.Tea = teamId;*//**/

                            // Thêm TodoItem vào Team
                            team1.TodoItems.Add(todoItem);

                            _context.Entry(team1).State = EntityState.Modified;

                            await _context.SaveChangesAsync();

                            return Ok(todoItem);
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
        [HttpPut("{teamId}/{todoItemId}")]
        public async Task<ActionResult<TodoItem>> PutTodoItemForTeam(int teamId, long todoItemId, TodoItem todoItem)
        {
            // Lấy thông tin Team từ cơ sở dữ liệu
            var team = await _context.Teams
                .Include(t => t.TeamUsers)
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            // Kiểm tra xem UserId đã được đăng nhập không
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Unauthorized("User not logged in");
            }

            // Kiểm tra quyền của người dùng
            var teamUser = await _context.TeamUsers.FirstOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == userId);
            if (teamUser == null || teamUser.Role != "Admin")
            {
                return Forbid("You are not authorized to perform this action");
            }

            // Lấy thông tin TodoItem từ cơ sở dữ liệu
            var existingTodoItem = await _context.TodoItems.FindAsync(todoItemId);
            if (existingTodoItem == null)
            {
                return NotFound("TodoItem not found");
            }

            // Cập nhật thông tin TodoItem
            existingTodoItem.Name = todoItem.Name;
            existingTodoItem.IsComplete = todoItem.IsComplete;

            await _context.SaveChangesAsync();

            return Ok(existingTodoItem);
        }



        [HttpDelete("{teamId}/{todoItemId}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItemFromTeam(int teamId, long todoItemId)
        {
            // Lấy thông tin Team từ cơ sở dữ liệu
            var team = await _context.Teams
                .Include(t => t.TeamUsers)
                .FirstOrDefaultAsync(t => t.TeamId == teamId);

            if (team == null)
            {
                return NotFound("Team not found");
            }

            // Kiểm tra xem UserId đã được đăng nhập không
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Unauthorized("User not logged in");
            }

            // Kiểm tra quyền của người dùng
            var teamUser = await _context.TeamUsers.FirstOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == userId);
            if (teamUser == null || teamUser.Role != "Admin")
            {
                return Forbid("You are not authorized to perform this action");
            }

            // Lấy thông tin TodoItem từ cơ sở dữ liệu
            var todoItem = await _context.TodoItems.FindAsync(todoItemId);
            if (todoItem == null)
            {
                return NotFound("TodoItem not found");
            }

            // Xóa TodoItem khỏi Team
            team.TodoItems.Remove(todoItem);
            
            await _context.SaveChangesAsync();

            return Ok(todoItem);
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.TodoItemId == id);
        }
       
        private  bool IsUserAuthorizedForTeam(ClaimsPrincipal user, Team team)
        {
          
           

            // Kiểm tra xem người dùng có là thành viên của nhóm không
            var userId = GetUserIdFromClaims(user);
            var teamId = team.TeamId;
            Console.WriteLine(userId + " " + teamId);
            var teamUser = team.TeamUsers;
            foreach(var i in teamUser)
            {
                if (i.UserId == userId && i.TeamId == teamId) return true;
                
            }
            //var teamUser = team.TeamUsers;
            //int tmp = 0;
            //if (teamUser == null)
            //{
            //    Console.WriteLine("Team Users is null");
            //}
            //else
            //{
            //    foreach (var user1 in teamUser)
            //    {
            //        Console.WriteLine($"User ID: {user1.TeamId}");
            //        tmp = user1.TeamId;
            //        break;
            //    }
            //}

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
    }
}
