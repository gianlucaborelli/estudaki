using Microsoft.AspNetCore.Components;

namespace ProvaOnline.Components.Layout
{
    public partial class NavMenuBase : ComponentBase
    {
        protected string[] _questionType{ get; set; } = ["Vestibular", "Concurso", "Prova da OAB"];
        protected IEnumerable<string> _questionTypeSelected { get; set; }

        protected string[] _mainArea{ get; set; } = ["Matemática", "Portugues", "Física", "Direito"];
        protected IEnumerable<string> _mainAreaSelected { get; set; }

        protected string[] _subArea { get; set; } = ["Aritmetica", "Analise Sintatica", "Movimento", "Penal"];
        protected IEnumerable<string> _subAreaSelected { get; set; }

        protected string _wordKey { get; set; }
        



    }
}
