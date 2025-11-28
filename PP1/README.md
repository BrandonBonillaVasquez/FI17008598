Brandon Bonilla Vasquez Carnet FI17008598.

Los comandos de dotnet utilizados (CLI).
dotnet new sln -n PP1
dotnet new console -o PP1
dotnet sln add PP1/

Páginas web donde halló posibles soluciones a problemas encontrados o Snippets de código.
Google/Gemini

Prompts (consultas y respuestas) de los chatbots de IA (Copilot, Gemini, ChatGPT, etc.) que haya utilizado.

Consulta: La fórmula de la suma aritmética de números enteros positivos (1 + 2 + 3 + … + n) se le atribuye a Carl Friedrich Gauss

Respuesta: Sí, la anécdota de la suma de los primeros 100 números enteros positivos se le atribuye a Carl Friedrich Gauss.

Cuenta la leyenda que, cuando Gauss era un niño en la escuela primaria, su maestro, para mantener ocupados a los alumnos, les pidió que sumaran todos los números del 1 al 100. El maestro esperaba que esto les llevara un buen rato.

Sin embargo, el joven Gauss encontró una solución ingeniosa y rápida. Se dio cuenta de que si emparejaba los números de los extremos, la suma de cada pareja era siempre la misma:

1 + 100 = 101

2 + 99 = 101

3 + 98 = 101

... y así sucesivamente.

Como había 100 números, habría 50 de estas parejas. Por lo tanto, la suma total sería simplemente 50 multiplicado por 101, lo que da como resultado 5050.

La Fórmula General
Este método lo llevó a desarrollar la fórmula general para la suma de una progresión aritmética de números enteros positivos desde 1 hasta un número cualquiera n:

S 
n
​
 = 
2
n(n+1)
​
 
Donde:

S 
n
​
  es la suma de los números.

n es el último número de la serie.

Aplicando la fórmula al problema original (sumar del 1 al 100):

S 
100
​
 = 
2
100(100+1)
​
 = 
2
100×101
​
 = 
2
10100
​
 =5050
-----------------------------------------------

Consulta: cuales son los metodos SumFor y SumIte

Respuesta: 

SumFor: Suma con un Bucle for
Este método es el más tradicional y directo. Consiste en recorrer una colección de números utilizando un bucle for que se controla mediante un índice (por ejemplo, i). Se inicializa una variable acumuladora en cero y, en cada paso del bucle, se añade el elemento correspondiente a esa posición.

La lógica es la siguiente:

Crear una variable suma e inicializarla en 0.

Iniciar un bucle que va desde el primer hasta el último elemento de la colección, usando su índice.

En cada iteración, obtener el elemento en la posición actual y sumarlo a la variable suma.

Al finalizar el bucle, suma contendrá el resultado total.

Características:

Control explícito: Tienes control total sobre el índice y cómo accedes a los elementos.

Claridad: Es muy fácil de entender para principiantes.

Uso común: Es una de las primeras técnicas que se aprenden en programación.


SumIte: Suma con un Iterador
Este método es más moderno y a menudo más "elegante" o abstracto. En lugar de manejar índices, se utiliza un iterador, que es un objeto diseñado para recorrer una colección elemento por elemento. Los bucles for-each (como el for item in coleccion: de Python) utilizan iteradores de forma implícita.

La lógica es:

Crear una variable suma e inicializarla en 0.

Pedir a la colección un iterador que sepa cómo entregar cada elemento uno por uno.

Usar un bucle que, en cada paso, obtiene el siguiente elemento disponible del iterador y lo suma a la variable suma.

El bucle termina cuando el iterador ya no tiene más elementos que entregar.

Características:

Abstracción: No te preocupas por los índices ni por el tamaño de la colección. Solo te interesa obtener cada elemento.

Flexibilidad: Funciona con cualquier objeto que sea "iterable", no solo con listas o arreglos (por ejemplo, conjuntos, generadores, etc.).

Legibilidad: El código suele ser más corto y directo, ya que se enfoca en "qué hacer" con cada elemento en lugar de en "cómo llegar" a él.


Vínculo compartido de dicho chatbot:

https://gemini.google.com/app/41290504b4b3f606

La respuesta a la siguientes preguntas:

¿Por qué todos los valores resultantes tanto de n como de sum difieren entre métodos (fórmula e implementación iterativa) y estrategias (ascendente y descendente)?
Respuesta: Ascendente (1 a Max): Esta estrategia encuentra el último n válido porque el programa se detiene en el primer valor que causa un desbordamiento. El valor de n justo antes de ese desbordamiento es el que se reporta.

Descendente (Max a 1): Esta estrategia encuentra el primer n que, al ser calculado desde un valor muy grande, da un resultado positivo. Como los números se desbordan de forma cíclica (de positivo a negativo y de vuelta a positivo), el resultado no es el mismo que el del cálculo ascendente, ya que la búsqueda solo se detiene al encontrar un valor válido, sin importar cuántos desbordamientos hayan ocurrido.


¿Qué cree que sucedería si se utilizan las mismas estrategias (ascendente y descendente) pero con el método recursivo de suma (SumRec)? [si desea puede implementarlo y observar qué sucede en ambos escenarios]
Respuesta:
Si se utiliza el método recursivo para la suma (SumRec), el programa fallará mucho antes de que se produzca un desbordamiento de enteros.

La razón: Desbordamiento de la pila (Stack Overflow)
La recursión funciona creando una nueva "capa" en la memoria (la pila) para cada llamada a la función. Para sumar los números hasta int.MaxValue, el programa necesitaría miles de millones de capas en la pila. La memoria del sistema no puede manejar tantas llamadas, por lo que la pila se desborda y el programa se cierra con un error de StackOverflowException.

Esto es lo que sucedería en ambos escenarios:

Estrategia Ascendente (1 a Max):

El programa no llegará a un desbordamiento de int.
Se bloqueará y se cerrará cuando el valor de n sea relativamente pequeño (alrededor de 10,000 a 20,000, dependiendo de tu sistema), mucho antes de que la suma se acerque a int.MaxValue.

Estrategia Descendente (Max a 1):

El programa fallará casi al instante.
La primera llamada a SumRec intentará anidar miles de millones de llamadas más, lo que causará un StackOverflowException inmediatamente.