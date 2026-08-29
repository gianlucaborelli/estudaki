using Estudaki.Modules.Questions.Domain.Entities;

namespace Estudaki.Modules.Questions.Domain.Extensions
{
    public static class ExamExtensions
    {
        public static Exam Clone(this Exam exam)
        {
            if (exam == null) return null;
    
            return new Exam
            {
                Id = exam.Id,
                ExamBookletUrl = exam.ExamBookletUrl,
                AnswerKeyUrl = exam.AnswerKeyUrl,
                EducationLevel = exam.EducationLevel,                    
                Position = exam.Position,
                Area = exam.Area,
                Phase = exam.Phase,
                AnswerKeyItems = exam.AnswerKeyItems
            };
        }
    }
}
