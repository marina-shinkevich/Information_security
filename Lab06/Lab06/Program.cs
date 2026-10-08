using Lab06;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using Lab06;


try
{
    static Dictionary<char, int> GetCharacterFrequency(char[] text)
    {
        var frequency = new Dictionary<char, int>();

        foreach (char c in text)
        {
            if (char.IsLetter(c)) // Можно проверять, только если это буква
            {
                if (frequency.ContainsKey(c))
                {
                    frequency[c]++;
                }
                else
                {
                    frequency[c] = 1;
                }
            }
        }

        return frequency;
    }

    var fileNameEncrypt = "encrypt.txt";
    var fileNameDecrypt = "decrypt.txt";
    var frequencyEncryptCsv = "frequency_encrypt.csv";
    var frequencyDecryptCsv = "frequency_decrypt.csv";

    var enigmaEncrypt = new Enigma(0, 0, 0);
    var enigmaDecrypt = new Enigma(0, 0, 0);
    var openMessage = EnigmaHelper.GetOpenText();

    Stopwatch stopwatchEncrypt = Stopwatch.StartNew();
    var encryptedMessage = enigmaEncrypt.Encrypt(openMessage);
    stopwatchEncrypt.Stop();
    Console.WriteLine($"Зашифровано: {EnigmaHelper.WriteToFile(encryptedMessage, fileNameEncrypt)}");
    Console.WriteLine($"Время зашифрования: {stopwatchEncrypt.ElapsedMilliseconds} ms");

    // Подсчет и сохранение частоты зашифрованного текста
    var encryptFrequency = GetCharacterFrequency(encryptedMessage);
    SaveFrequencyToCsv(encryptFrequency, frequencyEncryptCsv);
    Console.WriteLine($"Частота символов в зашифрованном тексте сохранена в {frequencyEncryptCsv}");

    Stopwatch stopwatchDecrypt = Stopwatch.StartNew();
    var decryptedMessage = enigmaDecrypt.Decrypt(encryptedMessage);
    stopwatchDecrypt.Stop();
    Console.WriteLine($"Расшифровано: {EnigmaHelper.WriteToFile(decryptedMessage, fileNameDecrypt)}");
    Console.WriteLine($"Время расшифрования: {stopwatchDecrypt.ElapsedMilliseconds} ms");

    // Подсчет и сохранение частоты расшифрованного текста
    var decryptFrequency = GetCharacterFrequency(decryptedMessage);
    SaveFrequencyToCsv(decryptFrequency, frequencyDecryptCsv);
    Console.WriteLine($"Частота символов в расшифрованном тексте сохранена в {frequencyDecryptCsv}");

    static void SaveFrequencyToCsv(Dictionary<char, int> frequency, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Character,Frequency");
            foreach (var kv in frequency.OrderBy(x => x.Key))
            {
                writer.WriteLine($"{kv.Key},{kv.Value}");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] {ex.Message}");
}

