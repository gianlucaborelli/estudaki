using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Models.DTOs;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public record UploadQuestionImagesCommand(List<UploadFileDto> Files, string PublicNoticeId) : ICommand<ValidationResult>;

    public class UploadQuestionImagesCommandValidator : AbstractValidator<UploadQuestionImagesCommand>
    {
        public UploadQuestionImagesCommandValidator()
        {
            RuleFor(x => x.PublicNoticeId).NotEmpty()
                .WithMessage("O ID do edital é obrigatório.");
            RuleFor(x => x.Files).NotNull()
                .WithMessage("Os arquivos do exame são obrigatórios.");
        }
    }
}
