# WordQuestionsParser

[![.NET](https://img.shields.io/badge/.NET-10-blue?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)

Консольное приложение на C# (.NET 10), которое извлекает вопросы и варианты ответов из Word-файла (.docx) и сохраняет в JSON.

## Что делает
- Парсит ~300 вопросов из документа по теории цепей (Dövrlər nəzəriyyəsi)
- Определяет вопросы по знаку "?"
- Автоматически присваивает варианты A/B/C...
- Сохраняет в quiz_adapted.json

## Как использовать
1. Скачай input.docx и положи в C:\Temp\
2. Запусти: dotnet run -- "C:/Temp/input.docx" "C:/Temp/"
3. Результат: C:/Temp/quiz_adapted.json с ~300 вопросами

## Пример результата (quiz_adapted.json)

Вот как выглядит фрагмент сгенерированного JSON (первые вопросы):

![Пример JSON с вопросами](example-quiz-json.png)
