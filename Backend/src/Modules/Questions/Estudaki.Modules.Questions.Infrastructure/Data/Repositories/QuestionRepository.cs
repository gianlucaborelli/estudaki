using System.Text.RegularExpressions;
using Estudaki.Commons.Core.Data.Context;
using Estudaki.Commons.Core.Data.Repository;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Entities;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Estudaki.Modules.Questions.Infrastructure.Data.Repositories;

public class QuestionRepository : MongoRepositoryBase<Question>, IQuestionRepository
{
    public QuestionRepository(IMongoContext context) : base(context)
    {
    }

    public async Task<FilterParameters> FindFilterParametersAsync()
    {
        var filterBuilder = Builders<Question>.Filter;
        var baseFilter = filterBuilder.Eq(q => q.IsPublished, true);

        // Executar todas as queries em paralelo
        var typeQuestionsTask = DbSet
            .Distinct(x => x.Type, baseFilter)
            .ToListAsync();

        var mainAreasTask = DbSet
            .Distinct(x => x.MainArea, baseFilter)
            .ToListAsync();

        var subAreasTask = DbSet
            .Find(baseFilter)
            .Project(q => q.SubAreas)
            .ToListAsync();

        var examsTask = DbSet
            .Find(baseFilter)
            .Project(q => q.Exams)
            .ToListAsync();

        await Task.WhenAll(typeQuestionsTask, mainAreasTask, subAreasTask, examsTask);

        var typeQuestions = await typeQuestionsTask;
        var mainAreas = await mainAreasTask;
        var allSubAreas = await subAreasTask;
        var questionsWithExams = await examsTask;

        var subAreas = allSubAreas
            .SelectMany(sa => sa)
            .Distinct()
            .Where(sa => !string.IsNullOrWhiteSpace(sa))
            .OrderBy(sa => sa)
            .ToArray();

        var examCategories = questionsWithExams
            .SelectMany(exams => exams)
            .Select(qe => qe.ExamCategory)
            .Where(ec => !string.IsNullOrWhiteSpace(ec))
            .Distinct()
            .OrderBy(ec => ec)
            .ToArray();

        var examContractingOrganizations = questionsWithExams
            .SelectMany(exams => exams)
            .Select(qe => qe.ContractingOrganization)
            .Where(ec => ec is not null && !string.IsNullOrWhiteSpace(ec))
            .Distinct()
            .OrderBy(ec => ec)
            .ToArray();

        var examExaminerOrganizations = questionsWithExams
            .SelectMany(exams => exams)
            .Select(qe => qe.ExaminerOrganization)
            .Where(ec => ec is not null && !string.IsNullOrWhiteSpace(ec))
            .Distinct()
            .OrderBy(ec => ec)
            .ToArray();

        var examYears = questionsWithExams
            .SelectMany(exams => exams)
            .Select(qe => qe.Year)
            .Distinct()
            .OrderBy(ey => ey)
            .ToArray();

        return new FilterParameters
        {
            TypeQuestions = typeQuestions
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .OrderBy(t => t)
                .ToArray(),
            ExamCategories = examCategories,
            ContractingOrganization =
                [
                    .. examContractingOrganizations?
                        .Where(x => x is not null)
                        .Select(x => x!)
                        ?? []
                ],
            ExaminerOrganization =
                [
                    .. examExaminerOrganizations?
                        .Where(x => x is not null)
                        .Select(x => x!)
                        ?? []
                ],
            Year = examYears,
            MainAreas = mainAreas
                .Where(ma => !string.IsNullOrWhiteSpace(ma))
                .Distinct()
                .OrderBy(ma => ma)
                .ToArray(),
            SubAreas = subAreas
        };
    }

    public async Task<(List<Question> Questions, long TotalCount)> FindQuestionsPaginatedAsync(FilterParameters searchParameter)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        // Filtro de publicação        
        filters.Add(filterBuilder.Eq(q => q.IsPublished, true));

        // Filtro de texto (busca no conteúdo)
        if (!string.IsNullOrWhiteSpace(searchParameter.WordKey))
        { 
            var words = searchParameter.WordKey
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var wordFilters = words
                .Select(word =>
                    filterBuilder.Regex(
                        q => q.Statement,
                        new BsonRegularExpression(
                            Regex.Escape(word),
                            "i")))
                .ToList();

            filters.Add(filterBuilder.Or(wordFilters));
        }

        // Filtro de tipo de questão - CORRIGIDO: normaliza e valida os valores
        if (searchParameter.TypeQuestions is { Length: > 0 })
        {
            var validTypes = searchParameter.TypeQuestions
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .ToList();

            if (validTypes.Count > 0)
            {
                filters.Add(filterBuilder.In(q => q.Type, validTypes));
            }
        }

        // Filtro de área principal - CORRIGIDO: normaliza e valida os valores
        if (searchParameter.MainAreas is { Length: > 0 })
        {
            var validAreas = searchParameter.MainAreas
                .Where(ma => !string.IsNullOrWhiteSpace(ma))
                .Select(ma => ma.Trim())
                .ToList();

            if (validAreas.Count > 0)
            {
                filters.Add(filterBuilder.In(q => q.MainArea, validAreas));
            }
        }

