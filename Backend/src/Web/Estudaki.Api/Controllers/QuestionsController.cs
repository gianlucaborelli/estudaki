using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.GetFilterParameters;
using Estudaki.Modules.Questions.Application.Queries.GetQuestionById;
using Estudaki.Modules.Questions.Application.Queries.SearchQuestions;
using Estudaki.Modules.Questions.Application.Services;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estudaki.Api.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
public class QuestionsController (ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IQuestionRepository questionRepository, IQuestionSupportRepository questionSupportRepository, ContentMigrationService contentMigrationService) : Controller
{
    public ICommandDispatcher CommandDispatcher = commandDispatcher;
    public IQueryDispatcher QueryDispatcher = queryDispatcher;

    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;
    private readonly ContentMigrationService _contentMigrationService = contentMigrationService;

    [HttpGet]
    public async Task<IActionResult> GetQuestions([FromQuery] FilterParameters query)
    {
        var queryRequest = new SearchQuestionsPaginatedQuery(query);
        var result = await QueryDispatcher.DispatchAsync<SearchQuestionsPaginatedQuery, PagedResult<QuestionDto>>(queryRequest);
        return Ok(result);
    }

    [HttpGet("parameters")]
    public async Task<IActionResult> GetParameters([FromQuery] FilterParameters query)
    {
        var queryRequest = new GetFilterParametersQuery(query);
        var result = await QueryDispatcher.DispatchAsync<GetFilterParametersQuery, FilterParameters>(queryRequest);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuestionById(string id)
    {
        var queryRequest = new GetQuestionByIdQuery(id);
        var result = await QueryDispatcher.DispatchAsync<GetQuestionByIdQuery, QuestionDto?>(queryRequest);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion()
    {
        var questions = await _questionRepository.GetAll();

        var cont = 0;
        foreach (var question in questions)
        {
            // Migra conteúdo da questão (QuestionContents -> Statement)
            if (question.QuestionContents != null && question.QuestionContents.Any())
            {
                var statementHtml = _contentMigrationService.MigrateQuestionContentsToStatement(question.QuestionContents);
                if (!string.IsNullOrEmpty(statementHtml))
                {
                    question.Statement = statementHtml;
                }
            }

            // Migra conteúdo das alternativas (Content/ContentBlocks -> Explanation)
            if (question.Choices != null)
            {
                foreach (var choice in question.Choices)
                {
                    var explanationHtml = _contentMigrationService.MigrateChoiceContentToExplanation(choice);
                    if (!string.IsNullOrEmpty(explanationHtml))
                    {
                        choice.Explanation = explanationHtml;
                    }
                }
            }

            cont++;
            await _questionRepository.Update(question);
        }

        Console.WriteLine("Migradas {0} questões", cont);

        // Migra conteúdo dos suportes (Contents -> Content)
        var supports = await _questionSupportRepository.GetAll();

        foreach (var support in supports)
        {
            if (support.Contents != null && support.Contents.Any())
            {
                var contentHtml = _contentMigrationService.MigrateQuestionSupportContentsToContent(support.Contents);
                if (!string.IsNullOrEmpty(contentHtml))
                {
                    support.Content = contentHtml;
                }
            }

            await _questionSupportRepository.Update(support);
        }

        Console.WriteLine("Migrados {0} suportes", supports.Count());

        return Ok(new { questions = cont, supports = supports.Count() });
    }
}
