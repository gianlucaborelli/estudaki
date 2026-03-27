using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionById;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text;
using System.Text.RegularExpressions;

namespace EstudaKi.Web.Components.Pages;

public partial class QuestionDetailBase : ComponentBase
{
    [Inject]
    protected IQueryDispatcher _queryDispatcher { get; set; } = default!;

    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;

    [Parameter]
    public string Id { get; set; } = string.Empty;

    protected QuestionWithNoticeDto? Question { get; set; }
    protected bool IsLoading { get; set; } = true;

    protected List<BreadcrumbItem> _breadcrumbItems = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadQuestionAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrEmpty(Id))
        {
            await LoadQuestionAsync();
        }
    }

    private async Task LoadQuestionAsync()
    {
        IsLoading = true;

        try
        {
            Question = await _queryDispatcher.DispatchAsync<GetQuestionByIdQuery, QuestionWithNoticeDto>(new GetQuestionByIdQuery(Id));

            if (Question != null)
            {
                SetupBreadcrumb();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao carregar questão: {ex.Message}", Severity.Error);
            Question = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void SetupBreadcrumb()
    {
        _breadcrumbItems = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Início", href: "/"),
            new BreadcrumbItem("Busca", href: "/result"),
            new BreadcrumbItem($"Questão #{Question?.Question.QuestionNumber ?? 0}", href: null, disabled: true)
        };
    }

    protected string GetPageTitle()
    {
        if (Question == null) return "Questão não encontrada - EstudaKi";

        var examName = Question.PublicNotice?.ExamBoard ?? Question.Question.QuestionType;
        var year = Question.PublicNotice?.Year ?? DateTime.Now.Year;

        return $"Questão {Question.Question.QuestionNumber} - {examName} {year} - {Question.Question.MainArea} | EstudaKi";
    }

    protected string GetMetaDescription()
    {
        if (Question == null) return "Questão não encontrada";

        var description = new StringBuilder();
        description.Append($"Pratique questão de {Question.Question.MainArea}");

        if (Question.Question.SubAreas.Length > 0)
        {
            description.Append($" sobre {string.Join(", ", Question.Question.SubAreas.Take(2))}");
        }

        var examName = Question.PublicNotice?.ExamBoard ?? Question.Question.QuestionType;
        description.Append($" do {examName}");

        if (Question.PublicNotice?.Year != null)
        {
            description.Append($" {Question.PublicNotice.Year}");
        }

        description.Append(". Resolva gratuitamente e aprimore seus conhecimentos.");

        return description.ToString();
    }

    protected string GetKeywords()
    {
        if (Question == null) return "questões, vestibular, concurso, OAB";

        var keywords = new List<string>
        {
            Question.Question.MainArea,
            Question.Question.QuestionType,
            $"questão {Question.Question.QuestionNumber}",
            "questões online",
            "estudo gratuito"
        };

        keywords.AddRange(Question.Question.SubAreas.Take(3));

        if (Question.PublicNotice != null)
        {
            keywords.Add(Question.PublicNotice.ExamBoard);
            keywords.Add($"{Question.PublicNotice.ExamBoard} {Question.PublicNotice.Year}");
        }

        return string.Join(", ", keywords);
    }

    protected string GetCanonicalUrl()
    {
        return $"https://estudaki.com.br/question/{Id}";
    }

    protected string GetStructuredData()
    {
        if (Question == null) return "{}";

        var examName = Question.PublicNotice?.ExamBoard ?? Question.Question.QuestionType;
        var year = Question.PublicNotice?.Year ?? DateTime.Now.Year;

        var questionText = GetQuestionTextForStructuredData();
        var correctAnswer = GetCorrectAnswerText();

        return $@"{{
            ""@context"": ""https://schema.org"",
            ""@type"": ""Question"",
            ""name"": ""Questão {Question.Question.QuestionNumber} - {examName} {year}"",
            ""text"": ""{questionText}"",
            ""answerCount"": {Question.Question.Choices?.Count ?? 0},
            ""eduQuestionType"": ""Multiple choice"",
            ""educationalLevel"": ""Higher Education"",
            ""learningResourceType"": ""Exam Question"",
            ""about"": {{
                ""@type"": ""Thing"",
                ""name"": ""{Question.Question.MainArea}""
            }},
            ""acceptedAnswer"": {{
                ""@type"": ""Answer"",
                ""text"": ""{correctAnswer}""
            }},
            ""author"": {{
                ""@type"": ""Organization"",
                ""name"": ""{examName}""
            }},
            ""datePublished"": ""{Question.Question.CreatedAt:yyyy-MM-dd}""
        }}";
    }

    private string SanitizeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";

        text = Regex.Replace(text, "<.*?>", "");

        if (text.Length > 200)
        {
            text = text.Substring(0, 197) + "...";
        }

        text = text.Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");

        return text;
    }

    private string GetQuestionTextForStructuredData()
    {
        if (Question?.Question.QuestionContents == null) return "";

        var textParts = new List<string>();

        foreach (var content in Question.Question.QuestionContents.OrderBy(c => c.Order))
        {
            if (content is ParagraphBlock paragraph)
            {
                foreach (var inline in paragraph.Inlines)
                {
                    if (inline is TextInline textInline)
                    {
                        textParts.Add(textInline.Text);
                    }
                }
            }
        }

        var fullText = string.Join(" ", textParts);
        return SanitizeText(fullText);
    }

    private string GetCorrectAnswerText()
    {
        var correctChoice = Question?.Question.Choices?.FirstOrDefault(c => c.IsCorrect);
        if (correctChoice != null)
        {
            var textParts = new List<string>();
            foreach (var inline in correctChoice.Content)
            {
                if (inline is TextInline textInline)
                {
                    textParts.Add(textInline.Text);
                }
            }
            return SanitizeText(string.Join(" ", textParts));
        }
        return "";
    }

    protected string GetAreaSearchLink()
    {
        return $"/result?areas={Uri.EscapeDataString(Question?.Question.MainArea ?? "")}";
    }

    protected string GetSubAreaSearchLink(string subArea)
    {
        return $"/result?subareas={Uri.EscapeDataString(subArea)}";
    }

    protected string GetTypeSearchLink()
    {
        return $"/result?types={Uri.EscapeDataString(Question?.Question.QuestionType ?? "")}";
    }
}
