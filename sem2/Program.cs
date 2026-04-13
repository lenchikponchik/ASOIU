using System;

// ============================================================
// Структура файла:
// 1. Тестовые матрицы
// 2. Задание 1 — COO
// 3. Задание 2 — LIL
// 4. Задание 3 — CSR
// 5. Вспомогательные статические функции
// 6. Статические функции COO
// 7. Статические функции LIL
// 8. Статические функции CSR
// ============================================================

// ============================================================
// ТЕСТОВЫЕ МАТРИЦЫ
// ============================================================

int[,] matrixCOO =
{
    { 1, 0, 0, 0, 0, 1 },
    { 0, 2, 0, 0, 0, 0 },
    { 3, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 4 },
    { 0, 0, 5, 0, 0, 0 }
};

int[,] matrixLIL =
{
    { 0, 1, 0, 2, 0, 0 },
    { 0, 0, 0, 0, 0, 3 },
    { 4, 0, 5, 0, 0, 0 },
    { 0, 0, 0, 6, 0, 0 }
};

int[,] matrixCSR =
{
    { 8, 0, 2, 0, 0 },
    { 0, 0, 5, 0, 0 },
    { 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0 },
    { 0, 0, 7, 1, 2 },
    { 0, 0, 0, 0, 0 },
    { 0, 0, 0, 9, 0 }
};

// ============================================================
// ЗАДАНИЕ 1: COO
// ============================================================

PrintSeparator("ЗАДАНИЕ 1: COO - КООРДИНАТНЫЙ ФОРМАТ ХРАНЕНИЯ");

int rCOO = matrixCOO.GetLength(0);
int cCOO = matrixCOO.GetLength(1);
int nCOO = CountNonZero(matrixCOO);

Console.WriteLine("Исходная плотная матрица (Рисунок 1, 5x6):");
PrintMatrix(matrixCOO);

Console.WriteLine("\n--- Анализ эффективности COO ---");
Console.WriteLine($"Размерность матрицы      : {rCOO} x {cCOO} = {rCOO * cCOO} ячеек");
Console.WriteLine($"Ненулевых элементов (N)  : {nCOO}");
Console.WriteLine($"COO хранение (N * 3)     : {nCOO * 3} ячеек");
Console.WriteLine($"Порог эффективности      : {(double)(rCOO * cCOO) / 3:F2}");
Console.WriteLine($"isCOOEffective           : {isCOOEffective(matrixCOO)}");

Console.WriteLine("\n--- Преобразование Dense -> COO ---");
DenseToCOO(matrixCOO, out int[] rowCOO, out int[] colCOO, out int[] dataCOO);
PrintArray("Row   ", rowCOO);
PrintArray("Column", colCOO);
PrintArray("Data  ", dataCOO);

Console.WriteLine("\n--- Преобразование COO -> Dense ---");
int[,] restoredCOO = COOToDense(rowCOO, colCOO, dataCOO, rCOO, cCOO);
PrintMatrix(restoredCOO);
Console.WriteLine($"Матрица восстановлена верно: {MatricesEqual(matrixCOO, restoredCOO)}");

// ============================================================
// ЗАДАНИЕ 2: LIL
// ============================================================

PrintSeparator("ЗАДАНИЕ 2: LIL - ХРАНЕНИЕ В ФОРМЕ СВЯЗНЫХ СПИСКОВ");

int rLIL = matrixLIL.GetLength(0);
int cLIL = matrixLIL.GetLength(1);
int nLIL = CountNonZero(matrixLIL);

Console.WriteLine("Исходная плотная матрица (Рисунок 2, 4x6):");
PrintMatrix(matrixLIL);

Console.WriteLine("\n--- Анализ эффективности LIL ---");
Console.WriteLine($"Размерность матрицы      : {rLIL} x {cLIL} = {rLIL * cLIL} ячеек");
Console.WriteLine($"Ненулевых элементов (N)  : {nLIL}");
Console.WriteLine($"LIL хранение (N * 2)     : {nLIL * 2} ячеек");
Console.WriteLine($"Порог эффективности      : {(double)(rLIL * cLIL) / 2:F2}");
Console.WriteLine($"isLILEffective           : {isLILEffective(matrixLIL)}");

