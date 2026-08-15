using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.DTOs;
using Estudaki.Modules.Questions.Application.Commands.CreateNewPublicNotice;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetExamExtractionList;
using Estudaki.Modules.Questions.Domain.Entities;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Modals;

public class CreateNewPublicNoticeModalBase : ComponentBase
{
    [CascadingParameter]
    protected IMudDialogInstance Dialog { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected ICommandDispatcher CommandDispatcher { get; set; } = default!;
    [Inject]
    protected IQueryDispatcher QueryDispatcher { get; set; } = default!;

    protected PublicNoticeDto NewPublicNotice { get; set; } = new PublicNoticeDto();    
    protected ExamExtractionDto SelectedExam { get; set; } = new ExamExtractionDto();
    protected MudForm Form { get; set; } = default!;
    protected bool IsUploading { get; set; }    
    protected List<ExamExtractionDto> Exams { get; set; } = new List<ExamExtractionDto>();    
    
    protected async override Task OnInitializedAsync()
    {
        await LoadExams();
    }    

    protected async Task LoadExams()
    {
        IsUploading = true;
        var query = new GetExamExtractionListQuery();
        Exams = await QueryDispatcher.DispatchAsync<GetExamExtractionListQuery, List<ExamExtractionDto>>(query);
        IsUploading = false;
    }

    protected async Task SaveAsync() 
    {
        IsUploading = true;
        var command = new CreateNewPublicNoticeCommand(NewPublicNotice, SelectedExam);
        var result = await CommandDispatcher.DispatchAsync<CreateNewPublicNoticeCommand, ValidationResult>(command);

        if(result.IsValid)
        {
            Snackbar.Add("Novo edital criado com sucesso!", Severity.Success);
            await LoadExams();
            StateHasChanged();
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Snackbar.Add(error.ErrorMessage, Severity.Error);
            }
        }

        IsUploading = false;
    }

    protected void OnExamSelected(ExamExtractionDto? exam)
    {
        SelectedExam = exam ?? new ExamExtractionDto();

        if (SelectedExam is null)
        {
            NewPublicNotice = new PublicNoticeDto();
            return;
        }

        NewPublicNotice = new PublicNoticeDto();
        var newExam = new Exam();
        NewPublicNotice.Exams.Add(newExam);        
    }

    protected string SelectedRowClassFunc(
    ExamExtractionDto exam,
    int rowNumber)
    {
        return SelectedExam is not null &&
               SelectedExam.Equals(exam)
            ? "selected"
            : string.Empty;
    }
}
