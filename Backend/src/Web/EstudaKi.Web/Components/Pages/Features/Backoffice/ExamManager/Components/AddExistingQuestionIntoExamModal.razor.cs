using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionsByPublicNoticeId;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components;

public partial class AddExistingQuestionIntoExamModalBase: ComponentBase
{
    [CascadingParameter]
    public IMudDialogInstance Dialog { get; set; } = default!;

    [Parameter]
    public string ExamId { get; set; } = string.Empty;
    [Parameter]
    public string PublicNoticeId { get; set; } = string.Empty;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;    
    [Inject]
    protected IQueryDispatcher QueryDispatcher { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;


    protected List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();

    protected QuestionDto? SelectedQuestion { get; set; } = null;
    protected string SearchString { get; set; } = string.Empty;


    protected override async Task OnParametersSetAsync()
    {
        var getQuestionsbyExamIdQuery = new GetQuestionsByPublicNoticeIdQuery(PublicNoticeId);
        var questions = await QueryDispatcher.DispatchAsync<GetQuestionsByPublicNoticeIdQuery, List<QuestionDto>>(getQuestionsbyExamIdQuery);
        Questions = questions.ToList();
    }

    protected async Task AddQuestionToExam()
    {
        if(SelectedQuestion == null || string.IsNullOrEmpty(ExamId))
        {
            Snackbar.Add("Selecione uma questão e um exame antes de adicionar.", Severity.Warning);
            return;
        }
        var command = new AddExistingQuestionIntoExamCommand(SelectedQuestion, ExamId);
        var result = await CommandDispatcher.DispatchAsync<AddExistingQuestionIntoExamCommand, ValidationResult>(command);

        if(result.IsValid)
        {
            Snackbar.Add("Questão adicionada com sucesso.", Severity.Success);
            Dialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Snackbar.Add("Falha ao adicionar a questão.", Severity.Error);
        }
    }

    protected bool FilterFunc1(QuestionDto question) => FilterFunc(question, SearchString);

    protected bool FilterFunc(QuestionDto question, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;

        if (question.QuestionNumber.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (!string.IsNullOrWhiteSpace(question.MainArea) && 
            question.MainArea.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (question.SubAreas.Any(sa => sa.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
            return true;

        if (!string.IsNullOrWhiteSpace(question.QuestionType) && 
            question.QuestionType.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (question.Positions.Any(p => p.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
            return true;

        foreach (var block in question.QuestionContents)
        {
            if (block is ParagraphBlock paragraph)
            {
                if (!string.IsNullOrWhiteSpace(paragraph.Title) && 
                    paragraph.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                foreach (var inline in paragraph.Inlines)
                {
                    if (inline is TextInline textInline && 
                        !string.IsNullOrWhiteSpace(textInline.Text) &&
                        textInline.Text.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            else if (block is ImageBlock imageBlock)
            {
                if ((!string.IsNullOrWhiteSpace(imageBlock.Title) && 
                     imageBlock.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(imageBlock.Description) && 
                     imageBlock.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    return true;
            }
        }

        if (question.Choices != null)
        {
            foreach (var choice in question.Choices)
            {
                if (!string.IsNullOrWhiteSpace(choice.Option) && 
                    choice.Option.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                foreach (var inline in choice.Content)
                {
                    if (inline is TextInline textInline && 
                        !string.IsNullOrWhiteSpace(textInline.Text) &&
                        textInline.Text.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
        }

        return false;
    }

    protected string SelectedQuestionRowClassFunc(QuestionDto question, int rowNumber)
            => SelectedQuestion == question ? "selected" : string.Empty;
}
