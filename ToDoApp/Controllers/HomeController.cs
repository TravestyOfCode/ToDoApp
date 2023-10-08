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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
