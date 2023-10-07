using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<ToDoItem> ToDoItems { get; set; }

    public DbSet<ToDoList> ToDoLists { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    }
}
