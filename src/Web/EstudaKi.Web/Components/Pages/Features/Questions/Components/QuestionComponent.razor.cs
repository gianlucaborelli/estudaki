using System.Text;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Estudaki.Commons.Core.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Questions.Components
{

    public partial class QuestionComponent : ComponentBase
    {
        [Inject]
        private ILogger<QuestionComponent> Logger { get; set; } = default!;

        [Inject]
        private IStorageService StorageService { get; set; } = default!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject]
        private ISnackbar Snackbar { get; set; } = default!;

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

        protected async Task OnCopyQuestion()
        {
            var text = BuildQuestionText();

            try
            {
                await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
                Snackbar.Add("Questão copiada para a área de transferência.", Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to copy question to clipboard - QuestionId: {QuestionId}", Value?.QuestionId);
                Snackbar.Add("Não foi possível copiar a questão.", Severity.Error);
            }
        }

        private string BuildQuestionText()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(Value?.ExaminerOrganization) || Value?.Year > 0)
            {
                sb.AppendLine($"({Value?.ExaminerOrganization}, {Value?.Year})");
            }

            if (Value?.QuestionSupports != null)
            {
                foreach (var support in Value.QuestionSupports)
                {
                    if (support.Contents != null)
                    {
                        foreach (var contentBlock in support.Contents.OrderBy(c => c.Order))
                        {
                            AppendContentBlock(sb, contentBlock);
                        }
                    }
                }
            }

            if (Value?.QuestionContents != null)
            {
                foreach (var contentBlock in Value.QuestionContents.OrderBy(c => c.Order))
                {
                    AppendContentBlock(sb, contentBlock);
                }
            }

            if (Value?.Choices != null && Value.Choices.Count > 0)
            {
                sb.AppendLine();
                foreach (var choice in Value.Choices)
                {
                    sb.Append(choice.Option).Append(") ");
                    sb.AppendLine(BuildInlineText(choice.Content));
                }
            }

            return sb.ToString().Trim();
        }

        private static void AppendContentBlock(StringBuilder sb, ContentBlock contentBlock)
        {
            if (contentBlock is ParagraphBlock paragraph)
            {
                if (!string.IsNullOrEmpty(paragraph.Title))
                {
                    sb.AppendLine(paragraph.Title);
                }

                sb.AppendLine(BuildInlineText(paragraph.Inlines));

                if (!string.IsNullOrEmpty(paragraph.Source))
                {
                    sb.AppendLine($"Fonte: {paragraph.Source}");
                }

                sb.AppendLine();
            }
            else if (contentBlock is ImageBlock imageBlock)
            {
                if (!string.IsNullOrEmpty(imageBlock.Title))
                {
                    sb.AppendLine(imageBlock.Title);
                }

                if (!string.IsNullOrEmpty(imageBlock.Source))
                {
                    sb.AppendLine($"Fonte: {imageBlock.Source}");
                }

                sb.AppendLine();
            }
        }

        private static string BuildInlineText(IEnumerable<InlineContent>? inlines)
        {
            if (inlines == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var inline in inlines)
            {
                if (inline is TextInline textInline)
                {
                    sb.Append(textInline.Text);
                }
            }

            return sb.ToString();
        }
    }
}
