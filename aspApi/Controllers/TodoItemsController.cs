﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using aspApi.Data;
using Microsoft.AspNetCore.Authorization;

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
        [HttpGet("GetTodoItemsByTeam/{teamId}")]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItemsByTeam(int teamId)
        {
            // Lấy danh sách TodoItem của một Team dựa trên teamId
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
    }
}
