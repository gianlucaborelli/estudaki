using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.ValueObjects;

namespace Estudaki.Modules.Questions.Application.Mappers;

public static class QuestionMapper
{
    /// <summary>
    /// Converte Question para DTO usando dados desnormalizados.
    /// Não requer mais buscar PublicNotice ou Exam separadamente.
    /// </summary>
    public static QuestionDto ToDto(
        this Question question,
        QuestionExam? questionExam = null,
        List<QuestionSupport>? questionSupports = null)
    {
        // Usa o primeiro exame se não foi especificado
        var exam = questionExam ?? question.Exams.FirstOrDefault();

        if (exam == null)
        {
            throw new InvalidOperationException($"Questão {question.Id} não possui exames associados.");
        }

        // Agregar todas as positions dos exames da questão
        var allPositions = question.Exams
            .Where(qe => !string.IsNullOrWhiteSpace(qe.Position))
            .Select(qe => qe.Position!)
            .Distinct()
            .ToList();

        return new QuestionDto
        {
            QuestionId = question.Id,
            PublicNoticeId = exam.PublicNoticeId,
            ExamId = exam.ExamId,
            PublicNoticeNumber = null,
            Year = exam.Year,
            ExaminerOrganization = exam.ExaminerOrganization,
            ContractingOrganization = exam.ContractingOrganization,
            ExamCategory = exam.ExamCategory,
            Phase = exam.Phase ?? string.Empty,
            Positions = allPositions,
            Area = exam.Area ?? string.Empty,
            EducationLevel = exam.EducationLevel ?? string.Empty,
            PublicNoticeFileUrl = string.Empty, 
            ExamBookletUrl = exam.ExamBookletUrl ?? string.Empty,
            AnswerKeyUrl = exam.AnswerKeyUrl ?? string.Empty,
            IsNullified = question.IsNullified,
            QuestionNumber = exam.QuestionNumber,
            QuestionType = question.Type,
            MainArea = question.MainArea,
            SubAreas = question.SubAreas,
            QuestionContents = question.QuestionContents,
            QuestionSupports = questionSupports?
                    .Where(qs => question.QuestionSupports.Contains(qs.Id))
                    .Select(qs => qs.ToDto())
                    .ToList() ?? [],
            Choices = question.Choices,
            CreatedAt = question.CreatedAt
        };
    }

    public static QuestionDto ToDto(
        this Question question,
        PublicNotice? publicNotice = null,
        QuestionExam? questionExam = null,
        List<QuestionSupport>? questionSupports = null)
    {
        var exam = questionExam ?? question.Exams.FirstOrDefault();

        if (exam == null)
        {
            throw new InvalidOperationException($"Questão {question.Id} não possui exames associados.");
        }

        var allPositions = question.Exams
            .Where(qe => !string.IsNullOrWhiteSpace(qe.Position))
            .Select(qe => qe.Position!)
            .Distinct()
            .ToList();

        return new QuestionDto
        {
            QuestionId = question.Id,
            PublicNoticeId = exam.PublicNoticeId,
            ExamId = exam.ExamId,
            PublicNoticeNumber = publicNotice?.Number ?? null,
            Year = exam.Year,
            ExaminerOrganization = exam.ExaminerOrganization,
            ContractingOrganization = exam.ContractingOrganization,
            ExamCategory = exam.ExamCategory,
            Phase = exam.Phase ?? string.Empty,
            Positions = allPositions,
            Area = exam.Area ?? string.Empty,
            EducationLevel = exam.EducationLevel ?? string.Empty,
            PublicNoticeFileUrl = publicNotice?.FileUrl ?? string.Empty,
            ExamBookletUrl = exam.ExamBookletUrl ?? string.Empty,
            AnswerKeyUrl = exam.AnswerKeyUrl ?? string.Empty,
            IsNullified = question.IsNullified,
            QuestionNumber = exam.QuestionNumber,
            QuestionType = question.Type,
            MainArea = question.MainArea,
            SubAreas = question.SubAreas,
            QuestionContents = question.QuestionContents,
            QuestionSupports = questionSupports?
                    .Where(qs => question.QuestionSupports.Contains(qs.Id))
                    .Select(qs => qs.ToDto())
                    .ToList() ?? [],
            Choices = question.Choices,
            CreatedAt = question.CreatedAt
        };
    }
}
