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

    public static QuestionSupport ToEntity(this QuestionSupportDto questionSupportDto)
    {
        return new QuestionSupport
        {
            Id = questionSupportDto.Id,
            PublicNoticeId = questionSupportDto.PublicNoticeId,
            Contents = questionSupportDto.Contents
        };
    }


    public static List<QuestionSupportDto> ToDtoList(this List<QuestionSupport> questionSupports)
    {
        return questionSupports.Select(q => q.ToDto()).ToList();
    }
}
