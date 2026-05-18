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

        // Buscar tipos de questões
        var typeQuestions = await DbSet
            .Distinct(x => x.Type, baseFilter)
            .ToListAsync();

        // Buscar áreas principais
        var mainAreas = await DbSet
            .Distinct(x => x.MainArea, baseFilter)
            .ToListAsync();

        // Buscar sub-áreas
        var allQuestions = await DbSet
            .Find(baseFilter)
            .Project(q => q.SubAreas)
            .ToListAsync();

        var subAreas = allQuestions
            .SelectMany(sa => sa)
            .Distinct()
            .OrderBy(sa => sa)
            .ToArray();

        // Buscar categorias de exames diretamente das questões (dados desnormalizados)
        var questionsWithExams = await DbSet
            .Find(baseFilter)
            .Project(q => q.Exams)
            .ToListAsync();

        var examCategories = questionsWithExams
            .SelectMany(exams => exams)
            .Select(qe => qe.ExamCategory)
            .Where(ec => !string.IsNullOrWhiteSpace(ec))
            .Distinct()
            .OrderBy(ec => ec)
            .ToArray();

        return new FilterParameters
        {
            TypeQuestions = typeQuestions.Where(t => !string.IsNullOrWhiteSpace(t)).OrderBy(t => t).ToArray(),
            ExamCategories = examCategories,
            MainAreas = mainAreas.Where(ma => !string.IsNullOrWhiteSpace(ma)).OrderBy(ma => ma).ToArray(),
            SubAreas = subAreas
        };
    }

    public async Task<(List<Question> Questions, long TotalCount)> FindQuestionsPaginatedAsync(SearchParameters searchParameter)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        // Filtro de publicação
        if (searchParameter.IsPublished)
        {
            filters.Add(filterBuilder.Eq(q => q.IsPublished, true));
        }

        // Filtro de texto (busca no conteúdo)
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

        // Filtro de tipo de questão
        if (searchParameter.TypeQuestions is { Length: > 0 })
        {
            filters.Add(filterBuilder.In(q => q.Type, searchParameter.TypeQuestions));
        }

        // Filtro de área principal
        if (searchParameter.MainAreas is { Length: > 0 })
        {
            filters.Add(filterBuilder.In(q => q.MainArea, searchParameter.MainAreas));
        }

        // Filtro de sub-áreas
        if (searchParameter.SubAreas is { Length: > 0 })
        {
            filters.Add(filterBuilder.AnyIn(q => q.SubAreas, searchParameter.SubAreas));
        }

        // Filtro de categoria de exame (usando dados desnormalizados)
        if (searchParameter.ExamCategories is { Length: > 0 })
        {
            var examFilter = filterBuilder.ElemMatch(
                q => q.Exams,
                Builders<QuestionExam>.Filter.In(qe => qe.ExamCategory, searchParameter.ExamCategories)
            );
            filters.Add(examFilter);
        }

        var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        // Contar total de itens
        var totalItems = await DbSet.CountDocumentsAsync(finalFilter);

        // Buscar questões paginadas
        var questions = await DbSet.Find(finalFilter)
            .Skip((searchParameter.CurrentPage - 1) * searchParameter.PageSize)
            .Limit(searchParameter.PageSize)
            .ToListAsync();

        return (questions, totalItems);
    }

    public async Task<List<Question>> GetByExamId(string examId)
    {
        var filterBuilder = Builders<Question>.Filter;

        // Buscar questões que contenham este examId no array Exams
        var filter = filterBuilder.ElemMatch(
            q => q.Exams,
            Builders<QuestionExam>.Filter.Eq(qe => qe.ExamId, examId)
        );

        var questions = await DbSet
            .Find(filter)
            .ToListAsync();

        return questions;
    }

    public async Task<List<Question>> GetByPublicNoticeId(string publicNoticeId)
    {
        var filterBuilder = Builders<Question>.Filter;
        
        var filter = filterBuilder.ElemMatch(
            q => q.Exams,
            Builders<QuestionExam>.Filter.Eq(qe => qe.PublicNoticeId, publicNoticeId)
        );

        var questions = await DbSet
            .Find(filter)
            .ToListAsync();

        return questions;
    }

    public async Task<List<Question>> GetManyById(List<string> questionIds)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filter = filterBuilder.In(q => q.Id, questionIds);

        var questions = await DbSet
            .Find(filter)
            .ToListAsync();

        return questions;
    }
}
