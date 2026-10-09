using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public record UploadExamFilesCommand(string publicNoticeId, string examId, IFormFile examFile, IFormFile answerKeyFile) : ICommand<CommandResult>;

    public class UploadExamFilesCommandValidator : AbstractValidator<UploadExamFilesCommand>
    {
        public UploadExamFilesCommandValidator()
        {
            RuleFor(x => x.publicNoticeId).NotEmpty()
                .WithMessage("O ID do edital é obrigatório.");
            RuleFor(x => x.examId).NotEmpty()
                .WithMessage("O ID do exame é obrigatório.");
            RuleFor(x => x.examFile).NotNull()
                .WithMessage("O arquivo do exame é obrigatório.");
            RuleFor(x => x.answerKeyFile).NotNull()
                .WithMessage("O arquivo do gabarito é obrigatório.");
        }
    }
}
