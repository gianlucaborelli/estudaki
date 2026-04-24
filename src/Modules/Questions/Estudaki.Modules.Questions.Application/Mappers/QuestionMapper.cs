using Estudaki.Commons.Core.Storage;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.Mappers;

public static class QuestionMapper
{
    public static QuestionDto ToDto(
        this Question question, 
        PublicNotice? publicNotice, 
        List<QuestionSupport>? questionSupports,
        IStorageService storageService, 
        StorageSettings s3Settings)
    {
        return new QuestionDto
        {
            Id = question.Id,
            PublicNoticeId = question.PublicNoticeId,
            PublicNotice = publicNotice?.ToDto(storageService, s3Settings),
            QuestionSupports = questionSupports?.Select(qs => qs.ToDto()).ToList() ?? [],
            CreatedAt = question.CreatedAt,
            IsPublished = question.IsPublished,
            IsNullified = question.IsNullified,
            QuestionNumber = question.QuestionNumber,
            QuestionType = question.Type,
            MainArea = question.MainArea,
            SubAreas = question.SubAreas,
            QuestionContents = question.QuestionContents,
            Choices = question.Choices
        };
    }
}
