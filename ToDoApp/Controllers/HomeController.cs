using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoApp.Data;
using ToDoApp.Models;
using ToDoApp.Utilities;

namespace ToDoApp.Controllers;
public class HomeController : Controller
{
    private readonly AppDbContext _dbContext;

    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext dbContext, ILogger<HomeController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [Route("/")]
    [Route("[controller]/")]
    [Route("[controller]/Index")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<ToDoList> query = _dbContext.ToDoLists.Include(p => p.Items);

            var results = await query.OrderByDescending(p => p.Id).ToListAsync(cancellationToken);

            return View(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting ToDoLists.");
            return StatusCode(500);
        }

    }

    [HttpGet("[controller]/Index/{listId}")]
    public async Task<IActionResult> Index(int listId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _dbContext.ToDoLists.Include(tdl => tdl.Items).SingleOrDefaultAsync(tdl => tdl.Id.Equals(listId), cancellationToken);

            if (entity == null)
            {
                return NotFound();
            }

            return PartialView("ToDoListPartial", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting ToDoList by Id.");

            return StatusCode(500);
        }
    }

    public async Task<IActionResult> CreateList(CancellationToken cancellationToken)
    {
        try
        {
            string title = HttpContext.Request.Headers["HX-Prompt"];

            if (title.IsWhiteSpace())
            {
                return StatusCode(400, "Invalid Title");
            }

            var entity = _dbContext.ToDoLists.Add(new ToDoList() { Title = title, Items = new List<ToDoItem>() });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return PartialView("NewToDoListPartial", entity.Entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating new ToDoList.");

            return StatusCode(500);
        }
    }

    public async Task<IActionResult> EditList(int listId, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _dbContext.ToDoLists.SingleOrDefaultAsync(tdl => tdl.Id.Equals(listId), cancellationToken);

            if (entity == null)
            {
                return NotFound();
            }

            return PartialView("EditToDoListPartial", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting edit for ToDoList.");

            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditList(int listId, string title, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _dbContext.ToDoLists.Include(tdl => tdl.Items).SingleOrDefaultAsync(tdl => tdl.Id.Equals(listId), cancellationToken);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Title = title;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return PartialView("ToDoListPartial", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving ToDoList changes.");

            return StatusCode(500);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteList(int listId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _dbContext.ToDoLists.SingleOrDefaultAsync(tdl => tdl.Id.Equals(listId), cancellationToken);

            if (entity == null)
            {
                return NotFound();
            }

            _dbContext.ToDoLists.Remove(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting ToDoList.");

            return StatusCode(500);
        }
    }

    // Updates a single ToDoItem's IsCompleted property. If isCompleted is 'on' then it is set to true, otherwise false.
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string isCompleted = "", CancellationToken cancellationToken = default)
    {
        try
        {
            var toDoItem = await _dbContext.ToDoItems.Where(tdi => tdi.Id.Equals(id)).SingleOrDefaultAsync(cancellationToken);
            if (toDoItem == null)
            {
                return NotFound();
            }

            toDoItem.IsCompleted = isCompleted.Equals("on");

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating ToDoItem status.");
            return StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var item = await _dbContext.ToDoItems.Where(tdi => tdi.Id.Equals(id)).SingleOrDefaultAsync(cancellationToken);

            if (item == null)
            {
                return NotFound();
            }

            return PartialView("EditToDoItemPartial", item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error editing a ToDoItem.");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string description, CancellationToken cancellationToken = default)
    {
        try
        {
            var toDoItem = await _dbContext.ToDoItems.SingleOrDefaultAsync(tdi => tdi.Id.Equals(id), cancellationToken);

            if (toDoItem == null)
            {
                return NotFound();
            }

            toDoItem.Description = description;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return PartialView("ToDoItemPartial", toDoItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving ToDoItem changes.");

            return StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var toDoItem = await _dbContext.ToDoItems.SingleOrDefaultAsync(tdi => tdi.Id.Equals(id), cancellationToken);

            if (toDoItem == null)
            {
                return NotFound();
            }

            return PartialView("ToDoItemPartial", toDoItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error canceling editing of ToDoItem.");
            return StatusCode(500);
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var toDoItem = await _dbContext.ToDoItems.SingleOrDefaultAsync(tdi => tdi.Id.Equals(id), cancellationToken);

            if (toDoItem == null)
            {
                return NotFound();
            }

            _dbContext.ToDoItems.Remove(toDoItem);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return StatusCode(200);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error trying to delete ToDoItem.");
            return StatusCode(500);
        }
    }

    [HttpGet]
    public IActionResult Create(int listId, CancellationToken cancellationToken = default)
    {
        try
        {
            return PartialView("CreateToDoItemPartial", listId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating new ToDoItem line.");
            return StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(int listId, string description, CancellationToken cancellationToken = default)
    {
        try
        {
            var listEntity = await _dbContext.ToDoLists.SingleOrDefaultAsync(tdl => tdl.Id.Equals(listId), cancellationToken);

            if (listEntity == null)
            {
                return NotFound();
            }

            var entity = _dbContext.ToDoItems.Add(new ToDoItem() { ToDoListId = listId, Description = description, ToDoList = listEntity });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return PartialView("SavedToDoItemPartial", entity.Entity);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving new ToDoItem.");
            return StatusCode(500);
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
