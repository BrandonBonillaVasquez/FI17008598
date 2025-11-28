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

Consulta: operaciones and, or, xor
Respuesta:
Las operaciones AND, OR y XOR son operaciones lógicas o bit a bit que se usan en programación, electrónica y matemáticas booleanas.
Aquí te explico cada una de forma clara y con ejemplos:

🟢 1. AND (Y lógico)

Símbolo: AND o &

Regla: El resultado es 1 solo si ambos bits son 1.

A	B	A AND B
0	0	0
0	1	0
1	0	0
1	1	1

Ejemplo:

1010 AND 1100 = 1000


👉 Solo se conserva el 1 cuando ambos bits son 1.

🟠 2. OR (O lógico)

Símbolo: OR o |

Regla: El resultado es 1 si al menos uno de los bits es 1.

A	B	A OR B
0	0	0
0	1	1
1	0	1
1	1	1

Ejemplo:

1010 OR 1100 = 1110


👉 Si cualquiera de los bits es 1, el resultado es 1.

🔵 3. XOR (O exclusivo)

Símbolo: XOR o ^

Regla: El resultado es 1 solo si los bits son diferentes.

A	B	A XOR B
0	0	0
0	1	1
1	0	1
1	1	0

Ejemplo:

1010 XOR 1100 = 0110


👉 Solo da 1 cuando uno de los dos bits es 1, pero no ambos.

Pregunta:
• Cambio de bases: o Binaria (0, 1) o Octal (0, 1, 2, 3, 4, 5, 6, 7) o Decimal (0, 1, 2, 3, 4, 5, 6, 7, 8, 9) o Hexadecimal (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, A, B, C, D, E, F)
Respuesta:
Perfecto 👍 Aquí tienes una explicación clara y resumida sobre el cambio de bases numéricas y las características de cada una:

🔢 Cambio de Bases Numéricas

Los sistemas numéricos son diferentes formas de representar los números según la base que utilicen.
Cada base determina cuántos símbolos se usan y cómo se agrupan los valores.

⚫ 1. Sistema Binario (Base 2)

Dígitos usados: 0, 1

Ejemplo: 1011₂

Equivalente en decimal:

1
×
2
3
+
0
×
2
2
+
1
×
2
1
+
1
×
2
0
=
8
+
0
+
2
+
1
=
11
10
1×2
3
+0×2
2
+1×2
1
+1×2
0
=8+0+2+1=11
10
	​


👉 Se usa en computadoras, ya que los circuitos electrónicos trabajan con dos estados: encendido (1) y apagado (0).

🟢 2. Sistema Octal (Base 8)

Dígitos usados: 0, 1, 2, 3, 4, 5, 6, 7

Ejemplo: 127₈

Equivalente en decimal:

1
×
8
2
+
2
×
8
1
+
7
×
8
0
=
64
+
16
+
7
=
87
10
1×8
2
+2×8
1
+7×8
0
=64+16+7=87
10
	​


👉 Cada dígito octal representa tres bits binarios, por lo que es útil en programación y electrónica.

🔵 3. Sistema Decimal (Base 10)

Dígitos usados: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9

Ejemplo: 572₁₀

Equivalente en sí mismo: Es el sistema que usamos normalmente en la vida cotidiana.

👉 Cada posición vale una potencia de 10 (unidades, decenas, centenas…).

🟣 4. Sistema Hexadecimal (Base 16)

Dígitos usados: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, A, B, C, D, E, F

Donde:

A = 10

B = 11

C = 12

D = 13

E = 14

F = 15

Ejemplo: 2F₁₆

Equivalente en decimal:

2
×
16
1
+
15
×
16
0
=
32
+
15
=
47
10
2×16
1
+15×16
0
=32+15=47
10
	​


👉 Cada dígito hexadecimal representa cuatro bits (un nibble), por lo que se usa mucho en programación, direcciones de memoria y colores (como #FF0000 en HTML).

🧮 Resumen general
Sistema	Base	Símbolos usados	Ejemplo	Equivalente decimal
Binario	2	0, 1	1011₂	11
Octal	8	0–7	127₈	87
Decimal	10	0–9	572₁₀	572
Hexadecimal	16	0–9, A–F	2F₁₆	47


📘 En resumen:
Operación	Símbolo	Regla principal	Ejemplo (binario)	Resultado
AND	&	1 solo si ambos son 1	1010 & 1100	1000
OR	`	`	1 si al menos uno es 1	1010 | 1100
XOR	^	1 si son diferentes	1010 ^ 1100	0110


Fuente: https://chatgpt.com/c/68e534df-1778-8320-be53-278b6d5cd6fb

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