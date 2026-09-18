using Estudaki.Commons.Core.CQRS;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Estudaki.Modules.Questions.Application.Commands
{
    public record UploadQuestionImagesCommand(IFormFile File, string PublicNoticeId) : ICommand<CommandResult>;

    public class UploadQuestionImagesCommandValidator : AbstractValidator<UploadQuestionImagesCommand>
    {
        public UploadQuestionImagesCommandValidator()
        {
            RuleFor(x => x.PublicNoticeId).NotEmpty()
                .WithMessage("O ID do edital é obrigatório.");
            RuleFor(x => x.File).NotNull()
                .WithMessage("Os arquivos do exame são obrigatórios.");
        }
    }
}
