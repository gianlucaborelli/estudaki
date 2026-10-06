using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.Mappers;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands.CreateNewPublicNotice;

public class CreateNewPublicNoticeCommandHandler : CommandHandler, ICommandHandler<CreateNewPublicNoticeCommand, CommandResult>
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IExamExtractionRepository _examExtractionRepository;
    private readonly IValidator<CreateNewPublicNoticeCommand> _validator;

    public CreateNewPublicNoticeCommandHandler(
        IQuestionRepository questionRepository, 
        IPublicNoticeRepository publicNoticeRepository, 
        IExamExtractionRepository examExtractionRepository,
        IValidator<CreateNewPublicNoticeCommand> validator)
    {
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _examExtractionRepository = examExtractionRepository;
        _validator = validator;
    }

    public async Task<CommandResult> HandleAsync(CreateNewPublicNoticeCommand command, CancellationToken cancellationToken = default)
    {
        SetValidationResult(await _validator.ValidateAsync(command, cancellationToken));
        if (!CommandResult.Success) return Result();

        var publicNotice = command.PublicNotice.ToEntity();
        publicNotice.CreatedAt = DateTime.UtcNow;

        _publicNoticeRepository.Add(publicNotice);
                
        var questionExam = QuestionExam.Create(command.PublicNotice.Exams[0], publicNotice);

        if (command.ExamExtraction.Questions != null && command.ExamExtraction.Questions.Any())
        {
            foreach (var questionDto in command.ExamExtraction.Questions) {
                questionExam.QuestionNumber = questionDto.QuestionNumber;

                var question = Question.Create(
                    QuestionType.MultipleChoice,
                    string.Empty,
                    Array.Empty<string>(),
                    new List<string> { },
                    questionDto.Content,
                    questionDto.SingleChoices.Select(c => new Choice
                    {
                        Explanation = c.Content,
                        Option = c.Option,
                        IsCorrect = false
                    }).ToList(),
                    questionExam
                );

                question.Exams.Add(questionExam);
                _questionRepository.Add(question);
            }
        }

        await _examExtractionRepository.Remove(command.ExamExtraction.Id);

        return Result();
    }
}
