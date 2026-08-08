namespace Estudaki.Commons.Core.AI.Prompts;

/// <summary>
/// Nomes centralizados dos prompts de IA armazenados no MongoDB.
/// Utilize estas constantes ao buscar um prompt por nome, evitando strings "mágicas" espalhadas pelo código.
/// </summary>
public static class AIPromptNames
{
    /// <summary>
    /// Prompt utilizado para gerar novas questões a partir de um conteúdo/tema.
    /// </summary>
    public const string GenerateQuestions = "generate-questions";

    /// <summary>
    /// Prompt utilizado para corrigir/avaliar a resposta de um usuário.
    /// </summary>
    public const string CorrectAnswer = "correct-answer";

    /// <summary>
    /// Prompt utilizado para gerar um resumo de conteúdo de estudo.
    /// </summary>
    public const string SummarizeContent = "summarize-content";

    /// <summary>
    /// Prompt utilizado para revisar uma questão em busca de defeitos (ambiguidade, gabarito incorreto, erros de digitação, etc.).
    /// </summary>
    public const string ReviewQuestion = "review-question";
}
