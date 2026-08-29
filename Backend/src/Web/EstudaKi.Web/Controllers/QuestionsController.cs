using Estudaki.Commons.Core.CQRS;
using Estudaki.Modules.Questions.Application.DTOs;
using Estudaki.Modules.Questions.Application.Queries.SearchQuestions;
using Estudaki.Modules.Questions.Domain.Common;
using Estudaki.Modules.Questions.Domain.Repositories;
using Estudaki.Modules.Questions.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EstudaKi.Web.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
public class QuestionsController (ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IQuestionRepository questionRepository, IQuestionSupportRepository questionSupportRepository) : Controller
{
    public ICommandDispatcher CommandDispatcher = commandDispatcher;
    public IQueryDispatcher QueryDispatcher = queryDispatcher;

    private readonly IQuestionRepository _questionRepository = questionRepository;
    private readonly IQuestionSupportRepository _questionSupportRepository = questionSupportRepository;

    [HttpGet]
    public async Task<IActionResult> GetQuestions([FromQuery] SearchParameters query)
    {
        var queryRequest = new SearchQuestionsPaginatedQuery(query);
        var result = await QueryDispatcher.DispatchAsync<SearchQuestionsPaginatedQuery, PagedResult<QuestionDto>>(queryRequest);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestion()
    {
        //var questions = await _questionRepository.GetAll();

        //var cont = 0;
        //foreach (var question in questions)
        //{
        //    if (question.QuestionSupports == null || !question.QuestionSupports.Any())
        //    {
        //        question.QuestionSupports = null;
        //    }


        //    cont++;
        //    foreach (var content in question.QuestionContents)
        //    {
        //        if (content is ParagraphBlock)
        //        {
        //            string text = string.Empty;

        //            if (((ParagraphBlock)content).Inlines != null)
        //            {
        //                foreach (var inline in ((ParagraphBlock)content).Inlines)
        //                {
        //                    if (inline is TextInline textInline)
        //                    {
        //                        text += textInline.Text;
        //                    }
        //                }
        //                ((ParagraphBlock)content).Text = text;
        //                ((ParagraphBlock)content).Inlines = null;
        //            }
        //        }
        //    }

        //    if (question.Choices != null)
        //    {
        //        foreach (var choice in question.Choices)
        //        {
        //            if (choice.Content != null)
        //            {
        //                var contentBlocks = new List<ContentBlock>();
        //                string text = string.Empty;
        //                foreach (var contentBlock in choice.Content)
        //                {
        //                    if (contentBlock is TextInline textInline)
        //                    {


        //                        if (textInline.Text != null)
        //                        {
        //                            text += textInline.Text;
        //                        }
        //                    }
        //                }

        //                if (!string.IsNullOrEmpty(text))
        //                {
        //                    var paragraphBlock = new ParagraphBlock
        //                    {
        //                        Inlines = null,
        //                        Text = text
        //                    };
        //                    contentBlocks.Add(paragraphBlock);
        //                    choice.Content = null;
        //                    choice.ContentBlocks = contentBlocks;
        //                }
        //            }
        //        }
        //    }

        //    Console.WriteLine("updated {0} of {1}", cont, questions.Count());
        //    await _questionRepository.Update(question);
        //}

        var supports = await _questionSupportRepository.GetAll();

        foreach (var support in supports)
        {
            if (support.Contents != null)
            {
                foreach (var content in support.Contents)
                {
                    if (content is ParagraphBlock)
                    {
                        string text = string.Empty;
                        if (((ParagraphBlock)content).Inlines != null)
                        {
                            foreach (var inline in ((ParagraphBlock)content).Inlines)
                            {
                                if (inline is TextInline textInline)
                                {
                                    text += textInline.Text;
                                }
                            }
                            ((ParagraphBlock)content).Text = text;
                            ((ParagraphBlock)content).Inlines = null;
                        }
                    }
                }
            }
            await _questionSupportRepository.Update(support);
        }

        return Ok();
    }
}
