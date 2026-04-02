using Estudaki.Modules.Questions.Application.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace EstudaKi.Web.Pages.Features.Questions.Components
{

    public partial class QuestionComponent : ComponentBase
    {
        [Inject]
        private ILogger<QuestionComponent> Logger { get; set; } = default!;

        [Parameter]
        public QuestionDto? Value { get; set; }

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

        protected void OnDownloadExamBooklet()
        {
            Logger.LogInformation(
                "Exam booklet download started - QuestionId: {QuestionId}, PublicNoticeId: {PublicNoticeId}, ExamBoard: {ExamBoard}, Year: {Year}, Url: {Url}",
                Value?.Id,
                Value?.PublicNotice?.Id,
                Value?.PublicNotice?.ExamBoard,
                Value?.PublicNotice?.Year,
                Value?.PublicNotice?.ExamBookletUrl
            );
        }

        protected void OnDownloadAnswerKey()
        {
            Logger.LogInformation(
                "Answer key download started - QuestionId: {QuestionId}, PublicNoticeId: {PublicNoticeId}, ExamBoard: {ExamBoard}, Year: {Year}, Url: {Url}",
                Value?.Id,
                Value?.PublicNotice?.Id,
                Value?.PublicNotice?.ExamBoard,
                Value?.PublicNotice?.Year,
                Value?.PublicNotice?.AnswerKeyUrl
            );
        }
    }
}
