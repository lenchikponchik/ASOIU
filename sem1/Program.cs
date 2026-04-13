using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== АНАЛИЗ КВАЛИФИКАЦИИ ГРАН-ПРИ ===");
        Console.WriteLine();

        Console.Write("Введите количество участников: ");
        int n = int.Parse(Console.ReadLine() ?? "");
        Console.WriteLine();8

        string[] teams = new string[n];
        double[] avgSpeeds = new double[n];

        InputData(teams, avgSpeeds, n);

        Console.WriteLine("--- СТАТИСТИКА КВАЛИФИКАЦИИ ---");
        CalculateStatistics(teams, avgSpeeds, n);
        Console.WriteLine();

        Console.WriteLine("--- ИСХОДНЫЙ ПОРЯДОК ---");
        PrintTable(teams, avgSpeeds, n, false);
        Console.WriteLine();

        string[] sortedTeams = new string[n];
        double[] sortedSpeeds = new double[n];
        CopyArrays(teams, avgSpeeds, sortedTeams, sortedSpeeds, n);

        BubbleSort(sortedTeams, sortedSpeeds, n);

        Console.WriteLine("--- ИТОГОВЫЙ ПРОТОКОЛ КВАЛИФИКАЦИИ ---");
        Console.WriteLine("Алгоритм сортировки: пузырьковая сортировка по убыванию скорости");
        PrintTable(sortedTeams, sortedSpeeds, n, true);
        Console.WriteLine();

        FilterBySpeed(sortedTeams, sortedSpeeds, n);
    }

    static void InputData(string[] teams, double[] speeds, int n)
    {
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"Участник #{i + 1}");
            Console.Write("Команда: ");
            teams[i] = Console.ReadLine() ?? "";

            Console.Write("Средняя скорость (км/ч): ");
            speeds[i] = ReadDouble();

            Console.WriteLine();
        }
    }

    static void CalculateStatistics(string[] teams, double[] speeds, int n)
    {
        double sum = 0;
        for (int i = 0; i < n; i++)
        {
            sum += speeds[i];
        }

        double average = sum / n;

        double maxSpeed = speeds[0];
        double minSpeed = speeds[0];
        string fastestTeam = teams[0];
        string slowestTeam = teams[0];

        for (int i = 1; i < n; i++)
        {
            if (speeds[i] > maxSpeed)
            {
                maxSpeed = speeds[i];
                fastestTeam = teams[i];
            }

            if (speeds[i] < minSpeed)
            {
                minSpeed = speeds[i];
                slowestTeam = teams[i];
            }
        }

        Console.WriteLine($"Средняя скорость: {average:F2} км/ч");
        Console.WriteLine($"Лидер: {fastestTeam} ({maxSpeed:F2} км/ч)");
        Console.WriteLine($"Самый медленный: {slowestTeam} ({minSpeed:F2} км/ч)");
        Console.WriteLine($"Разница темпа: {maxSpeed - minSpeed:F2} км/ч");
    }

    static void PrintTable(string[] teams, double[] speeds, int n, bool showPosition)
    {
        Console.WriteLine("-----------------------------------------------");

        if (showPosition)
        {
            Console.WriteLine("| Поз. | Команда              | Скорость       |");
        }
        else
        {
            Console.WriteLine("| Команда              | Скорость (км/ч)      |");
        }

        Console.WriteLine("-----------------------------------------------");

        for (int i = 0; i < n; i++)
        {
            if (showPosition)
            {
                Console.WriteLine($"| {i + 1,4} | {teams[i],-20} | {speeds[i],13:F2} |");
            }
            else
            {
                Console.WriteLine($"| {teams[i],-20} | {speeds[i],19:F2} |");
            }
        }

        Console.WriteLine("-----------------------------------------------");
    }

    static void BubbleSort(string[] teams, double[] speeds, int n)
    {
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (speeds[j] < speeds[j + 1])
                {
                    double tempSpeed = speeds[j];
                    speeds[j] = speeds[j + 1];
                    speeds[j + 1] = tempSpeed;

                    string tempTeam = teams[j];
                    teams[j] = teams[j + 1];
                    teams[j + 1] = tempTeam;
                }
            }
        }
    }

    static void CopyArrays(string[] srcTeams, double[] srcSpeeds,
                           string[] dstTeams, double[] dstSpeeds, int n)
    {
        for (int i = 0; i < n; i++)
        {
            dstTeams[i] = srcTeams[i];
            dstSpeeds[i] = srcSpeeds[i];
        }
    }

    static void FilterBySpeed(string[] teams, double[] speeds, int n)
    {
        Console.WriteLine("--- ФИЛЬТР ПО МИНИМАЛЬНОЙ СКОРОСТИ ---");
        Console.Write("Введите минимальную скорость для отбора (км/ч): ");
        double minSpeed = ReadDouble();
        Console.WriteLine();

        Console.WriteLine($"Команды со скоростью >= {minSpeed:F2} км/ч:");

        int count = 0;
        for (int i = 0; i < n; i++)
        {
            if (speeds[i] >= minSpeed)
            {
                Console.WriteLine($"- {teams[i]} ({speeds[i]:F2} км/ч)");
                count++;
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Отобрано команд: {count}");
    }

    static double ReadDouble()
    {
        string input = Console.ReadLine() ?? "";
        input = input.Replace(',', '.');
        return double.Parse(input, CultureInfo.InvariantCulture);
    }
}
