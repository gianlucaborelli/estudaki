using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Commands;
using Estudaki.Modules.Questions.Application.DTOs;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace EstudaKi.Web.Components.Pages.Features.Backoffice.ExamManager.Components
{
    public partial class EditPublicNoticeModalBase : ComponentBase
    {
        [CascadingParameter]
        protected IMudDialogInstance Dialog { get; set; } = default!;

        [Inject]
        protected ICommandDispatcher CommandDispatcher { get; set; } = default!;
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;

        protected MudForm Form = default!;
        protected bool IsUploading { get; set; } = false;
        

        [Parameter]
        public PublicNoticeDto? OriginalNoticePublic { get; set; }
        protected PublicNoticeDto EditedPublicNotice { get; set; } = new PublicNoticeDto();

        protected override void OnParametersSet()
        {
            if (OriginalNoticePublic != null)
            {
                EditedPublicNotice = PublicNoticeDto.Clone(OriginalNoticePublic);
            }
        }

        protected async Task SavePublicNotice()
        {
            try
            {
                IsUploading = true;

                var updatePublicNoticeCommand = new UpdatePublicNoticeCommand(EditedPublicNotice);
                var result = await CommandDispatcher
                    .DispatchAsync<UpdatePublicNoticeCommand, ValidationResult>(updatePublicNoticeCommand);

                if (result.IsValid)
                {
                    Snackbar.Add("Public notice updated successfully!", Severity.Success);
                    Dialog.Close();
                }
                else
                {
                    var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage));
                    Snackbar.Add($"Failed to update public notice: {errors}", Severity.Error);
                }

            }
            catch (Exception ex)
            {
                Snackbar.Add("An error occurred while saving the public notice.", Severity.Error); Snackbar.Add(ex.Message);
                Console.Error.WriteLine($"Error saving public notice: {ex.Message}");
            }
            finally { IsUploading = false; }
        }
    }
}
