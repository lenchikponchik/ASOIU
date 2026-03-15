using System;

class Program
{
    static int Distance(string s1, string s2)
    {
        int m = s1.Length;
        int n = s2.Length;

        int[,] d = new int[m + 1, n + 1];

        for (int i = 0; i <= m; i++)
            d[i, 0] = i;

        for (int j = 0; j <= n; j++)
            d[0, j] = j;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                int cost;

                if (s1[i - 1] == s2[j - 1])
                    cost = 0;
                else
                    cost = 1;

                int insert = d[i, j - 1] + 1;
                int delete = d[i - 1, j] + 1;
                int replace = d[i - 1, j - 1] + cost;

                int min = insert;

                if (delete < min)
                    min = delete;

                if (replace < min)
                    min = replace;

                d[i, j] = min;

                if (i > 1 && j > 1 &&
                    s1[i - 1] == s2[j - 2] &&
                    s1[i - 2] == s2[j - 1])
                {
                    int swap = d[i - 2, j - 2] + cost;

                    if (swap < d[i, j])
                        d[i, j] = swap;
                }
            }
        }

        return d[m, n];
    }

    static void Main()
    {
        while (true)
        {
            Console.Write("Введите первую строку: ");
            string a = Console.ReadLine();

            if (a == "exit")
                break;

            Console.Write("Введите вторую строку: ");
            string b = Console.ReadLine();

            int result = Distance(a, b);

            Console.WriteLine("Расстояние: " + result);
            Console.WriteLine();
        }
    }
}