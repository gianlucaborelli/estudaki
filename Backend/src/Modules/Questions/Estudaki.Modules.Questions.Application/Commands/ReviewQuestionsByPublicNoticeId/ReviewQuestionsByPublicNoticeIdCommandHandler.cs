using System.Text.Json;
using Estudaki.Commons.Core.AI;
using Estudaki.Commons.Core.AI.Prompts;
using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.AI;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands.ReviewQuestionsByPublicNoticeId;

public class ReviewQuestionsByPublicNoticeIdCommandHandler
    : CommandHandler, ICommandHandler<ReviewQuestionsByPublicNoticeIdCommand, CommandResult>
{
    /// <summary>
    /// Limite máximo de requisições concorrentes enviadas à IA. O provedor utilizado
    /// suporta até 2500 requisições concorrentes, porém um valor mais conservador é
    /// usado por padrão para evitar sobrecarga desnecessária em lotes muito grandes.
    /// </summary>
    private const int MaxConcurrency = 50;

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

    public async Task<CommandResult> HandleAsync(
        ReviewQuestionsByPublicNoticeIdCommand command,
        CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var prompt = await _aiService.GetPromptAsync(AIPromptNames.ReviewQuestion, cancellationToken);

        if(string.IsNullOrEmpty(prompt))
        {
            AddError("Prompt de revisão de questões não configurado.");
            return Result();
        }

        var questions = await _questionRepository.GetByPublicNoticeId(command.PublicNoticeId);
        var questionSupports = await _questionSupportRepository.GetAllByPublicNoticeIdAsync(command.PublicNoticeId);

        using var throttler = new SemaphoreSlim(MaxConcurrency);

        var reviewTasks = questions.Select(async question =>
        {
            await throttler.WaitAsync(cancellationToken);
            try
            {
                var iaQuestion = question.ToIaQuestion(questionSupports);
                var questionContent = JsonSerializer.Serialize(iaQuestion);

                return await ReviewQuestionAsync(question.Id, prompt, questionContent, cancellationToken);
            }
            finally
            {
                throttler.Release();
            }
        });

        var reviewResults = await Task.WhenAll(reviewTasks);
        var results = new List<QuestionReviewResult>();
        results.AddRange(reviewResults);

        return Result(results);
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
    public static IAQuestion ToIaQuestion(this Question question, List<QuestionSupport> availableSupports)
    {
        var questionSupports = availableSupports
            .Where(s => question.QuestionSupports.Contains(s.Id))
            .Select(c => new SimpleContent
            {
                Text = string.Join(" ", c.Content),
            })
            .ToList();

        return new IAQuestion
        {
            Id = question.Id,
            MainArea = question.MainArea,
            SubAreas = question.SubAreas.ToList(),
            Statement = question.Statement,

            QuestionSupports = questionSupports,

            Alternatives = question.Choices?
                .Select(c => new SimpleAlternative
                {
                    Letter = c.Option,
                    IsCorrect = c.IsCorrect,
                    Text = string.Join(" ", c.Explanation)
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
    public string Statement { get; set; } = string.Empty;

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
    public int Order { get; set; }    
}

public class SimpleAlternative : SimpleContent
{
    public string? Letter { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
}
