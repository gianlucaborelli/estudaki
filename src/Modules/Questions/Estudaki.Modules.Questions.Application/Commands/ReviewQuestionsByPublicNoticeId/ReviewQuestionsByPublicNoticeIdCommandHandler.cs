using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Estudaki.Commons.Core.AI;
using Estudaki.Commons.Core.AI.Prompts;
using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.AI;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Extensions;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands.ReviewQuestionsByPublicNoticeId;

public class ReviewQuestionsByPublicNoticeIdCommandHandler
    : CommandHandler, ICommandHandler<ReviewQuestionsByPublicNoticeIdCommand, List<QuestionReviewResult>>
{
    private readonly IValidator<ReviewQuestionsByPublicNoticeIdCommand> _validator;
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository;
    private readonly IAIService _aiService;

    public ReviewQuestionsByPublicNoticeIdCommandHandler(
        IValidator<ReviewQuestionsByPublicNoticeIdCommand> validator,
        IQuestionRepository questionRepository,
        IQuestionSupportRepository questionSupportRepository,
        IAIService aiService)
    {
        _validator = validator;
        _questionRepository = questionRepository;
        _questionSupportRepository = questionSupportRepository;
        _aiService = aiService;        
    }

    public async Task<List<QuestionReviewResult>> HandleAsync(
        ReviewQuestionsByPublicNoticeIdCommand command,
        CancellationToken cancellationToken = default)
    {
        var results = new List<QuestionReviewResult>();

        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!ValidationResult.IsValid)
            return results;

        var prompt = await _aiService.GetPromptAsync(AIPromptNames.ReviewQuestion, cancellationToken);

        if(string.IsNullOrEmpty(prompt))
        {
            AddError("Prompt de revisão de questões não configurado.");
            return results;
        }

        var questions = await _questionRepository.GetByPublicNoticeId(command.PublicNoticeId);
        var questionSupports = await _questionSupportRepository.GetByPublicNoticeId(command.PublicNoticeId);

        foreach (var question in questions)
        {
            var iaQuestion = question.ToIaQuestion(questionSupports);
            var questionContent = JsonSerializer.Serialize(iaQuestion);

            var result = await ReviewQuestionAsync(question.Id, prompt, questionContent, cancellationToken);
            results.Add(result);
        }

        return results;
    }

    private async Task<QuestionReviewResult> ReviewQuestionAsync(
        string questionId,
        string promptInstructions,
        string questionContent,
        CancellationToken cancellationToken)
    {
        try
        {
            var message = AIChatMessage.FromUser(questionContent);
            var review = await _aiService.RunAgentAsync<QuestionReview>(
                promptInstructions,
                [message],
                cancellationToken);

            return new QuestionReviewResult(questionId, true, review, null);
        }
        catch (Exception ex)
        {
            return new QuestionReviewResult(questionId, false, null, ex.Message);
        }
    }
}

public static class QuestionExtensions
{
    private static string GetInlineText(InlineContent inline)
        => inline switch
        {
            TextInline t => t.Text,
            ImageInline img => img.Alt ?? string.Empty,
            _ => string.Empty
        };

    private static SimpleContent ToSimpleContent(ContentBlock block)
        => block switch
        {
            ParagraphBlock p => new SimpleContent
            {
                Text = string.Join(" ", p.Inlines.Select(GetInlineText)),
                Order = p.Order,
                Type = ContentType.Text.ToString(),
            },
            ImageBlock img => new SimpleContent
            {
                Text = img.Description ?? string.Empty,
                Order = img.Order,
                Type = ContentType.Image.ToString(),
            },
            _ => throw new NotSupportedException()
        };

    public static IAQuestion ToIaQuestion(this Question question, List<QuestionSupport> availableSupports)
    {
        var questionSupports = availableSupports
            .Where(s => question.QuestionSupports.Contains(s.Id))
            .SelectMany(s => s.Contents)
            .Select(ToSimpleContent)
            .ToList();

        return new IAQuestion
        {
            Id = question.Id,
            MainArea = question.MainArea,
            SubAreas = question.SubAreas.ToList(),
            QuestionContents = question.QuestionContents
                .Select(ToSimpleContent)
                .ToList(),

            QuestionSupports = questionSupports,

            Alternatives = question.Choices?
                .Select(c => new SimpleAlternative
                {
                    Letter = c.Option,
                    IsCorrect = c.IsCorrect,
                    Text = string.Join(" ", c.Content.Select(GetInlineText)),
                    Type = c.Content.Any(i => i is ImageInline)
                        ? ContentType.Image.ToString()
                        : ContentType.Text.ToString(),
                }).ToList(),
        };
    }
}

public class IAQuestion 
{
    public string Id { get; set; } = string.Empty;

    public string MainArea { get; set; } = string.Empty;

    public List<string> SubAreas { get; set; } = [];

    /// <summary>
    /// Enunciado da questão.
    /// </summary>
    public List<SimpleContent> QuestionContents { get; set; } = [];

    /// <summary>
    /// Suportes da questão, como imagens, gráficos ou tabelas.
    /// </summary>
    public List<SimpleContent> QuestionSupports { get; set; } = [];
    /// <summary>
    /// Alternativas da questão.
    /// </summary>
    public List<SimpleAlternative>? Alternatives { get; set; } = [];

}

public class SimpleContent
{
    public string Text { get; set; } = string.Empty;
    public string Type {  get; set; } = string.Empty;
    public int Order { get; set; }

    public static string GetContentType(string type)
    {
        return type.ToLower() switch
        {
            "text" => ContentType.Text.ToString(),
            "image" => ContentType.Image.ToString(),
            _ => throw new ArgumentException($"Tipo de conteúdo inválido: {type}")
        };
    }

    public static ContentType GetContentTypeEnum(string type)
    {
        return type.ToLower() switch
        {
            "text" => ContentType.Text,
            "image" => ContentType.Image,
            _ => throw new ArgumentException($"Tipo de conteúdo inválido: {type}")
        };
    }    
}

public enum ContentType
{
    Text,
    Image
}

public class SimpleAlternative : SimpleContent
{
    public string? Letter { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
}