Console.WriteLine("\n--- Преобразование Dense -> LIL ---");
DenseToLIL(matrixLIL, out int[][] rowsLIL, out int[][] dataLIL);
PrintJaggedArray("Rows", rowsLIL);
PrintJaggedArray("Data", dataLIL);

Console.WriteLine("\n--- Преобразование LIL -> Dense ---");
int[,] restoredLIL = LILToDense(rowsLIL, dataLIL, rLIL, cLIL);
PrintMatrix(restoredLIL);
Console.WriteLine($"Матрица восстановлена верно: {MatricesEqual(matrixLIL, restoredLIL)}");

// ============================================================
// ЗАДАНИЕ 3: CSR
// ============================================================

PrintSeparator("ЗАДАНИЕ 3: CSR - COMPRESSED SPARSE ROW");

int rCSR = matrixCSR.GetLength(0);
int cCSR = matrixCSR.GetLength(1);
int nCSR = CountNonZero(matrixCSR);

Console.WriteLine("Исходная плотная матрица (Рисунок 3, 7x5):");
PrintMatrix(matrixCSR);

Console.WriteLine("\n--- Анализ эффективности CSR ---");
Console.WriteLine($"Размерность матрицы      : {rCSR} x {cCSR} = {rCSR * cCSR} ячеек");
Console.WriteLine($"Ненулевых элементов (N)  : {nCSR}");
Console.WriteLine($"CSR хранение (2N + R + 1): {2 * nCSR + rCSR + 1} ячеек");
Console.WriteLine($"Порог эффективности      : N < (R * C - R - 1) / 2");
Console.WriteLine($"isCSREffective           : {isCSREffective(matrixCSR)}");

Console.WriteLine("\n--- Преобразование Dense -> CSR ---");
DenseToCSR(matrixCSR, out int[] dataCSR, out int[] indicesCSR, out int[] indexPointersCSR);
PrintArray("Data         ", dataCSR);
PrintArray("Indices      ", indicesCSR);
PrintArray("IndexPointers", indexPointersCSR);

Console.WriteLine("\n--- Преобразование CSR -> Dense ---");
int[,] restoredCSR = CSRToDense(dataCSR, indicesCSR, indexPointersCSR, rCSR, cCSR);
PrintMatrix(restoredCSR);
Console.WriteLine($"Матрица восстановлена верно: {MatricesEqual(matrixCSR, restoredCSR)}");

// ============================================================
// ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
// ============================================================

static int CountNonZero(int[,] matrix)
{
    int count = 0;

    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if (matrix[i, j] != 0)
            {
                count++;
            }
        }
    }

    return count;
}

static void PrintSeparator(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', 70));
}

static void PrintMatrix(int[,] matrix)
{
    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            Console.Write($"{matrix[i, j],4}");
        }

        Console.WriteLine();
    }
}

static void PrintArray(string name, int[] array)
{
    Console.WriteLine($"{name}: [ {string.Join(", ", array)} ]");
}

static void PrintJaggedArray(string name, int[][] array)
{
    Console.WriteLine($"{name}:");

    for (int i = 0; i < array.Length; i++)
    {
        Console.WriteLine($"  {name}[{i}] = [ {string.Join(", ", array[i])} ]");
    }
}

static bool MatricesEqual(int[,] left, int[,] right)
{
    if (left.GetLength(0) != right.GetLength(0) || left.GetLength(1) != right.GetLength(1))
    {
        return false;
    }

    for (int i = 0; i < left.GetLength(0); i++)
    {
        for (int j = 0; j < left.GetLength(1); j++)
        {
            if (left[i, j] != right[i, j])
            {
                return false;
            }
        }
    }

    return true;
}

// ============================================================
// COO
// ============================================================

