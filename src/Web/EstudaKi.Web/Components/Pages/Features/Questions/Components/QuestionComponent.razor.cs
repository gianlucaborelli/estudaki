using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Extensions;
using Estudaki.Commons.Core.Storage;
using Microsoft.AspNetCore.Components;

namespace EstudaKi.Web.Components.Pages.Features.Questions.Components
{

    public partial class QuestionComponent : ComponentBase
    {
        [Inject]
        private ILogger<QuestionComponent> Logger { get; set; } = default!;

        [Inject]
        private IStorageService StorageService { get; set; } = default!;

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
                Value?.QuestionId,
                Value?.PublicNoticeId,
                Value?.ExaminerOrganization,
                Value?.Year,
                Value?.ExamBookletUrl
            );
        }

        protected void OnDownloadAnswerKey()
        {
            Logger.LogInformation(
                "Answer key download started - QuestionId: {QuestionId}, PublicNoticeId: {PublicNoticeId}, ExamBoard: {ExamBoard}, Year: {Year}, Url: {Url}",
                Value?.QuestionId,
                Value?.PublicNoticeId,
                Value?.ExaminerOrganization,
                Value?.Year,
                Value?.AnswerKeyUrl
            );
        }

        private string GetImageUrl(string key)
        {
            //To-do: Refatorar para evitar criar um PublicNotice só para usar a extensão. 
            if (string.IsNullOrEmpty(Value?.PublicNoticeId))
            {
                return string.Empty;
            }

            
            var notice = new PublicNotice
            {
                Id = Value.PublicNoticeId,
                Year = Value.Year,                
            };

            return notice.GetImageUrl(key, StorageService);
        }
    }
}
