using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    private const string DefaultOutputFileName = "quiz_adapted.json";

    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Использование: program.exe <путь_к_word_или_папке> <папка_выхода>");
            Console.WriteLine("Пример (файл): program.exe \"C:\\Temp\\input.docx\" \"C:\\Temp\\\"");
            Console.WriteLine("Пример (папка): program.exe \"C:\\Temp\\Docs\" \"C:\\Temp\\Output\"");
            return;
        }

        string inputPath = args[0];
        string outputFolder = args[1];

        try
        {
            Directory.CreateDirectory(outputFolder);
            var inputFiles = GetInputFiles(inputPath);
            if (inputFiles.Count == 0)
            {
                Console.WriteLine("Подходящих .docx файлов не найдено.");
                return;
            }

            foreach (var filePath in inputFiles)
            {
                var questions = ExtractQuestions(filePath);
                string json = JsonConvert.SerializeObject(questions, Formatting.Indented);

                string outputPath = BuildOutputPath(outputFolder, filePath, inputFiles.Count);
                File.WriteAllText(outputPath, json);

                Console.WriteLine($"Готово! Файл сохранён: {outputPath}");
                Console.WriteLine($"Найдено вопросов: {questions.Count}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка:");
            Console.WriteLine(ex.Message);
        }
    }

    static List<string> GetInputFiles(string inputPath)
    {
        if (File.Exists(inputPath))
        {
            return new List<string> { inputPath };
        }

        if (Directory.Exists(inputPath))
        {
            return Directory.GetFiles(inputPath, "*.docx").OrderBy(path => path).ToList();
        }

        Console.WriteLine($"Файл или папка не найдены: {inputPath}");
        return new List<string>();
    }

    static string BuildOutputPath(string outputFolder, string inputFilePath, int totalInputs)
    {
        if (totalInputs == 1)
        {
            return Path.Combine(outputFolder, DefaultOutputFileName);
        }

        string baseName = Path.GetFileNameWithoutExtension(inputFilePath);
        return Path.Combine(outputFolder, $"{baseName}_quiz.json");
    }

    static List<QuizQuestion> ExtractQuestions(string filePath)
    {
        var questions = new List<QuizQuestion>();
        QuizQuestion? current = null;

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, false))
        {
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body == null) return questions;

            foreach (var para in body.Descendants<Paragraph>())
            {
                string text = (para.InnerText ?? "").Trim();
                if (string.IsNullOrWhiteSpace(text)) continue;

                // Вопросы чаще всего заканчиваются на "?" или начинаются с номера.
                if (IsQuestion(text))
                {
                    if (current != null && current.options.Count > 0)
                        questions.Add(current);

                    current = new QuizQuestion
                    {
                        q = text,
                        options = new Dictionary<string, string>(),
                        correct = null
                    };
                    continue;
                }

                // Если есть текущий вопрос — добавляем как вариант
                if (current != null)
                {
                    // Проверяем на формат "A) текст", "A. текст", "A - текст"
                    var optionMatch = Regex.Match(text, @"^([A-J])\s*[\)\.\-]\s*(.+)$", RegexOptions.IgnoreCase);
                    string key;
                    string content = text;

                    if (optionMatch.Success)
                    {
                        key = optionMatch.Groups[1].Value.ToUpper();
                        content = optionMatch.Groups[2].Value.Trim();
                    }
                    else
                    {
                        // Авто-буква, если без префикса (как в начале)
                        key = ((char)('A' + current.options.Count)).ToString();
                    }

                    current.options[key] = content;

                    // Проверка на bold (жирный шрифт) для correct
                    bool isBold = para.Descendants<Run>().Any(r => r.RunProperties?.Bold != null);
                    if (isBold)
                    {
                        current.correct = key;
                    }
                }
            }

            // Добавляем последний вопрос
            if (current != null && current.options.Count > 0)
                questions.Add(current);
        }

        return questions;
    }

    static bool IsQuestion(string text)
    {
        if (text.EndsWith("?"))
        {
            return true;
        }

        return Regex.IsMatch(text, @"^\s*\d+[\)\.]\s+");
    }
}

public class QuizQuestion
{
    public string q { get; set; } = "";
    public Dictionary<string, string> options { get; set; } = new();
    public string? correct { get; set; }
}
