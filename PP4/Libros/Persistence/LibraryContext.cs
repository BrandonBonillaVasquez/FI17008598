using Microsoft.EntityFrameworkCore;
using Libros.Models;

namespace Libros.Data;

public class LibraryContext : DbContext
{
    public LibraryContext() { }
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Title> Titles => Set<Title>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TitleTag> TitleTags => Set<TitleTag>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Si la Factory ya configuró, no sobreescribir.
        if (optionsBuilder.IsConfigured) return;

        // Usar carpeta Data/ (mayúscula) dentro del PROJECT
        var projectRoot = GetProjectRoot();
        var dataDir = Path.Combine(projectRoot, "Data");
        Directory.CreateDirectory(dataDir);
        var dbPath = Path.Combine(dataDir, "books.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tabla intermedia con nombre "TitlesTags"
        modelBuilder.Entity<TitleTag>().ToTable("TitlesTags");

        // Relaciones
        modelBuilder.Entity<Title>()
            .HasOne(t => t.Author)
            .WithMany(a => a.Titles)
            .HasForeignKey(t => t.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TitleTag>()
            .HasOne(tt => tt.Title)
            .WithMany(t => t.TitlesTags)
            .HasForeignKey(tt => tt.TitleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TitleTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TitlesTags)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices de unicidad para evitar duplicados lógicos
        modelBuilder.Entity<Author>().HasIndex(a => a.AuthorName).IsUnique();
        modelBuilder.Entity<Tag>().HasIndex(t => t.TagName).IsUnique();
        modelBuilder.Entity<Title>().HasIndex(t => new { t.AuthorId, t.TitleName }).IsUnique();
        modelBuilder.Entity<TitleTag>().HasIndex(tt => new { tt.TitleId, tt.TagId }).IsUnique();

        // Orden de columnas en Title: TitleId, AuthorId, TitleName
        modelBuilder.Entity<Title>().Property(t => t.TitleId).HasColumnOrder(0);
        modelBuilder.Entity<Title>().Property(t => t.AuthorId).HasColumnOrder(1);
        modelBuilder.Entity<Title>().Property(t => t.TitleName).HasColumnOrder(2);
    }

    // Ubica la raíz del proyecto (donde está Libros.csproj) desde bin/...
    private static string GetProjectRoot()
    {
        var dir = AppContext.BaseDirectory;
        DirectoryInfo? d = new DirectoryInfo(dir);
        while (d != null)
        {
            if (d.GetFiles("*.csproj").Any())
                return d.FullName;
            d = d.Parent;
        }
        return Directory.GetCurrentDirectory();
    }
}