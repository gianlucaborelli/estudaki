namespace Estudaki.Modules.Questions.Domain.ValueObjects;

/// <summary>
/// Representa uma fórmula química inline no conteúdo de uma questão ou texto de apoio.
/// Utiliza a notação mhchem para renderização de fórmulas químicas.
/// </summary>
public class ChemicalFormulaInline : InlineContent
{
    /// <summary>
    /// Fórmula química em notação mhchem.
    /// Exemplos: "H2O", "CO2", "C6H12O6", "H2SO4", etc.
    /// </summary>
    public string Formula { get; set; } = string.Empty;

    public ChemicalFormulaInline() 
    { 
        Type = "chemical";
    }
}
