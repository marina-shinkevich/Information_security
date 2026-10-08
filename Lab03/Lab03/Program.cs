using System;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\nВыберите операцию:");
            Console.WriteLine("1. Вычислить НОД");
            Console.WriteLine("2. Найти простые числа в интервале");
            Console.WriteLine("3. Подсчитать количество простых чисел в интервале");
            Console.WriteLine("4. Выход");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    CalculateGCD();
                    break;
                case "2":
                    FindPrimeNumbersInRange();
                    break;
                case "3":
                    CountPrimeNumbersInRange();
                    break;
                case "4":
                    Console.WriteLine("Выход из программы.");
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                    break;
            }
        }
    }

    static void CalculateGCD()
    {
        Console.WriteLine("Введите количество чисел (2 или 3):");
        int count = int.Parse(Console.ReadLine());
        int[] numbers = new int[count];

        for (int i = 0; i < count; i++)
        {
            Console.Write($"Введите число {i + 1}: ");
            numbers[i] = int.Parse(Console.ReadLine());
        }

        int gcd = numbers[0];
        for (int i = 1; i < count; i++)
        {
            gcd = GCD(gcd, numbers[i]);
        }

        Console.WriteLine($"НОД чисел {string.Join(", ", numbers)} = {gcd}");
    }

    static int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static void FindPrimeNumbersInRange()
    {
        Console.WriteLine("Введите начало интервала (m):");
        int m = int.Parse(Console.ReadLine());
        Console.WriteLine("Введите конец интервала (n):");
        int n = int.Parse(Console.ReadLine());

        Console.WriteLine($"Простые числа в интервале [{m}, {n}]:");
        for (int i = m; i <= n; i++)
        {
            if (IsPrime(i))
            {
                Console.Write(i + " ");
            }
        }
        Console.WriteLine();
    }

    static void CountPrimeNumbersInRange()
    {
        Console.WriteLine("Введите начало интервала (m):");
        int m = int.Parse(Console.ReadLine());
        Console.WriteLine("Введите конец интервала (n):");
        int n = int.Parse(Console.ReadLine());
        int count = 0;

        for (int i = m; i <= n; i++)
        {
            if (IsPrime(i))
            {
                count++;
            }
        }

        Console.WriteLine($"Количество простых чисел в интервале [{m}, {n}]: {count}");
    }

    static bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }
}