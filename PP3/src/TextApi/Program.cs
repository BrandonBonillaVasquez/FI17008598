using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"))
   .WithTags("Meta")
   .WithDescription("Redirects to Swagger UI");

app.MapPost("/include/{position:int}", async (
    HttpRequest req,
    [FromRoute] int position,
    [FromQuery] string? value) =>
{
    bool wantXml = GetXmlHeader(req);

    if (position < 0)
        return BadRequest("'position' must be 0 or higher");
    if (string.IsNullOrWhiteSpace(value))
        return BadRequest("'value' cannot be empty");

    var form = await req.ReadFormAsync();
    var text = form["text"].ToString();
    if (string.IsNullOrWhiteSpace(text))
        return BadRequest("'text' cannot be empty");

    var words = SplitWords(text);
    if (position >= words.Count) words.Add(value!);
    else words.Insert(position, value!);

    var resultText = string.Join(" ", words);
    return FormatResult(text, resultText, wantXml);
})
.WithTags("Operations")
.Produces(200)
.Produces(400);

app.MapPut("/replace/{length:int}", async (
    HttpRequest req,
    [FromRoute] int length,
    [FromQuery] string? value) =>
{
    bool wantXml = GetXmlHeader(req);

    if (length <= 0)
        return BadRequest("'length' must be greater than 0");
    if (string.IsNullOrWhiteSpace(value))
        return BadRequest("'value' cannot be empty");

    var form = await req.ReadFormAsync();
    var text = form["text"].ToString();
    if (string.IsNullOrWhiteSpace(text))
        return BadRequest("'text' cannot be empty");

    var words = SplitWords(text);
    for (int i = 0; i < words.Count; i++)
        if (words[i].Length == length) words[i] = value!;

    var resultText = string.Join(" ", words);
    return FormatResult(text, resultText, wantXml);
})
.WithTags("Operations")
.Produces(200)
.Produces(400);

app.MapDelete("/erase/{length:int}", async (
    HttpRequest req,
    [FromRoute] int length) =>
{
    bool wantXml = GetXmlHeader(req);

    if (length <= 0)
        return BadRequest("'length' must be greater than 0");

    var form = await req.ReadFormAsync();
    var text = form["text"].ToString();
    if (string.IsNullOrWhiteSpace(text))
        return BadRequest("'text' cannot be empty");

    var words = SplitWords(text);
    var filtered = words.Where(w => w.Length != length).ToList();

    var resultText = string.Join(" ", filtered);
    return FormatResult(text, resultText, wantXml);
})
.WithTags("Operations")
.Produces(200)
.Produces(400);

app.Run();

// Helpers
static List<string> SplitWords(string text)
    => text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

static IResult BadRequest(string message)
    => Results.Json(new { error = message }, statusCode: 400);

static bool GetXmlHeader(HttpRequest req)
{
    if (!req.Headers.TryGetValue("xml", out var values)) return false;
    var raw = values.ToString();
    if (string.IsNullOrEmpty(raw)) return false;
    return bool.TryParse(raw, out var b) && b;
}

static IResult FormatResult(string ori, string @new, bool xml)
{
    if (!xml) return Results.Json(new ResultDto(ori, @new));

    var xdoc = new XDocument(
        new XDeclaration("1.0", "utf-16", null),
        new XElement("Result",
            new XElement("Ori", ori),
            new XElement("New", @new))
    );
    return Results.Text(xdoc.ToString(), "application/xml");
}

public record ResultDto(string ori, string @new);