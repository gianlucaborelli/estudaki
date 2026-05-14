using Estudaki.Commons.Core.CQRS;
using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Extensions;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace Estudaki.Modules.Questions.Application.Commands;

public class UnifyPublicNoticeCommandHandler : CommandHandler, ICommandHandler<UnifyPublicNoticeCommand, ValidationResult>
{
    private readonly IValidator<UnifyPublicNoticeCommand> _validator;
    private readonly IQuestionSupportRepository _questionSupportRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IPublicNoticeRepository _publicNoticeRepository;
    private readonly IStorageService _storageService;

    public UnifyPublicNoticeCommandHandler(
        IValidator<UnifyPublicNoticeCommand> validator, 
        IQuestionSupportRepository questionSupportRepository,
        IQuestionRepository questionRepository,
        IPublicNoticeRepository publicNoticeRepository,
        IStorageService storageService)
    {
        _validator = validator;
        _questionSupportRepository = questionSupportRepository;
        _questionRepository = questionRepository;
        _publicNoticeRepository = publicNoticeRepository;
        _storageService = storageService;
    }

    private PublicNotice publicNoticeToUnify = new();

    public async Task<ValidationResult> HandleAsync(UnifyPublicNoticeCommand command, CancellationToken cancellationToken = default)
    {
        ValidationResult = await _validator.ValidateAsync(command, cancellationToken);
        if(!ValidationResult.IsValid) return ValidationResult;
        
        var publicNotices = new List<PublicNotice>();
        var questionSupports = new List<QuestionSupport>();
        var questions = new List<Question>();

        foreach (var publicNoticeId in command.PublicNoticeIds)
        {
            var publicNotice = await _publicNoticeRepository.GetById(publicNoticeId);
            if (publicNotice is null)
            {
                ValidationResult.Errors.Add(new ValidationFailure(nameof(command.PublicNoticeIds), $"Public notice with ID {publicNoticeId} not found."));
                return ValidationResult;
            }
            publicNotices.Add(publicNotice);

            var supports = await _questionSupportRepository.GetByPublicNoticeId(publicNoticeId);
            questionSupports.AddRange(supports);

            var examQuestions = await _questionRepository.GetByPublicNoticeId(publicNoticeId);
            questions.AddRange(examQuestions);
        }

        publicNoticeToUnify = publicNotices.First();

        publicNoticeToUnify.Exams = publicNotices.SelectMany(pn => pn.Exams).ToList();

        foreach(var exam in publicNoticeToUnify.Exams)
        {
            var oldExamBookletUrl = exam.ExamBookletUrl;
            var oldExamAnswerKeyUrl = exam.AnswerKeyUrl;

            var newExamBookletUrl = publicNoticeToUnify.BuildExamFilePath(exam.Id);
            var newExamAnswerKeyUrl = publicNoticeToUnify.BuildAnswerKeyPath(exam.Id);

            var examBookletExists = await _storageService.FileExistsAsync(oldExamBookletUrl);
            if (examBookletExists)
            {
                //await _storageService.MoveFileAsync(oldExamBookletUrl, newExamBookletUrl);
                exam.ExamBookletUrl = newExamBookletUrl;
            }

            var examAnswerKeyExists = await _storageService.FileExistsAsync(oldExamAnswerKeyUrl);
            if(examAnswerKeyExists)
            {
                //await _storageService.MoveFileAsync(oldExamAnswerKeyUrl, newExamAnswerKeyUrl);
                exam.AnswerKeyUrl = newExamAnswerKeyUrl;
            }
        }

        foreach (var support in questionSupports)
        {
            support.PublicNoticeId = publicNoticeToUnify.Id;
            await UpdateImages(support.Contents);
        }            

        foreach (var question in questions)
        {
            var questionExam = question.Exams;

            foreach (var exam in questionExam!)
            {
                exam.Year = publicNoticeToUnify.Year;
                exam.ExamCategory = publicNoticeToUnify.ExamCategory;
                exam.ExaminerOrganization = publicNoticeToUnify.ExaminerOrganization;
                exam.ContractingOrganization = publicNoticeToUnify.ContractingOrganization;
            }

            await UpdateImages(question.QuestionContents);
        }

        //await _publicNoticeRepository.Update(publicNoticeToUnify);
        //foreach (var question in questions) await _questionRepository.Update(question);
        //foreach (var support in questionSupports) await _questionSupportRepository.Update(support);

        return ValidationResult;
    }

    public async Task UpdateImages(List<ContentBlock> contentBlocks)
    {
        foreach (var content in contentBlocks)
        {
            if (content is ImageBlock imageBlock)
            {
                var oldImagePath = imageBlock.Key;
                var newImagePath = publicNoticeToUnify.GetImagesFolder();

                if (oldImagePath is not null)
                {
                    var fileExists = await _storageService.FileExistsAsync(oldImagePath);

                    if (fileExists)
                    {
                        var fileName = Path.GetFileName(new Uri(oldImagePath).AbsolutePath);
                        newImagePath = newImagePath + fileName;

                        //await _storageService.MoveFileAsync(oldImagePath, newImagePath);

                        imageBlock.Key = newImagePath;
                    }
                }
            }
            else if (content is ParagraphBlock paragraphBlock)
            {
                foreach (var inlineContent in paragraphBlock.Inlines)
                {
                    if (inlineContent is ImageInline imageInInline)
                    {
                        var oldImagePath = imageInInline.Key;
                        var newImagePath = publicNoticeToUnify.GetImagesFolder();

                        if (oldImagePath is not null)
                        {
                            var fileExists = await _storageService.FileExistsAsync(oldImagePath);

                            if (fileExists)
                            {
                                var fileName = Path.GetFileName(new Uri(oldImagePath).AbsolutePath);
                                newImagePath = newImagePath + fileName;

                                //await _storageService.MoveFileAsync(oldImagePath, newImagePath);

                                imageInInline.Key = newImagePath;
                            }
                        }
                    }
                }
            }
        }
    }
}
