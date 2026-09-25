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
            TypeQuestions = typeQuestions.Where(t => !string.IsNullOrWhiteSpace(t)).OrderBy(t => t).ToArray(),
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
            MainAreas = mainAreas.Where(ma => !string.IsNullOrWhiteSpace(ma)).OrderBy(ma => ma).ToArray(),
            SubAreas = subAreas
        };
    }

    public async Task<(List<Question> Questions, long TotalCount)> FindQuestionsPaginatedAsync(FilterParameters searchParameter)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filters = new List<FilterDefinition<Question>>();

        // Filtro de publicação        
        filterBuilder.Eq(q => q.IsPublished, true);        

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

        // Filtro de categoria de exame
        if (searchParameter.ExamCategories is { Length: > 0 })
        {
            var examFilter = filterBuilder.ElemMatch(
                q => q.Exams,
                Builders<QuestionExam>.Filter.In(qe => qe.ExamCategory, searchParameter.ExamCategories)
            );
            filters.Add(examFilter);
        }

        // Filtro por contratante do exame
        if (searchParameter.ContractingOrganization is { Length: > 0 })
        {
            var examFilter = filterBuilder.ElemMatch(
                q => q.Exams,
                Builders<QuestionExam>.Filter.In(qe => qe.ContractingOrganization, searchParameter.ContractingOrganization)
            );
            filters.Add(examFilter);
        }

        // Filtro por organizador do exame
        if (searchParameter.ExaminerOrganization is { Length: > 0 })
        {
            var examFilter = filterBuilder.ElemMatch(
                q => q.Exams,
                Builders<QuestionExam>.Filter.In(qe => qe.ExaminerOrganization, searchParameter.ExaminerOrganization)
            );
            filters.Add(examFilter);
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
            .Skip((searchParameter.Page) * searchParameter.PageSize)
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

    public async Task<(List<Question> Questions, long TotalCount)> GetByExamIdPaged(string examId, int page, int pageSize, string? sortLabel, string? sortDirection)
    {
        var filterBuilder = Builders<Question>.Filter;
        var filter = filterBuilder.ElemMatch(
            q => q.Exams,
            Builders<QuestionExam>.Filter.Eq(qe => qe.ExamId, examId)
        );
        var sort = GetSortDefinition<Question>(sortLabel, sortDirection);

        var questions = await DbSet
            .Find(filter)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        var totalCount = await DbSet.CountDocumentsAsync(filter);
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
}
