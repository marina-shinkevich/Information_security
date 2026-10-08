using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace lab02
{
    public class lab02
    {
        static void Main()
        {
            string curdir = @"D:\3курс6сем\ИБ\лабы+отчеты\Lab02\";
            Directory.SetCurrentDirectory(curdir);
            string filePath = "lat.txt";
            string filePath2 = "kir.txt";
            string filePath3 = "bin.txt";


            if (File.Exists(filePath) && File.Exists(filePath2) && File.Exists(filePath3))
            {
                string text = File.ReadAllText(filePath, Encoding.UTF8);
                //lat

                string text2 = File.ReadAllText(filePath2, Encoding.UTF8);
               //kir

                string text3 = File.ReadAllText(filePath3, Encoding.UTF8);
               

                // Частоты символов латиницы и кириллицы
                var latinFrequencies = GetCharacterFrequencies(text, 'a', 'z');
                var cyrillicFrequencies = GetCharacterFrequencies(text2, 'а', 'я');
                var binFrequencies = GetCharacterFrequencies(text3, '0', '1');

                // Excel файл
                var filePathExcel = "Frequencies.xlsx";

                if (File.Exists(filePathExcel))
                {
                    File.Delete(filePathExcel);
                }

                var fileInfo = new FileInfo("Frequencies.xlsx");
                using (var package = new ExcelPackage(fileInfo))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Frequencies");

                    WriteFrequenciesToWorksheet(worksheet, latinFrequencies, "Латиница", 1);

                    WriteFrequenciesToWorksheet(worksheet, cyrillicFrequencies, "Кириллица", 20);

                    WriteFrequenciesToWorksheet(worksheet, binFrequencies, "Бинарный", 60);
                    package.Save();
                }

                Console.WriteLine("Excel файл создан: Frequencies.xlsx");
            
                //  энтропия Шеннона
                double entropyLatin = CalculateShannonEntropy(latinFrequencies, text.Length);
                double entropyCyrillic = CalculateShannonEntropy(cyrillicFrequencies, text2.Length);
                double entropyBin = CalculateShannonEntropy(binFrequencies, text3.Length);

                Console.WriteLine($"Энтропия латиницы: {entropyLatin}");
                Console.WriteLine($"Энтропия кириллицы: {entropyCyrillic}");
                Console.WriteLine($"Энтропия бинарного алфавита: {entropyBin}");


                //  кол-во информации для ФИО
                string fullName = "Shinkevich Marina Dmitrievna";
                string fullName1 = "Шинкевич Марина Дмитриевна";
                double infoAmountLatin = fullName.Length * entropyLatin;
                double infoAmountCyrillic = fullName1.Length * entropyCyrillic;
                double ascii = fullName1.Length * 8;

                Console.WriteLine($"\nКоличество информации (латиница): {infoAmountLatin} бит");
                Console.WriteLine($"\nКоличество информации (кириллица): {infoAmountCyrillic} бит");
                Console.WriteLine($"\nКоличество информации (ASCII): {ascii} бит");


                double[] errorProbabilities = { 0.1, 0.5, 1.0 };

                foreach (double p in errorProbabilities)
                {
                    double q = 1 - p; // Вероятность корректной передачи

                    // Условная энтропия
                    double conditionalEntropy = -(p * Log2(p) + q * Log2(q));

                    // Эффективная энтропия
                    double effectiveEntropyLatin = entropyLatin - conditionalEntropy;
                    double effectiveEntropyCyrillic = entropyCyrillic - conditionalEntropy;

                    // Количество информации
                    double infoAmountLatinErr = fullName.Length * effectiveEntropyLatin;
                    double infoAmountCyrillicErr = fullName1.Length * effectiveEntropyCyrillic;

                    Console.WriteLine($"\nКоличество информации с вероятностью ошибки {p}:");
                    Console.WriteLine($"(латиница): {Math.Round(infoAmountLatinErr, 5)} бит");
                    Console.WriteLine($"(кириллица): {Math.Round(infoAmountCyrillicErr, 5)} бит");
                }
            }
         else
            {
                Console.WriteLine("\nФайл не найден.");
            }
        }

        // частоты символов в тексте для диапазона
        static Dictionary<char, int> GetCharacterFrequencies(string text, char start, char end)
        {
            var freq = new Dictionary<char, int>();
            foreach (char c in text)
            {
                if (c >= start && c <= end)
                {
                    if (!freq.ContainsKey(c))
                        freq[c] = 0;
                    freq[c]++;
                }
            }
            return freq;
        }

        
        //  количество появления символов в строке
        public static Dictionary<char, int> GetSymbolAppearances(string str)
        {
            var symbolAppearances = new Dictionary<char, int>();
            foreach (char c in str)
            {
                if (!symbolAppearances.ContainsKey(c))
                    symbolAppearances.Add(c, 1);
                else
                    symbolAppearances[c] += 1;
            }
            return symbolAppearances;
        }

        // энтропия по Шеннону
        static double CalculateShannonEntropy(Dictionary<char, int> frequencies, int total)
        {
            double entropy = 0;
            foreach (var kvp in frequencies)
            {
                double probability = (double)kvp.Value / total;
                entropy -= probability * Log2(probability);
            }
            return Math.Round(entropy, 3);
        }

        //  с учетом вероятности ошибки
        public static double GetInformationAmountWithError(string str, double p)
        {
            var informationAmount = GetShannonEntropy(str) * str.Length * GetEffectiveEntropy(p);
            return Math.Round(informationAmount, 3);
        }

        // Эффективная энтропия с учетом вероятности ошибки
        public static double GetEffectiveEntropy(double p)
        {
            var q = 1 - p;
            if (p == 0 || q == 0)
                return 1;
            return 1 - (-p * Log2(p) - q * Log2(q));
        }

        public static double Log2(double value)
        {
            return Math.Log(value) / Math.Log(2);
        }

        //  энтропия по Шеннону для строки
        public static double GetShannonEntropy(string str)
        {
            var symbolAppearances = GetSymbolAppearances(str);
            double entropy = 0;
            foreach (var item in symbolAppearances)
            {
                double probability = (double)item.Value / str.Length;
                entropy -= probability * Log2(probability);
            }
            return Math.Round(entropy, 3);
        }

        static void WriteFrequenciesToWorksheet(ExcelWorksheet worksheet, Dictionary<char, int> frequencies, string title, int startRow)
        {
            worksheet.Cells[startRow, 1].Value = title;
            worksheet.Cells[startRow, 1, startRow, 2].Merge = true;

            int row = startRow + 1;
            foreach (var kvp in frequencies)
            {
                worksheet.Cells[row, 1].Value = kvp.Key;
                worksheet.Cells[row, 2].Value = kvp.Value;
                row++;
            }

            // Создание гистограммы
            var chart = worksheet.Drawings.AddChart(title + " Chart", eChartType.ColumnClustered);
            chart.SetPosition(startRow + 1, 0, 3, 0);
            chart.SetSize(400, 250);
            chart.Series.Add(worksheet.Cells[startRow + 1, 2, row - 1, 2], worksheet.Cells[startRow + 1, 1, row - 1, 1]);
            chart.Title.Text = title + " Гистограмма";
        }
    }
}

