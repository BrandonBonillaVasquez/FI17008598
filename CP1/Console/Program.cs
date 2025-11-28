public class Numbers
{
    private static readonly double N = 25;

    // Instancia estática para poder invocar Round (método de instancia)
    private static readonly Numbers _instance = new Numbers();

    public static double Formula(double z)
    {
        // (z + √(4 + z²)) / 2, redondeado a 10 decimales
        return _instance.Round((z + Math.Sqrt(4 + Math.Pow(z, 2))) / 2);
    }

    public static double Recursive(double z)
    {
        // razón f(z,25) / f(z,24), redondeado a 10 decimales
        return _instance.Round(Recursive(z, N) / Recursive(z, N - 1));
    }

    public static double Iterative(double z)
    {
        // razón f(z,25) / f(z,24), redondeado a 10 decimales
        return _instance.Round(Iterative(z, N) / Iterative(z, N - 1));
    }

    // -------- Implementaciones privadas corregidas --------

    // Recursiva: f(z,0)=1; f(z,1)=1; f(z,n)=z*f(z,n-1)+f(z,n-2)
    private static double Recursive(double z, double n)
    {
        int ni = (int)n; // n llega como double, pero es entero lógico
        if (ni == 0) return 1.0;
        if (ni == 1) return 1.0;
        return z * Recursive(z, ni - 1) + Recursive(z, ni - 2);
    }

    // Iterativa: misma lógica que la recursiva pero con bucle
    private static double Iterative(double z, double n)
    {
        int ni = (int)n;
        if (ni == 0) return 1.0;
        if (ni == 1) return 1.0;

        double a = 1.0; // f(z,0)
        double b = 1.0; // f(z,1)

        for (int i = 2; i <= ni; i++)
        {
            double next = z * b + a; // f(z,i) = z*f(z,i-1) + f(z,i-2)
            a = b;
            b = next;
        }
        return b; // f(z, n)
    }

    private double Round(double value)
    {
        return Math.Round(value, 10);
    }

    public static void Main(string[] args)
    {
        String[] metallics = [
            "Platinum", // [0]
            "Golden",   // [1]
            "Silver",   // [2]
            "Bronze",   // [3]
            "Copper",   // [4]
            "Nickel",   // [5]
            "Aluminum", // [6]
            "Iron",     // [7]
            "Tin",      // [8]
            "Lead",     // [9]
        ];

        for (var z = 0; z < metallics.Length; z++)
        {
            Console.WriteLine("\n[" + z + "] " + metallics[z]);
            Console.WriteLine(" ↳ formula(" + z + ")   ≈ " + Formula(z));
            Console.WriteLine(" ↳ recursive(" + z + ") ≈ " + Recursive(z));
            Console.WriteLine(" ↳ iterative(" + z + ") ≈ " + Iterative(z));
        }
    }
}
//https://gemini.google.com/share/132fd9c3167a