using System.Xml.Serialization;
using System.Text;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

object FormatError(string message) => new { error = message };

string SerializeToXml(Result result)
{
    var xmlSerializer = new XmlSerializer(typeof(Result));
    using var stringWriter = new StringWriter();
    xmlSerializer.Serialize(stringWriter, result);
    return stringWriter.ToString();
}

app.MapPost("/include/{position:int}", (
    [FromRoute] int position,
    [FromQuery] string value,
    [FromForm] string text,
    [FromHeader(Name = "xml")] bool xml = false) =>
{
    if (position < 0)
        return Results.BadRequest(FormatError("'position' must be 0 or higher"));

    if (string.IsNullOrWhiteSpace(value))
        Results.BadRequest(FormatError("'value' cannot be empty"));

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(FormatError("'text' cannot be empty"));

    var words = text.Split(' ').ToList();
    var ori = text;

    if (position >= words.Count)
        words.Add(value);
    else
        words.Insert(position, value);

    var newText = string.Join(" ", words);
    var result = new Result { Ori = ori, New = newText };

    return xml
        ? Results.Text(SerializeToXml(result), "application/xml")
        : Results.Json(result);
});

app.MapPut("/replace/{length:int}", (
    [FromRoute] int length,
    [FromQuery] string value,
    [FromForm] string text,
    [FromHeader(Name = "xml")] bool xml = false) =>
{
    if (length <= 0)
        return Results.BadRequest(FormatError("'length' must be greater than 0"));

    if (string.IsNullOrWhiteSpace(value))
        return Results.BadRequest(FormatError("'value' cannot be empty"));

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(FormatError("'text' cannot be empty"));

    var words = text.Split(' ');
    var ori = text;
    var newWords = words.Select(w => w.Length == length ? value : w);
    var newText = string.Join(" ", newWords);
    var result = new Result { Ori = ori, New = newText };

    return xml
        ? Results.Text(SerializeToXml(result), "application/xml")
        : Results.Json(result);
});

app.MapDelete("/erase/{length:int}", (
    [FromRoute] int length,
    [FromForm] string text,
    [FromHeader(Name = "xml")] bool xml = false) =>
{
    if (length <= 0)
        return Results.BadRequest(FormatError("'length' must be greater than 0"));

    if (string.IsNullOrWhiteSpace(text))
        return Results.BadRequest(FormatError("'text' cannot be empty"));

    var words = text.Split(' ');
    var ori = text;
    var newWords = words.Where(w => w.Length != length);
    var newText = string.Join(" ", newWords);
    var result = new Result { Ori = ori, New = newText };

    return xml
        ? Results.Text(SerializeToXml(result), "application/xml")
        : Results.Json(result);
});

app.Run();

public class Result
{
    public string Ori { get; set; } = string.Empty;
    public string New { get; set; } = string.Empty;
}