# WordQuestionsParser

[![.NET](https://img.shields.io/badge/.NET-10-blue?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)

Word faylından (.docx) sualları və cavab variantlarını çıxararaq JSON formatında saxlayan C# konsol tətbiqi.

## Nə edir
- Dövrlər nəzəriyyəsi fənni üzrə ~300 sualı avtomatik ayırd edir
- Sualları "?" işarəsi ilə müəyyənləşdirir
- Cavab variantlarını A/B/C... kimi avtomatik təyin edir
- Nəticəni quiz_adapted.json faylında saxlayır

## Necə istifadə etmək
1. input.docx faylını yükləyin və C:\Temp qovluğuna qoyun
2. Konsolda layihə qovluğuna keçin və icra edin: dotnet run -- "C:/Temp/input.docx" "C:/Temp/"
3. Nəticə: C:/Temp/quiz_adapted.json faylı ~300 sualla

## Nəticənin nümunəsi (quiz_adapted.json)

İlk sualların JSON formatında necə göründüyü:

![JSON nəticəsinin nümunəsi](example-quiz-json.png)

(Saytdan skrinşot çəkin, `example-quiz-json.png` adı ilə saxlayın və repozitoriyaya əlavə edin)

## Quraşdırma və inkişaf
- .NET 10 SDK quraşdırılmış olmalıdır
- Layihəni klonlayın: git clone https://github.com/NIKO77FIKO/WordQuestionsParser.git
- Lazım olan paketləri bərpa edin: dotnet restore
- Tətbiqi işə salın (yuxarıdakı nümunəyə baxın)

## Gələcək planlar
- Sualların düzgün cavablarını avtomatik aşkarlamaq (bold şrift və ya başqa işarələr)
- Fərqli formatlara ixrac (CSV, Excel, Quizlet və s.)
- GUI (Windows Forms və ya WPF) əlavə etmək
- Bir neçə Word faylını toplu emal etmək

Əməkdaşlığa hazıram!  
Təklifləriniz və ya pull request-ləriniz varsa – xoş gəlmisiniz 😊

## Müəllif
- GitHub: [@NIKO77FIKO](https://github.com/NIKO77FIKO)
