using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoApp.Data;
using ToDoApp.Models;

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

    public async Task<IActionResult> Index(bool includeCompleted = false, CancellationToken cancellationToken = default)
    {
        try
        {
            IQueryable<ToDoList> query = _dbContext.ToDoLists.Include(p => p.Items);

            if (!includeCompleted)
            {
                query = query.Where(p => p.IsCompleted == false);
            }

            var results = await query.ToListAsync(cancellationToken);

            return View(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting ToDoLists.");
            return StatusCode(500);
        }

    }

    public IActionResult Privacy()
    {
        return View();
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
