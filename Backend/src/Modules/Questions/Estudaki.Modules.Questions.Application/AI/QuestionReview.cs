using Estudaki.Modules.Questions.Application.Commands.ReviewQuestionsByPublicNoticeId;

namespace Estudaki.Modules.Questions.Application.AI;

/// <summary>
/// Representa a revisão de uma questão realizada pela IA, contendo os defeitos encontrados.
/// Este é o formato de resposta estruturada esperado do agente de IA (ver <see cref="Estudaki.Commons.Core.AI.IAIService.RunAgentAsync{TResponse}"/>).
/// </summary>
public class QuestionReview
{
    public IAQuestion Question { get; set; } = new IAQuestion();

    /// <summary>
    /// Indica se a IA encontrou algum defeito na questão.
    /// </summary>
    public bool HasDefects { get; set; }

    /// <summary>
    /// Resumo geral da avaliação feita pela IA.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Lista de defeitos encontrados na questão.
    /// </summary>
    public List<QuestionDefect> Defects { get; set; } = [];
}

/// <summary>
/// Representa um defeito específico encontrado pela IA ao revisar uma questão.
/// </summary>
public class QuestionDefect
{
    /// <summary>
    /// Categoria do defeito (ex.: "Ambiguidade", "Gabarito incorreto", "Erro gramatical", "Enunciado incompleto").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do defeito encontrado.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Severidade do defeito (ex.: "Baixa", "Média", "Alta").
    /// </summary>
    public string Severity { get; set; } = string.Empty;
}
