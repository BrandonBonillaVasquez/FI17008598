using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Libros.Data;

public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
{
    public LibraryContext CreateDbContext(string[] args)
    {
        // Cuando corre dotnet ef, el CurrentDirectory suele ser la carpeta del proyecto (Libros)
        var basePath = Directory.GetCurrentDirectory();
        var dataDir  = Path.Combine(basePath, "data");
        Directory.CreateDirectory(dataDir);

        var dbPath = Path.Combine(dataDir, "books.db");

        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        return new LibraryContext(options);
    }
}
