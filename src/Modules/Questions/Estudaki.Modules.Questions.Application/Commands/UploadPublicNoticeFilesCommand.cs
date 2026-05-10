using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public record UploadPublicNoticeFilesCommand(string publicNoticeId, string examId, UploadFileDto examFile, UploadFileDto answerKeyFile) : ICommand<ValidationResult>;

    public class UploadPublicNoticeFilesCommandValidator : AbstractValidator<UploadPublicNoticeFilesCommand>
    {
        public UploadPublicNoticeFilesCommandValidator()
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
