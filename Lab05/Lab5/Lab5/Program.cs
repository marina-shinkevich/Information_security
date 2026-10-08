using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OfficeOpenXml;

class Program
{
    static void Main()
    {
        string inputFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\input.txt";
        string encryptedFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\encrypted_multi.txt";
        string decryptedFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\decrypted_multi.txt";
        string histogramDECFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\decrypted_his_multi.xlsx";
        string histogramENFilePath = "D:\\3курс6сем\\ИБ\\лабы+отчеты\\Lab05\\en_histogram_multi.xlsx";
        string key1 = "Марина";
        string key2 = "Шинкевич";
        Stopwatch sw = Stopwatch.StartNew();
        string text = File.ReadAllText(inputFilePath).PadRight(5000);

        string encryptedText = Encrypt(text, key1, key2);
        sw.Stop();
        File.WriteAllText(encryptedFilePath, encryptedText);
        Console.WriteLine($"Время шифрования: {sw.ElapsedMilliseconds} мс");
        GenerateHistogram(encryptedText, histogramENFilePath);
        Console.WriteLine($"Гистограмма зашифрованного текста сохранена в {histogramENFilePath}");
        sw.Restart();

        string decryptedText = Decrypt(encryptedText, key1, key2);
        sw.Stop();
        File.WriteAllText(decryptedFilePath, decryptedText);
        TimeSpan ts = sw.Elapsed;
        Console.WriteLine($"Время расшифрования: {ts.TotalMilliseconds} мс");
        GenerateHistogram(decryptedText, histogramDECFilePath);
        Console.WriteLine($"Гистограмма зашифрованного текста сохранена в {histogramDECFilePath}");

    }

    static string Encrypt(string text, string key1, string key2)
    {
        int cols = 10;
        int rows = 50;
        while (key1.Length < cols) key1 += key1;
        key1 = key1.Substring(0, cols);

        while (key2.Length < rows) key2 += key2;
        key2 = key2.Substring(0, rows);

        char[,] table = new char[rows, cols];
        int index = 0;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                table[r, c] = index < text.Length ? text[index++] : ' ';

        var sortedCols = key1.Select((ch, i) => (ch, i)).OrderBy(x => x.ch).Select(x => x.i).ToArray();
        var sortedRows = key2.Select((ch, i) => (ch, i)).OrderBy(x => x.ch).Select(x => x.i).ToArray();

        char[] encryptedText = new char[rows * cols];
        index = 0;
        foreach (int r in sortedRows)
            foreach (int c in sortedCols)
                encryptedText[index++] = table[r, c];

        return new string(encryptedText);
    }

    static string Decrypt(string text, string key1, string key2)
    {
        int cols = 10;
        int rows = 12;
        while (key1.Length < cols) key1 += key1;
        key1 = key1.Substring(0, cols);

        while (key2.Length < rows) key2 += key2;
        key2 = key2.Substring(0, rows);

        char[,] table = new char[rows, cols];

        var sortedCols = key1.Select((ch, i) => (ch, i)).OrderBy(x => x.ch).Select(x => x.i).ToArray();
        var sortedRows = key2.Select((ch, i) => (ch, i)).OrderBy(x => x.ch).Select(x => x.i).ToArray();
        int index = 0;
        foreach (int r in sortedRows)
            foreach (int c in sortedCols)
                table[r, c] = index < text.Length ? text[index++] : ' ';
        char[] decryptedText = new char[rows * cols];
        index = 0;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                decryptedText[index++] = table[r, c];

        return new string(decryptedText).Trim();
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