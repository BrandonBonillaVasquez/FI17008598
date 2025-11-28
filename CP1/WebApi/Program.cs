using System.Xml.Serialization; // Necesario para la serialización a XML
using Microsoft.AspNetCore.Mvc; // ¡NECESARIO para [FromForm] y [FromHeader]!
using System.Text; // Necesario para escribir el XML

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// La lista se mantiene en memoria.
var list = new List<object>();
var random = new Random(); // Instancia de Random reusable.

// ----------------------------------------------------------------
// GET: / (Redirige a Swagger)
// ----------------------------------------------------------------
app.MapGet("/", () => Results.Redirect("/swagger"));

// ----------------------------------------------------------------
// POST: / (Obtener la lista) - IMPLEMENTACIÓN DE ACTUALIZACIÓN (XML)
// Parámetro opcional 'xml' en los Headers
// ----------------------------------------------------------------
app.MapPost("/", ([FromHeader(Name = "xml")] bool xml = false) =>
{
    if (xml)
    {
        // Retorna en formato XML
        try
        {
            var serializer = new XmlSerializer(typeof(List<object>));
            var stringBuilder = new StringBuilder();
            
            // Usamos StringWriter para capturar la salida del serializador
            using (var writer = new StringWriter(stringBuilder))
            {
                serializer.Serialize(writer, list);
                // Retorna ContentResult con el tipo de contenido "application/xml"
                return Results.Content(stringBuilder.ToString(), "application/xml");
            }
        }
        catch (Exception)
        {
            // En caso de que la serialización falle por algún motivo
            return Results.StatusCode(500); 
        }
    }
    
    // Retorna por defecto en formato JSON
    return Results.Ok(list); 
});

// ----------------------------------------------------------------
// PUT: / (Agregar elementos) - IMPLEMENTACIÓN DE MEJORA (Validaciones 400)
// ----------------------------------------------------------------
app.MapPut("/", ([FromForm] int quantity, [FromForm] string type) =>
{
    // --- VALIDACIONES PARA STATUS 400 ---

    // 1. Validar que 'quantity' sea mayor que cero
    if (quantity <= 0)
    {
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" }); // Status 400
    }

    // 2. Validar que 'type' sea 'int' o 'float'
    string typeLower = (type?? "").ToLower();
    if (typeLower != "int" && typeLower != "float")
    {
        return Results.BadRequest(new { error = "'type' must be either 'int' or 'float'" }); // Status 400
    }

    // --- LÓGICA DE AGREGAR ELEMENTOS ---
    if (typeLower == "int")
    {
        for (int i = 0; i < quantity; i++)
        {
            // Agregado: Random.Next() con límites para números más manejables (0 - 1000)
            list.Add(random.Next(0, 1001)); 
        }
    }
    else // typeLower == "float"
    {
        for (int i = 0; i < quantity; i++)
        {
            // Agregado: Random.NextSingle() genera un float entre 0.0 y 1.0
            list.Add(random.NextSingle()); 
        }
    }

    return Results.Ok(list); // Status 200 (OK/Success)
}).DisableAntiforgery();

// ----------------------------------------------------------------
// DELETE: / (Eliminar elementos) - IMPLEMENTACIÓN DE MEJORA (Validaciones 400)
// ----------------------------------------------------------------
app.MapDelete("/", ([FromForm] int quantity) =>
{
    // --- VALIDACIONES PARA STATUS 400 ---

    // 1. Validar que 'quantity' sea mayor que cero
    if (quantity <= 0)
    {
        return Results.BadRequest(new { error = "'quantity' must be higher than zero" }); // Status 400
    }

    // 2. Validar que la lista tenga la cantidad suficiente de elementos
    if (list.Count < quantity)
    {
        return Results.BadRequest(new { error = $"List has only {list.Count} elements, cannot delete {quantity}" }); // Status 400
    }

    // --- LÓGICA DE ELIMINAR ELEMENTOS ---
    // RemoveRange es más eficiente que usar un bucle RemoveAt(0)
    list.RemoveRange(0, quantity);

    return Results.Ok(list); // Status 200 (OK/Success)
}).DisableAntiforgery();

// ----------------------------------------------------------------
// PATCH: / (Limpiar la lista) - IMPLEMENTACIÓN
// ----------------------------------------------------------------
app.MapPatch("/", () =>
{
    list.Clear(); // Limpia todos los elementos de la lista
    return Results.Ok(new { message = "List cleared successfully" }); // Status 200
});

app.Run();
//https://gemini.google.com/share/1c78ee5e165c