static void DenseToCOO(int[,] matrix, out int[] row, out int[] column, out int[] data)
{
    int count = CountNonZero(matrix);
    row = new int[count];
    column = new int[count];
    data = new int[count];

    int index = 0;
    for (int i = 0; i < matrix.GetLength(0); i++)
    {
        for (int j = 0; j < matrix.GetLength(1); j++)
        {
            if (matrix[i, j] != 0)
            {
                row[index] = i;
                column[index] = j;
                data[index] = matrix[i, j];
                index++;
            }
        }
    }
}

static int[,] COOToDense(int[] row, int[] column, int[] data, int rowCount, int columnCount)
{
    int[,] matrix = new int[rowCount, columnCount];

    for (int i = 0; i < data.Length; i++)
    {
        matrix[row[i], column[i]] = data[i];
    }

    return matrix;
}

static bool isCOOEffective(int[,] matrix)
{
    int nonZero = CountNonZero(matrix);
    int denseCells = matrix.GetLength(0) * matrix.GetLength(1);
    return nonZero * 3 < denseCells;
}

// ============================================================
// LIL
// ============================================================

static void DenseToLIL(int[,] matrix, out int[][] rows, out int[][] data)
{
    int rowCount = matrix.GetLength(0);
    int columnCount = matrix.GetLength(1);

    rows = new int[rowCount][];
    data = new int[rowCount][];

    for (int i = 0; i < rowCount; i++)
    {
        int nonZeroInRow = 0;
        for (int j = 0; j < columnCount; j++)
        {
            if (matrix[i, j] != 0)
            {
                nonZeroInRow++;
            }
        }

        rows[i] = new int[nonZeroInRow];
        data[i] = new int[nonZeroInRow];

        int index = 0;
        for (int j = 0; j < columnCount; j++)
        {
            if (matrix[i, j] != 0)
            {
                rows[i][index] = j;
                data[i][index] = matrix[i, j];
                index++;
            }
        }
    }
}

static int[,] LILToDense(int[][] rows, int[][] data, int rowCount, int columnCount)
{
    int[,] matrix = new int[rowCount, columnCount];

    for (int i = 0; i < rows.Length; i++)
    {
        for (int j = 0; j < rows[i].Length; j++)
        {
            matrix[i, rows[i][j]] = data[i][j];
        }
    }

    return matrix;
}

static bool isLILEffective(int[,] matrix)
{
    int nonZero = CountNonZero(matrix);
    int denseCells = matrix.GetLength(0) * matrix.GetLength(1);
    return nonZero * 2 < denseCells;
}

// ============================================================
// CSR
// ============================================================

static void DenseToCSR(int[,] matrix, out int[] data, out int[] indices, out int[] indexPointers)
{
    int rowCount = matrix.GetLength(0);
    int columnCount = matrix.GetLength(1);
    int nonZero = CountNonZero(matrix);

    data = new int[nonZero];
    indices = new int[nonZero];
    indexPointers = new int[rowCount + 1];

    int currentIndex = 0;
    indexPointers[0] = 0;

    for (int i = 0; i < rowCount; i++)
    {
        for (int j = 0; j < columnCount; j++)
        {
            if (matrix[i, j] != 0)
            {
                data[currentIndex] = matrix[i, j];
                indices[currentIndex] = j;
                currentIndex++;
            }
        }

        indexPointers[i + 1] = currentIndex;
    }
}

static int[,] CSRToDense(int[] data, int[] indices, int[] indexPointers, int rowCount, int columnCount)
{
    int[,] matrix = new int[rowCount, columnCount];

    for (int row = 0; row < rowCount; row++)
    {
        int start = indexPointers[row];
        int end = indexPointers[row + 1];

        for (int i = start; i < end; i++)
        {
            matrix[row, indices[i]] = data[i];
        }
    }

    return matrix;
}

static bool isCSREffective(int[,] matrix)
{
    int rowCount = matrix.GetLength(0);
    int denseCells = rowCount * matrix.GetLength(1);
    int nonZero = CountNonZero(matrix);
    return 2 * nonZero + rowCount + 1 < denseCells;
}
