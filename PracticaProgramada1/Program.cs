using System;

class Program
{
    // SumFor: utiliza la fórmula de Gauss, con manejo de desbordamiento
    public static int SumFor(int n)
    {
        checked
        {
            return n * (n + 1) / 2;
        }
    }

    // SumIte: utiliza una versión iterativa, con manejo de desbordamiento
    public static int SumIte(int n)
    {
        int sum = 0;
        checked
        {
            for (int i = 1; i <= n; i++)
            {
                sum += i;
            }
        }
        return sum;
    }

    // Método para validar un 'sum' (suma)
    public static bool IsValidSum(int sum)
    {
        return sum > 0;
    }

    static void Main(string[] args)
    {
        Console.WriteLine("• SumFor:");
        Console.WriteLine($"\t◦ From 1 to Max → n: {FindLastValidAsc(SumFor, 1)}");
        Console.WriteLine($"\t◦ From Max to 1 → n: {FindFirstValidDesc(SumFor, int.MaxValue)}");

        Console.WriteLine("\n• SumIte:");
        Console.WriteLine($"\t◦ From 1 to Max → n: {FindLastValidAsc(SumIte, 1)}");
        Console.WriteLine($"\t◦ From Max to 1 → n: {FindFirstValidDesc(SumIte, int.MaxValue)}");
    }

    // Busca el último 'n' válido de forma ascendente
    public static string FindLastValidAsc(Func<int, int> sumMethod, int startN)
    {
        int lastValidN = 0;
        int lastValidSum = 0;

        for (int n = startN; n <= int.MaxValue; n++)
        {
            try
            {
                int currentSum = sumMethod(n);
                if (IsValidSum(currentSum))
                {
                    lastValidN = n;
                    lastValidSum = currentSum;
                }
                else
                {
                    break;
                }
            }
            catch (OverflowException)
            {
                // El desbordamiento indica el final de los valores válidos
                break;
            }
        }
        return $"{lastValidN} → sum: {lastValidSum}";
    }

    // Busca el primer 'n' válido de forma descendente
    public static string FindFirstValidDesc(Func<int, int> sumMethod, int startN)
    {
        for (int n = startN; n >= 1; n--)
        {
            try
            {
                int currentSum = sumMethod(n);
                if (IsValidSum(currentSum))
                {
                    return $"{n} → sum: {currentSum}";
                }
            }
            catch (OverflowException)
            {
                // Ignoramos el desbordamiento y seguimos buscando
                // El bucle de for ya manejará el decremento del valor
            }
        }
        return "No se encontró un valor válido.";
    }
}