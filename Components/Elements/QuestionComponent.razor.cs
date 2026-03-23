using Microsoft.AspNetCore.Components;
using ProvaOnline.Models.DTO;

namespace ProvaOnline.Components.Elements
{

    public partial class QuestionComponent : ComponentBase
    {
        [Parameter]
        public QuestionWithNoticeDto? Value { get; set; }

        protected bool _showAnswers = false;
        protected string? _selectedValue = null;

        protected override void OnParametersSet()
        {
            // Reset quando Value mudar (ao trocar de página)
            _showAnswers = false;
            _selectedValue = null;
        }

        protected void ToggleAnswers()
        {
            _showAnswers = !_showAnswers;

            // Se está ocultando as respostas, limpa a seleção
            if (!_showAnswers)
            {
                _selectedValue = null;
            }
        }

        protected void OnAnswerSelected(string value)
        {
            _selectedValue = value;
            _showAnswers = true;
        }
    }
}