        // Filtro de sub-áreas
        if (searchParameter.SubAreas is { Length: > 0 })
        {
            var validSubAreas = searchParameter.SubAreas
                .Where(sa => !string.IsNullOrWhiteSpace(sa))
                .Select(sa => sa.Trim())
                .ToList();

            if (validSubAreas.Count > 0)
            {
                filters.Add(filterBuilder.AnyIn(q => q.SubAreas, validSubAreas));
            }
        }

        // Filtro de categoria de exame
        if (searchParameter.ExamCategories is { Length: > 0 })
        {
            var validCategories = searchParameter.ExamCategories
                .Where(ec => !string.IsNullOrWhiteSpace(ec))
                .Select(ec => ec.Trim())
                .ToList();

            if (validCategories.Count > 0)
            {
                var examFilter = filterBuilder.ElemMatch(
                    q => q.Exams,
                    Builders<QuestionExam>.Filter.In(qe => qe.ExamCategory, validCategories)
                );
                filters.Add(examFilter);
            }
        }

        // Filtro por contratante do exame
        if (searchParameter.ContractingOrganization is { Length: > 0 })
        {
            var validOrganizations = searchParameter.ContractingOrganization
                .Where(org => !string.IsNullOrWhiteSpace(org))
                .Select(org => org.Trim())
                .ToList();

            if (validOrganizations.Count > 0)
            {
                var examFilter = filterBuilder.ElemMatch(
                    q => q.Exams,
                    Builders<QuestionExam>.Filter.In(qe => qe.ContractingOrganization, validOrganizations)
                );
                filters.Add(examFilter);
            }
        }

        // Filtro por organizador do exame
        if (searchParameter.ExaminerOrganization is { Length: > 0 })
        {
            var validOrganizations = searchParameter.ExaminerOrganization
                .Where(org => !string.IsNullOrWhiteSpace(org))
                .Select(org => org.Trim())
                .ToList();

            if (validOrganizations.Count > 0)
            {
                var examFilter = filterBuilder.ElemMatch(
                    q => q.Exams,
                    Builders<QuestionExam>.Filter.In(qe => qe.ExaminerOrganization, validOrganizations)
                );
                filters.Add(examFilter);
            }
        }

        // Filtro por Ano do exame
        if (searchParameter.Year is { Length: > 0 })
        {
            var examFilter = filterBuilder.ElemMatch(
                q => q.Exams,
                Builders<QuestionExam>.Filter.In(qe => qe.Year, searchParameter.Year)
            );
            filters.Add(examFilter);
        }

        var finalFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        // Contar total de itens
        var totalItems = await DbSet.CountDocumentsAsync(finalFilter);

        // Buscar questões paginadas
        var questions = await DbSet.Find(finalFilter)
            .Skip((searchParameter.Page - 1) * searchParameter.PageSize)
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

    public async Task<(List<Question> Questions, long TotalCount)> GetByExamIdPaged(
    string examId,
    int page,
    int pageSize,
    string? sortLabel,
    string? sortDirection)
    {
        var filterBuilder = Builders<Question>.Filter;

        var filter = filterBuilder.ElemMatch(
            q => q.Exams,
            Builders<QuestionExam>.Filter.Eq(qe => qe.ExamId, examId)
        );

        var skip = (page - 1) * pageSize;

        // Conta todas as questões que correspondem ao filtro.
        var totalCount = await DbSet.CountDocumentsAsync(filter);

        // Define a direção da ordenação.
        var direction = string.Equals(
            sortDirection,
            "desc",
            StringComparison.OrdinalIgnoreCase)
            ? -1
            : 1;

        var questions = await DbSet.Aggregate()
            .Match(filter)
            .AppendStage<Question>(
                new BsonDocument("$addFields",
                    new BsonDocument("__questionNumber",
                        new BsonDocument("$let",
                            new BsonDocument
                            {
                            {
                                "vars",
                                new BsonDocument("matchingExam",
                                    new BsonDocument("$arrayElemAt",
                                        new BsonArray
                                        {
                                            new BsonDocument("$filter",
                                                new BsonDocument
                                                {
                                                    {
                                                        "input", "$Exams"
                                                    },
                                                    {
                                                        "as", "exam"
                                                    },
                                                    {
                                                        "cond",
                                                        new BsonDocument("$eq",
                                                            new BsonArray
                                                            {
                                                                "$$exam.ExamId",
                                                                examId
                                                            })
                                                    }
                                                }),
                                            0
                                        }))
                            },
                            {
                                "in", "$$matchingExam.QuestionNumber"
                            }
                            })))
            )
            .Sort(new BsonDocument(
                "__questionNumber",
                direction))
            .Skip(skip)
            .Limit(pageSize)
            .As<Question>()
            .ToListAsync();

        return (questions, totalCount);
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

    public async Task<List<(string QuestionId, DateTime CreatedAt)>> GetPublishedQuestionsForSitemapAsync()
    {
        var filterBuilder = Builders<Question>.Filter;
        var filter = filterBuilder.Eq(q => q.IsPublished, true);

        var projectionBuilder = Builders<Question>.Projection;
        var projection = projectionBuilder
            .Include(q => q.Id)
            .Include(q => q.CreatedAt);

        var result = await DbSet
            .Find(filter)
            .Project<BsonDocument>(projection)
            .ToListAsync();

        var sitemapdatas = result
            .Select(doc => (
                QuestionId: doc["_id"].AsObjectId.ToString(),
                CreatedAt: doc["CreatedAt"].ToUniversalTime()
            ))
            .ToList();

        return sitemapdatas;
    }
}
