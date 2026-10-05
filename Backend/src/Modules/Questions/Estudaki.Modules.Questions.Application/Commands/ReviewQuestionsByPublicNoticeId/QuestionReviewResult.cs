namespace Estudaki.Modules.Questions.Application.Commands.ReviewQuestionsByPublicNoticeId;

/// <summary>
/// Resultado da revisão de uma questão específica pela IA.
/// </summary>
/// <param name="QuestionId">Identificador da questão revisada.</param>
/// <param name="Success">Indica se a revisão foi concluída com sucesso.</param>
/// <param name="Review">Revisão estruturada retornada pela IA, quando <paramref name="Success"/> é verdadeiro.</param>
/// <param name="ErrorMessage">Mensagem de erro, quando a revisão falha para esta questão.</param>
public record QuestionReviewResult(string QuestionId, bool Success, AI.QuestionReview? Review, string? ErrorMessage);
