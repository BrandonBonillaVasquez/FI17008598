using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var myList = new List<string>();

app.MapGet("/", () => Results.Redirect("/swagger/index.html"));

app.MapGet("/obtain", () => myList);

app.MapGet("/obtain/{index:int}", (int index) =>
{
    if (index < myList.Count && index > -1)
    {
        var value = myList[index];
        return Results.Ok(value);
    }

    else

    {
        return Results.NotFound($"Item in [{index}] not found");
    }
    
});

app.MapPost("/include", ([FromForm] string item) =>
{
    myList.Add(item);
    return Results.Created("/include/{item}", item);
}).DisableAntiforgery();


app.MapPut("/replace/{index:int}", ([FromRoute] int index, [FromForm] string value) =>
{
    if (index < myList.Count && index > -1)
    {
        myList[index] = value;
        return Results.Ok($"Item in [{index}] replace to '{value}'");
    }
    else
    {
        return Results.NotFound($"Item in [{index}] not found");
    }
}).DisableAntiforgery();


app.MapDelete("/erase/{index:int}", (int index) =>
{
    if (index < myList.Count && index > -1)
    {
        myList.RemoveAt(index);
        return Results.Ok($"Item in [{index}] deleted");
    }
    else
    {
        return Results.NotFound($"Item in [{index}] not found");
    }
});


app.MapDelete("/erase", ([FromForm] string value) =>
{
    var index = myList.IndexOf(value);
    if (index > -1)
    {
        myList.RemoveAt(index);
        return Results.Ok($"Item in [{index}] deleted");
    }
    else
    {
        return Results.NotFound($"Item '${value}' not found");
    }


}).DisableAntiforgery();

app.Run();

