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
        [HttpGet]
        [Authorize(Roles = "Admin, User")]

        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
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

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]


        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new { id = todoItem.TodoItemId }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.TodoItemId == id);
        }
        // GET: api/Todo/GetTodoItemsByTeam/{teamId}
        //[HttpGet("GetTodoItemsByTeam/{teamId}")]
        //public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItemsByTeam(int teamId)
        //{
        //    // Lấy danh sách TodoItem của một Team dựa trên teamId
        //    var todoItems = await _context.Teams
        //        .Where(t => t.TeamId == teamId)
        //        .SelectMany(t => t.TodoItems)
        //        .ToListAsync();

        //    if (todoItems == null || todoItems.Count == 0)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(todoItems);
        //}
        [HttpGet("{teamId}/todoItems")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetTodoItemsForTeam(int teamId)
        {
            var team = await _context.Teams
        .Include(t => t.TeamUsers)
        .FirstOrDefaultAsync(t => t.TeamId == teamId);

           
            {
                
            }
            if (team == null)
            {
                return NotFound("Team not found");
            }



            // Kiểm tra xem người dùng có quyền truy cập vào nhóm không
            var currentUser = HttpContext.User;
            
            //Console.WriteLine(currentUser.ToJson);
            if (!IsUserAuthorizedForTeam(currentUser, team))
            {
                return Forbid(); // Hoặc có thể trả về NotFound() tùy thuộc vào logic ứng dụng của bạn
            }
            var todoItems = await _context.Teams
                .Where(t => t.TeamId == teamId)
                .SelectMany(t => t.TodoItems)
                .ToListAsync();

            if (todoItems == null || todoItems.Count == 0)
            {
                return NotFound();
            }
            return Ok(todoItems);
 
        }


        private  bool IsUserAuthorizedForTeam(ClaimsPrincipal user, Team team)
        {
            // Kiểm tra xem người dùng có vai trò "Admin" không
            if (user.IsInRole("Admin"))
            {
                return true;
            }

            // Kiểm tra xem người dùng có là thành viên của nhóm không
            var userId = GetUserIdFromClaims(user);
            var teamId = team.TeamId;
            Console.WriteLine(userId + " " + teamId);
            var teamUser = team.TeamUsers;
            foreach(var i in teamUser)
            {
                if (i.UserId == userId && i.TeamId == teamId) return true;
                else return false;
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
