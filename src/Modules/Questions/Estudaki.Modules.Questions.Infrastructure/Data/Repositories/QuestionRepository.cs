using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class QuestionRepository : BaseRepository<Question>, IQuestionRepository
{
    public QuestionRepository(IMongoContext context) : base(context)
    {
    }

    public async Task<FilterParameters> FindFilterParametersAsync()
    {
        var filterBuilder = Builders<Question>.Filter;
        var baseFilter = filterBuilder.Eq(q => q.IsPublished, true);

        var typeQuestions = await DbSet
            .Distinct(x => x.Type, baseFilter)
            .ToListAsync();

        var mainAreas = await DbSet
            .Distinct(x => x.MainArea, baseFilter)
            .ToListAsync();

        var allQuestions = await DbSet
            .Find(baseFilter)
            .Project(q => q.SubAreas)
            .ToListAsync();

        var subAreas = allQuestions
            .SelectMany(sa => sa)
            .Distinct()
            .OrderBy(sa => sa)
            .ToArray();

        var publicNoticesCollection = Context.GetCollection<PublicNotice>();
        var publicNoticeFilter = Builders<PublicNotice>.Filter.Eq(pn => pn.IsPublished, true);

        var examCategories = await publicNoticesCollection
            .Distinct(x => x.ExamCategory, publicNoticeFilter)
            .ToListAsync();

        return new FilterParameters
        {
            TypeQuestions = typeQuestions.Where(t => !string.IsNullOrWhiteSpace(t)).OrderBy(t => t).ToArray(),
            ExamCategories = examCategories.Where(ec => !string.IsNullOrWhiteSpace(ec)).OrderBy(ec => ec).ToArray(),
            MainAreas = mainAreas.Where(ma => !string.IsNullOrWhiteSpace(ma)).OrderBy(ma => ma).ToArray(),
            SubAreas = subAreas
        };
    }

    public async Task<(Dictionary<ExamQuestion, Question> QuestionsWithExam, long TotalCount)> FindQuestionsPaginatedAsync(SearchParameters searchParameter)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        if (searchParameter.IsPublished)
        {
            filters.Add(filterBuilder.Eq(q => q.IsPublished, true));
        }

        if (!string.IsNullOrWhiteSpace(searchParameter.WordKey))
        {
            var textFilter = filterBuilder.ElemMatch(
                q => q.QuestionContents,
                Builders<ContentBlock>.Filter.OfType<ParagraphBlock>(
                    Builders<ParagraphBlock>.Filter.ElemMatch(
                        p => p.Inlines,
                        Builders<InlineContent>.Filter.OfType<TextInline>(
                            Builders<TextInline>.Filter.Regex(
                                t => t.Text,
                                new BsonRegularExpression(searchParameter.WordKey, "i")
                            )
                        )
                    )
                )
            );
            filters.Add(textFilter);
        }

        if (searchParameter.TypeQuestions is { Length: > 0 })
        {
            filters.Add(filterBuilder.In(q => q.Type, searchParameter.TypeQuestions));
        }

        if (searchParameter.MainAreas is { Length: > 0 })
        {
            filters.Add(filterBuilder.In(q => q.MainArea, searchParameter.MainAreas));
        }

        if (searchParameter.SubAreas is { Length: > 0 })
        {
            filters.Add(filterBuilder.AnyIn(q => q.SubAreas, searchParameter.SubAreas));
        }

        if (searchParameter.ExamCategories is { Length: > 0 })
        {
            var publicNoticeBuilder = Builders<PublicNotice>.Filter;
            var categoryFilter = publicNoticeBuilder.And(
                publicNoticeBuilder.In(pn => pn.ExamCategory, searchParameter.ExamCategories),
                publicNoticeBuilder.Eq(pn => pn.IsPublished, true)
            );

            var publicNoticesCollection = Context.GetCollection<PublicNotice>();
            var publicNotices = await publicNoticesCollection
                .Find(categoryFilter)
                .ToListAsync();

            var examIds = publicNotices
                .SelectMany(pn => pn.Exams)
                .Select(e => e.Id)
                .ToList();

            if (examIds.Any())
            {
                var examQuestionsCollection = Context.GetCollection<ExamQuestion>();
                var examQuestionsFilter = Builders<ExamQuestion>.Filter.In(eq => eq.ExamId, examIds);
                var filteredQuestionIds = await examQuestionsCollection
                    .Find(examQuestionsFilter)
                    .Project(eq => eq.QuestionId)
                    .ToListAsync();

                if (filteredQuestionIds.Any())
                {
                    filters.Add(filterBuilder.In(q => q.Id, filteredQuestionIds));
                }
                else
                {
                    return (new Dictionary<ExamQuestion, Question>(), 0);
                }
            }
            else
            {
                return (new Dictionary<ExamQuestion, Question>(), 0);
            }
        }

        var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        var totalItems = await DbSet.CountDocumentsAsync(finalFilter);

        var items = await DbSet.Find(finalFilter)
            .Skip((searchParameter.CurrentPage - 1) * searchParameter.PageSize)
            .Limit(searchParameter.PageSize)
            .ToListAsync();

        var foundQuestionIds = items.Select(q => q.Id).ToList();
        var examQuestionsCollectionFinal = Context.GetCollection<ExamQuestion>();
        var examQuestions = await examQuestionsCollectionFinal
            .Find(Builders<ExamQuestion>.Filter.In(eq => eq.QuestionId, foundQuestionIds))
            .ToListAsync();

        var result = new Dictionary<ExamQuestion, Question>();
        foreach (var examQuestion in examQuestions)
        {
            var question = items.FirstOrDefault(q => q.Id == examQuestion.QuestionId);
            if (question != null)
            {
                result[examQuestion] = question;
            }
        }

        return (result, totalItems);
    }

    public async Task<List<Question>> GetByExamId(string examId)
    {
        var questionIds = await Context.GetCollection<ExamQuestion>()
            .Find(x => x.ExamId == examId)
            .Project(x => x.QuestionId)
            .ToListAsync();

        var questions = await DbSet
            .Find(x => questionIds.Contains(x.Id))
            .ToListAsync();

        return questions;
    }
}
