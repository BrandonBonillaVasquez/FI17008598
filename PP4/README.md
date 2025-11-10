# Su nombre y carnet:
***Brandon Bonilla Vásquez Carnet FI17008598.***

# Los comandos de dotnet utilizados (CLI).

1. dotnet new sln -n PP4
2. dotnet new console -n Libros -f net8.0
3. dotnet sln PP4.sln add Libros/Libros.csproj
4. dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
5. dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 9.0.0
6. dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
7. dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
8. dotnet build
9. dotnet ef migrations add InitialCreate
10. dotnet ef database update
11. dotnet run


# Páginas web donde halló posibles soluciones a problemas encontrados o Snippets de código.

* https://learn.microsoft.com/es-es/ef/ef6/modeling/code-first/migrations/
* https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli
* https://www.netmentor.es/entrada/migrations-efcore

# Prompts (consultas y respuestas) de los chatbots de IA (Copilot, Gemini, ChatGPT, etc.) que haya utilizado.

* https://gemini.google.com/share/f04634346ad9

Las respuestas a las siguientes preguntas:
1- ¿Cómo cree que resultaría el uso de la estrategia de Code First para crear y actualizar una base de datos de tipo NoSQL (como por ejemplo MongoDB)? ¿Y con Database First? ¿Cree que habría complicaciones con las Foreign Keys?

**Respuesta:**

Code First en NoSQL: No sería práctico porque NoSQL no sigue un esquema relacional rígido. EF Core está diseñado para bases relacionales, por lo que mapear entidades y migraciones a un modelo flexible como MongoDB sería complejo. Las migraciones perderían sentido porque NoSQL no requiere esquema fijo.

Database First en NoSQL: Tampoco es viable, ya que no existe un esquema definido que pueda reverse-engineer para generar clases.

Foreign Keys: En NoSQL no hay relaciones estrictas, por lo que las FK no se aplican. Se usan referencias o documentos embebidos, lo que rompe la lógica relacional que EF espera.

2- ¿Cuál carácter, además de la coma (,) y el Tab (\t), se podría usar para separar valores en un archivo de texto con el objetivo de ser interpretado como una tabla (matriz)? ¿Qué extensión le pondría y por qué? Por ejemplo: Pipe (|) con extensión .pipe.

**Respuesta:**

Se podría usar el pipe (|) como separador, porque es poco común en datos textuales y reduce conflictos con comas o espacios.

La extensión podría ser .pipe, indicando claramente el tipo de delimitador usado. Esto facilita que herramientas y scripts identifiquen el formato sin ambigüedad.




