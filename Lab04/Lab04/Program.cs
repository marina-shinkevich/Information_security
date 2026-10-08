using System;
using System.IO;
using System.Text;
using System.Diagnostics; 
using System.Collections.Generic;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart; 

class CryptoApp
{
    const string alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
    const int m = 33;

    public static string AffineEncrypt(string input, int a, int b)
    {
        StringBuilder encrypted = new StringBuilder();

        foreach (char c in input)
        {
            int index = alphabet.IndexOf(char.ToUpper(c));
            if (index != -1)
            {
                int encryptedIndex = (a * index + b) % m;
                encrypted.Append(alphabet[encryptedIndex]);
            }
            else
            {
                encrypted.Append(c);
            }
        }

        return encrypted.ToString();
    }

    public static string AffineDecrypt(string input, int a, int b)
    {
        StringBuilder decrypted = new StringBuilder();
        int aInverse = ModInverse(a, m);

        foreach (char c in input)
        {
            int index = alphabet.IndexOf(c);
            if (index != -1)
            {
                int decryptedIndex = (aInverse * (index - b + m)) % m;
                decrypted.Append(alphabet[decryptedIndex]);
            }
            else
            {
                decrypted.Append(c);
            }
        }

        return decrypted.ToString();
    }

    public static string VigenereEncrypt(string input, string key)
    {
        StringBuilder encrypted = new StringBuilder();
        key = key.ToUpper();
        int keyLength = key.Length;

        for (int i = 0, j = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (alphabet.Contains(char.ToUpper(c)))
            {
                int cIndex = alphabet.IndexOf(char.ToUpper(c));
                int kIndex = alphabet.IndexOf(key[j % keyLength]);
                int encryptedIndex = (cIndex + kIndex) % m;
                encrypted.Append(alphabet[encryptedIndex]);
                j++;
            }
            else
            {
                encrypted.Append(c);
            }
        }

        return encrypted.ToString();
    }

    public static string VigenereDecrypt(string input, string key)
    {
        StringBuilder decrypted = new StringBuilder();
        key = key.ToUpper();
        int keyLength = key.Length;

        for (int i = 0, j = 0; i < input.Length; i++)
        {
            char c = input[i];
            if (alphabet.Contains(c))
            {
                int cIndex = alphabet.IndexOf(c);
                int kIndex = alphabet.IndexOf(key[j % keyLength]);
                int decryptedIndex = (cIndex - kIndex + m) % m;
                decrypted.Append(alphabet[decryptedIndex]);
                j++;
            }
            else
            {
                decrypted.Append(c);
            }
        }

        return decrypted.ToString();
    }

    private static int ModInverse(int a, int m)
    {
        a = a % m;
        for (int x = 1; x < m; x++)
        {
            if ((a * x) % m == 1)
                return x;
        }
        return 1;
    }

    private static Dictionary<char, int> GetFrequency(string input)
    {
        Dictionary<char, int> frequency = new Dictionary<char, int>();

        foreach (char c in input)
        {
            if (alphabet.Contains(char.ToUpper(c)))
            {
                char upperChar = char.ToUpper(c);
                if (frequency.ContainsKey(upperChar))
                {
                    frequency[upperChar]++;
                }
                else
                {
                    frequency[upperChar] = 1;
                }
            }
        }

        return frequency;
    }

    /*private static void SaveHistogramToExcel(Dictionary<char, int> frequency, string title)
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add(title);

            worksheet.Cells[1, 1].Value = "Символ";
            worksheet.Cells[1, 2].Value = "Частота";

            int row = 2;
            foreach (var pair in frequency)
            {
                worksheet.Cells[row, 1].Value = pair.Key;
                worksheet.Cells[row, 2].Value = pair.Value;
                row++;
            }

            // Создание гистограммы
            var chart = worksheet.Drawings.AddChart("FrequencyChart", eChartType.ColumnClustered);
            chart.Title.Text = "Гистограмма частот";
            chart.SetPosition(1, 0, 3, 0); // Позиция на листе
            chart.SetSize(600, 400); // Размер графика

            // Создание серии
            var series = chart.Series.Add(worksheet.Cells[2, 2, row - 1, 2], worksheet.Cells[2, 1, row - 1, 1]);

            // Установка XSeries используя диапазон
            series.XSeries = worksheet.Cells[2, 1, row - 1, 1]; // Подписи по оси X

            var fileName = $"{title}.xlsx";
            File.WriteAllBytes(fileName, package.GetAsByteArray());
            Console.WriteLine($"Гистограмма сохранена в {fileName}");
        }
    }*/

    static void Main(string[] args)
    {
        string filePath = @"D:\3курс6сем\ИБ\лабы+отчеты\Lab04\text.txt";
        string surname = "Шинкевич"; 

        string text = File.ReadAllText(filePath);

        Stopwatch watch = Stopwatch.StartNew();
        string affineEncrypted = AffineEncrypt(text, 7, 10);
        watch.Stop();
        Console.WriteLine($"Время шифрования аффинным шифром: {watch.ElapsedMilliseconds} мс");

        watch.Restart();
        string affineDecrypted = AffineDecrypt(affineEncrypted, 7, 10);
        watch.Stop();
        Console.WriteLine($"Время расшифрования аффинным шифром: {watch.ElapsedMilliseconds} мс");

        watch.Restart();
        string vigenereEncrypted = VigenereEncrypt(text, surname);
        watch.Stop();
        Console.WriteLine($"Время шифрования шифром Виженера: {watch.ElapsedMilliseconds} мс");

        watch.Restart();
        string vigenereDecrypted = VigenereDecrypt(vigenereEncrypted, surname);
        watch.Stop();
        Console.WriteLine($"Время расшифрования шифром Виженера: {watch.ElapsedMilliseconds} мс");

        File.WriteAllText("affine_encrypted.txt", affineEncrypted);
        File.WriteAllText("affine_decrypted.txt", affineDecrypted);
        File.WriteAllText("vigenere_encrypted.txt", vigenereEncrypted);
        File.WriteAllText("vigenere_decrypted.txt", vigenereDecrypted);

        var originalFrequency = GetFrequency(text);
        var affineFrequency = GetFrequency(affineEncrypted);
        var vigenereFrequency = GetFrequency(vigenereEncrypted);

       /* // Сохранение гистограмм в Excel
        SaveHistogramToExcel(originalFrequency, "Гистограмма частот исходного текста");
        SaveHistogramToExcel(affineFrequency, "Гистограмма частот зашифрованного текста (Аффинный шифр)");
        SaveHistogramToExcel(vigenereFrequency, "Гистограмма частот зашифрованного текста (Шифр Виженера)");*/

        Console.WriteLine("Результаты шифрования и расшифрования записаны в файлы.");
    }
}