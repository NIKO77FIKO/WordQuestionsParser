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
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Использование: program.exe <путь_к_word> <папка_выхода>");
            Console.WriteLine("Пример: program.exe \"C:\\Temp\\input.docx\" \"C:\\Temp\\\"");
            return;
        }

        string inputDocx = args[0];
        string outputFolder = args[1];

        if (!File.Exists(inputDocx))
        {
            Console.WriteLine($"Файл не найден: {inputDocx}");
            return;
        }

        Directory.CreateDirectory(outputFolder);

        try
        {
            var questions = ExtractQuestions(inputDocx);
            string json = JsonConvert.SerializeObject(questions, Formatting.Indented);

            string outputPath = Path.Combine(outputFolder, "quiz_adapted.json");
            File.WriteAllText(outputPath, json);

            Console.WriteLine($"Готово! Файл сохранён: {outputPath}");
            Console.WriteLine($"Найдено вопросов: {questions.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка:");
            Console.WriteLine(ex.Message);
        }
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

                // Если заканчивается "?" — это вопрос (все 300 в твоём файле такие)
                if (text.EndsWith("?"))
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
                    // Проверяем на формат "A) текст" (как в конце документа)
                    var optionMatch = Regex.Match(text, @"^([A-E])\)\s*(.+)$", RegexOptions.IgnoreCase);
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
}

public class QuizQuestion
{
    public string q { get; set; } = "";
    public Dictionary<string, string> options { get; set; } = new();
    public string? correct { get; set; }
}