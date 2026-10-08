using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab06
{
    public class EnigmaHelper
    {
        const string pathToFolder = "../../../Texts/";
        const string fileNameOpen = "input.txt";


        // Посчитать кол-во появлений символов в тексте
        public static Dictionary<char, int> GetSymbolAppearances(char[] str)
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


        // Чтение текста из файла
        public static char[] ReadFromFile(string fileName)
        {
            var filePath = Path.Combine(pathToFolder, fileName);
            var text = "";
            using (var sr = new StreamReader(filePath))
                text = sr.ReadToEnd();
            return text.ToLower().ToCharArray();
        }


        // Запись текста в файл
        public static bool WriteToFile(char[] text, string fileName)
        {
            var filePath = Path.Combine(pathToFolder, fileName);
            try
            {
                using (var sw = new StreamWriter(filePath))
                    sw.WriteLine(text);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        // Получить массив char[] с исходным текстом
        public static char[] GetOpenText() => ReadFromFile(fileNameOpen);
    }
}
