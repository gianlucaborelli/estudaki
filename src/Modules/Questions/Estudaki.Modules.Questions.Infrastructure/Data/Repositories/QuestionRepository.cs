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

    public async Task<FilterParameters> FindFilterParametersAsync(FilterParameters filterParameters)
    {
        // Implementation placeholder - you'll need to implement this based on your logic
        return await Task.FromResult(filterParameters);
    }

    public async Task<(Dictionary<ExamQuestion, Question> QuestionsWithExam, long TotalCount)> FindQuestionsPaginatedAsync(SearchParameters searchParameter)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        // 1. Filtro de publicação
        if (searchParameter.IsPublished)
        {
            filters.Add(filterBuilder.Eq(q => q.IsPublished, true));
        }

        // 2. Filtro por WordKey - busca em todos os textos do conteúdo da questão
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

        // 3. Filtro por tipo de questão
        if (searchParameter.TypeQuestions is { Length: > 0 })
        {
            filters.Add(filterBuilder.In(q => q.Type, searchParameter.TypeQuestions));
        }

        // 4. Filtro por MainAreas
        if (searchParameter.MainAreas is { Length: > 0 })
        {
            filters.Add(filterBuilder.In(q => q.MainArea, searchParameter.MainAreas));
        }

        // 5. Filtro por SubAreas
        if (searchParameter.SubAreas is { Length: > 0 })
        {
            filters.Add(filterBuilder.AnyIn(q => q.SubAreas, searchParameter.SubAreas));
        }

        // 6. Filtro por ExamCategories - precisa buscar ExamQuestion -> Exam -> PublicNotice
        if (searchParameter.ExamCategories is { Length: > 0 })
        {
            // 6.1. Buscar PublicNotices que tenham a categoria desejada e estejam publicados
            var publicNoticeBuilder = Builders<PublicNotice>.Filter;
            var categoryFilter = publicNoticeBuilder.And(
                publicNoticeBuilder.In(pn => pn.ExamCategory, searchParameter.ExamCategories),
                publicNoticeBuilder.Eq(pn => pn.IsPublished, true)
            );

            var publicNoticesCollection = Context.GetCollection<PublicNotice>();
            var publicNotices = await publicNoticesCollection
                .Find(categoryFilter)
                .ToListAsync();

            // 6.2. Extrair todos os ExamIds dos Exams dentro dos PublicNotices
            var examIds = publicNotices
                .SelectMany(pn => pn.Exams)
                .Select(e => e.Id)
                .ToList();

            if (examIds.Any())
            {
                // 6.3. Buscar ExamQuestions que referenciam esses ExamIds
                var examQuestionsCollection = Context.GetCollection<ExamQuestion>();
                var examQuestionsFilter = Builders<ExamQuestion>.Filter.In(eq => eq.ExamId, examIds);
                var filteredQuestionIds = await examQuestionsCollection
                    .Find(examQuestionsFilter)
                    .Project(eq => eq.QuestionId)
                    .ToListAsync();

                if (filteredQuestionIds.Any())
                {
                    // 6.4. Filtrar questões pelos IDs encontrados
                    filters.Add(filterBuilder.In(q => q.Id, filteredQuestionIds));
                }
                else
                {
                    // Se não há questões com essa categoria, retornar vazio
                    return (new Dictionary<ExamQuestion, Question>(), 0);
                }
            }
            else
            {
                // Se não há editais com essa categoria, retornar vazio
                return (new Dictionary<ExamQuestion, Question>(), 0);
            }
        }

        // 7. Construir filtro final
        var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        // 8. Contar total de questões que atendem os critérios
        var totalItems = await DbSet.CountDocumentsAsync(finalFilter);

        // 9. Buscar questões com paginação
        var items = await DbSet.Find(finalFilter)
            .Skip((searchParameter.CurrentPage - 1) * searchParameter.PageSize)
            .Limit(searchParameter.PageSize)
            .ToListAsync();

        // 10. Buscar ExamQuestions relacionadas às questões encontradas
        var foundQuestionIds = items.Select(q => q.Id).ToList();
        var examQuestionsCollectionFinal = Context.GetCollection<ExamQuestion>();
        var examQuestions = await examQuestionsCollectionFinal
            .Find(Builders<ExamQuestion>.Filter.In(eq => eq.QuestionId, foundQuestionIds))
            .ToListAsync();

        // 11. Criar dictionary com ExamQuestion como chave e Question como valor
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
