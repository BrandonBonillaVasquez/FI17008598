using Libros.Data;
using Libros.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO; // TextFieldParser

Console.OutputEncoding = System.Text.Encoding.UTF8;

using var db = new LibraryContext();

// ¿BD vacía?
var isEmpty = !await db.Authors.AnyAsync();

if (isEmpty)
{
    Console.WriteLine("La base de datos está vacía, por lo que será llenada a partir de los datos del archivo CSV.");
    Console.Write("Procesando... ");

    var csvPath = Path.Combine(GetProjectRoot(), "data", "books.csv");
    await LoadCsvAsync(db, csvPath);

    Console.WriteLine("Listo.");
}
else
{
    Console.WriteLine("La base de datos se está leyendo para crear los archivos TSV.");
    Console.Write("Procesando... ");

    var dataDir = Path.Combine(GetProjectRoot(), "data");
    await ExportTsvAsync(db, dataDir);

    Console.WriteLine("Listo.");
}

static async Task LoadCsvAsync(LibraryContext db, string csvPath)
{
    if (!File.Exists(csvPath))
        throw new FileNotFoundException("No se encontró el archivo CSV", csvPath);

    using var parser = new TextFieldParser(csvPath, System.Text.Encoding.UTF8)
    {
        TextFieldType = FieldType.Delimited,
        HasFieldsEnclosedInQuotes = true
    };
    parser.SetDelimiters(",");

    // Leer encabezado
    if (!parser.EndOfData) parser.ReadFields();

    // Caches en memoria para evitar duplicados y consultas innecesarias
    var authors = new Dictionary<string, Author>(StringComparer.OrdinalIgnoreCase);
    var tags    = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
    var titlesByAuthor = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

    // Entidades a agregar
    var newTitles   = new List<Title>();
    var newLinks    = new List<TitleTag>();

    while (!parser.EndOfData)
    {
        var fields = parser.ReadFields();
        if (fields is null || fields.Length < 3) continue;

        var authorName = fields[0].Trim();
        var titleName  = fields[1].Trim();
        var tagsField  = fields[2].Trim();

        if (string.IsNullOrWhiteSpace(authorName) || string.IsNullOrWhiteSpace(titleName))
            continue;

        // Autor
        if (!authors.TryGetValue(authorName, out var author))
        {
            author = new Author { AuthorName = authorName };
            authors[authorName] = author;
            db.Authors.Add(author);
        }

        // Título único por autor
        if (!titlesByAuthor.TryGetValue(authorName, out var titleSet))
        {
            titleSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            titlesByAuthor[authorName] = titleSet;
        }

        Title title;
        if (!titleSet.Contains(titleName))
        {
            title = new Title { Author = author, TitleName = titleName };
            titleSet.Add(titleName);
            newTitles.Add(title);
            db.Titles.Add(title);
        }
        else
        {
            // Recupera el Title recién creado en newTitles (mismo AuthorName + TitleName)
            title = newTitles.First(t => t.Author.AuthorName.Equals(authorName, StringComparison.OrdinalIgnoreCase)
                                       && t.TitleName.Equals(titleName, StringComparison.OrdinalIgnoreCase));
        }

        // Tags separadas por pipe '|'
        var tagItems = tagsField.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var tagName in tagItems)
        {
            if (!tags.TryGetValue(tagName, out var tag))
            {
                tag = new Tag { TagName = tagName };
                tags[tagName] = tag;
                db.Tags.Add(tag);
            }

            // Vinculación; evitamos duplicado lógico en memoria
            var alreadyLinked = newLinks.Any(tt =>
                tt.Title == title && tt.Tag.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (!alreadyLinked)
            {
                var link = new TitleTag { Title = title, Tag = tag };
                newLinks.Add(link);
                db.TitleTags.Add(link);
            }
        }
    }

    await db.SaveChangesAsync();
}

static async Task ExportTsvAsync(LibraryContext db, string dataDir)
{
    Directory.CreateDirectory(dataDir);

    // ✅ Consulta plana con JOINs explícitos (sin APPLY)
    var rows = await (
        from t  in db.Titles.AsNoTracking()
        join a  in db.Authors.AsNoTracking()   on t.AuthorId equals a.AuthorId
        join tt in db.TitleTags.AsNoTracking() on t.TitleId equals tt.TitleId
        join tg in db.Tags.AsNoTracking()      on tt.TagId equals tg.TagId
        select new
        {
            AuthorName = a.AuthorName,
            TitleName  = t.TitleName,
            TagName    = tg.TagName
        }
    ).ToListAsync();

    // Agrupar por primera letra del autor
    var groups = rows.GroupBy(r =>
    {
        var ch = string.IsNullOrEmpty(r.AuthorName) ? '#' : char.ToUpperInvariant(r.AuthorName[0]);
        return char.IsLetterOrDigit(ch) ? ch : '#';
    });

    foreach (var grp in groups)
    {
        var fileName = Path.Combine(dataDir, $"{grp.Key}.tsv");

        // Orden descendente por PRIMERA letra: Autor > Título > Tag
        var ordered = grp.OrderByDescending(r => FirstCharKey(r.AuthorName))
                         .ThenByDescending(r => FirstCharKey(r.TitleName))
                         .ThenByDescending(r => FirstCharKey(r.TagName))
                         .ToList();

        using var sw = new StreamWriter(fileName, false, new System.Text.UTF8Encoding(false));
        await sw.WriteLineAsync("AuthorName\tTitleName\tTagName");
        foreach (var r in ordered)
            await sw.WriteLineAsync($"{r.AuthorName}\t{r.TitleName}\t{r.TagName}");
    }

    static string FirstCharKey(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "\0";
        var ch = char.ToUpperInvariant(s[0]);
        return new string(ch, 1);
    }
}

static string GetProjectRoot()
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