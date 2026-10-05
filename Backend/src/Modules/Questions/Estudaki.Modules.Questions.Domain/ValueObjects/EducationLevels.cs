namespace Estudaki.Modules.Questions.Domain.ValueObjects;

public static class EducationLevels
{
    public const string Elementary = "Elementary";
    public const string HighSchool = "HighSchool";
    public const string Undergraduate = "Undergraduate";
    public const string Technical = "Technical";
    public static readonly string[] All =
    {
        Elementary, HighSchool, Undergraduate, Technical
    };

    public static string GetDisplayName(string category)
    {
        return category switch
        {
            Elementary => "Fundamental",
            HighSchool => "Médio",
            Undergraduate => "Superior",
            Technical => "Técnico",
            _ => category
        };
    }
}
