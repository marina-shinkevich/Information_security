using System;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;


class SpiralTranspositionCipher
{
    static void Main()
    {
        string inputFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\input.txt";
        string encryptedFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\encrypted.txt";
        string decryptedFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\decrypted.txt";
        string histogramDECFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\decrypted_his.xlsx";
        string histogramENFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\en_histogram.xlsx";


        string text = File.ReadAllText(inputFilePath);

        Stopwatch sw = Stopwatch.StartNew();
        string encryptedText = Encrypt(text);
        sw.Stop();
        File.WriteAllText(encryptedFilePath, encryptedText);
        Console.WriteLine($"Время шифрования: {sw.ElapsedMilliseconds} мс");
        //GenerateHistogram(encryptedText, histogramENFilePath);
        //Console.WriteLine($"Гистограмма зашифрованного текста сохранена в {histogramENFilePath}");


        sw.Restart();
        string decryptedText = Decrypt(encryptedText);
        sw.Stop();
        TimeSpan ts = sw.Elapsed;

        File.WriteAllText(decryptedFilePath, decryptedText);
        Console.WriteLine($"Время расшифрования: {ts.TotalMilliseconds} мс");
        //GenerateHistogram(decryptedText, histogramDECFilePath);
        //Console.WriteLine($"Гистограмма зашифрованного текста сохранена в {histogramDECFilePath}");

    }

    static string Encrypt(string input)
    {
        int size = (int)Math.Ceiling(Math.Sqrt(input.Length));
        char[,] grid = new char[size, size];
        int index = 0;

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                grid[i, j] = index < input.Length ? input[index++] : ' ';

        return TraverseSpiral(grid, size);
    }

    static string Decrypt(string input)
    {
        int size = (int)Math.Ceiling(Math.Sqrt(input.Length));
        char[,] grid = new char[size, size];
        FillSpiral(grid, input, size);

        string result = "";
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                result += grid[i, j];

        return result.Trim();
    }

    static string TraverseSpiral(char[,] grid, int size)
    {
        int left = 0, right = size - 1, top = 0, bottom = size - 1;
        string result = "";
        while (left <= right && top <= bottom)
        {
            for (int j = left; j <= right; j++) result += grid[top, j];
            top++;
            for (int i = top; i <= bottom; i++) result += grid[i, right];
            right--;
            for (int j = right; j >= left; j--) result += grid[bottom, j];
            bottom--;
            for (int i = bottom; i >= top; i--) result += grid[i, left];
            left++;
        }
        return result;
    }

    static void FillSpiral(char[,] grid, string input, int size)
    {
        int left = 0, right = size - 1, top = 0, bottom = size - 1, index = 0;
        while (left <= right && top <= bottom)
        {
            for (int j = left; j <= right; j++) grid[top, j] = input[index++];
            top++;
            for (int i = top; i <= bottom; i++) grid[i, right] = input[index++];
            right--;
            for (int j = right; j >= left; j--) grid[bottom, j] = input[index++];
            bottom--;
            for (int i = bottom; i >= top; i--) grid[i, left] = input[index++];
            left++;
        }
    }
    static void GenerateHistogram(string text, string fileName)
    {
        Dictionary<char, int> frequency = new Dictionary<char, int>();
        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                if (frequency.ContainsKey(c))
                    frequency[c]++;
                else
                    frequency[c] = 1;
            }
        }

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Histogram");
            worksheet.Cells[1, 1].Value = "Symbol";
            worksheet.Cells[1, 2].Value = "Frequency";

            int row = 2;
            foreach (var kvp in frequency)
            {
                worksheet.Cells[row, 1].Value = kvp.Key;
                worksheet.Cells[row, 2].Value = kvp.Value;
                row++;
            }

            package.SaveAs(new FileInfo(fileName));
        }
    }


}
