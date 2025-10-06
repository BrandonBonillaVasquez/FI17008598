Brandon Bonilla Vasquez Carnet FI17008598.

Los comandos de dotnet utilizados (CLI).

dotnet new sln -n PP2
 
dotnet new mvc -o PP2.Web
 
dotnet sln add PP2.Web/

dotnet clean

dotnet build

dotnet run

Páginas web donde halló posibles soluciones a problemas encontrados o Snippets de código.

Se utilizaron chatbots de IA (Copilot, Gemini, ChatGPT) para poder entender los procedimientos y tener una idea base para la ejecución del proyecto.

Prompts (consultas y respuestas) de los chatbots de IA (Copilot, Gemini, ChatGPT, etc.) que haya utilizado.



Este puede ser el vínculo compartido de dicho chatbot.
Las respuestas a las siguientes preguntas:

¿Cuál es el número que resulta al multiplicar, si se introducen los valores máximos permitidos en a y b? Indíquelo en todas las bases (binaria, octal, decimal y hexadecimal).
Respuesta:
El número que resulta al multiplicar los valores máximos permitidos en a y b depende de la cantidad de bits con que se representen los números binarios.

Si consideramos que ambos números tienen 8 bits, el valor máximo que puede representarse es 11111111 en binario, lo cual equivale a 255 en decimal.

Al multiplicar ambos valores, se obtiene 255 por 255, cuyo resultado es 65 025 en decimal.

Este mismo número puede expresarse en las distintas bases de la siguiente manera:

En binario, se representa como 1111111000000001.

En octal, se representa como 177401.

En decimal, es 65 025.

En hexadecimal, se representa como FE01.

Por lo tanto, el número resultante de la multiplicación de los valores máximos permitidos en a y b (considerando 8 bits) es 1111111000000001 en binario, 177401 en octal, 65 025 en decimal y FE01 en hexadecimal.


¿Es posible hacer las operaciones en otra capa? Si sí, ¿en cuál sería?

Respuesta:
Sí, es posible realizar las operaciones en otra capa, y esa capa sería la capa de lógica de negocio (también llamada Business Logic Layer o BLL).

En una arquitectura por capas, las responsabilidades se dividen de la siguiente manera:

Capa de presentación (UI): se encarga de mostrar la información al usuario y recibir sus entradas.

Capa de lógica de negocio (BLL): se encarga de realizar los cálculos, validaciones y reglas de negocio.

Capa de datos (DAL): se encarga de conectarse y comunicarse con la base de datos.

Por tanto, las operaciones o cálculos no deberían hacerse en la capa de presentación, sino en la capa de lógica de negocio, que es la encargada de procesar la información antes de enviarla o mostrarla.