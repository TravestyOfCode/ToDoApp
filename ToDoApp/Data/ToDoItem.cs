using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ToDoApp.Data;

public class ToDoItem
{
    public int Id { get; set; }

    public int ToDoListId { get; set; }

    public ToDoList ToDoList { get; set; }

    public bool IsCompleted { get; set; }

    public string Description { get; set; }
}

public class ToDoItemConfiguration : IEntityTypeConfiguration<ToDoItem>
{
    public void Configure(EntityTypeBuilder<ToDoItem> builder)
    {
        builder.ToTable(nameof(ToDoItem));

        builder.Property(p => p.Description)
            .IsRequired(true);
    }
}
