using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.Mappers;

public static class QuestionSupportMapper
{
    public static QuestionSupportDto ToDto(this QuestionSupport questionSupport)
    {
        return new QuestionSupportDto
        {
            Id = questionSupport.Id,
            PublicNoticeId = questionSupport.PublicNoticeId,
            Contents = questionSupport.Contents
        };
    }
}
