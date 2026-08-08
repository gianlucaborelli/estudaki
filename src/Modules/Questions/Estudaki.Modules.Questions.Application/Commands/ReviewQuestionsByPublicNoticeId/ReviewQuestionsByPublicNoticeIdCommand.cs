using Estudaki.Commons.Core.CQRS;
using FluentValidation;

namespace Estudaki.Modules.Questions.Application.Commands.ReviewQuestionsByPublicNoticeId;

/// <summary>
/// Comando que busca todas as questões relacionadas a um edital (PublicNotice) e solicita
/// à IA a revisão de cada uma delas, retornando os defeitos encontrados.
/// </summary>
public record ReviewQuestionsByPublicNoticeIdCommand(string PublicNoticeId) : ICommand<List<QuestionReviewResult>>;

public class ReviewQuestionsByPublicNoticeIdCommandValidator : AbstractValidator<ReviewQuestionsByPublicNoticeIdCommand>
{
    public ReviewQuestionsByPublicNoticeIdCommandValidator()
    {
        RuleFor(x => x.PublicNoticeId).NotEmpty()
            .WithMessage("O ID do edital é obrigatório.");
    }
}
