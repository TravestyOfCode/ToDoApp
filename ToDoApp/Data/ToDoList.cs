using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace ToDoApp.Data;

public class ToDoList
{
    public int Id { get; set; }

    public string Title { get; set; }

    public bool IsCompleted { get; set; }

    public List<ToDoItem> Items { get; set; }
}

public class ToDoListConfiguration : IEntityTypeConfiguration<ToDoList>
{
    public void Configure(EntityTypeBuilder<ToDoList> builder)
    {
        builder.ToTable(nameof(ToDoList));

        builder.Property(p => p.Title)
            .IsRequired(true)
            .HasMaxLength(64);
    }
}
