//using Estudaki.Modules.Questions.Domain.Entities;
//using Estudaki.Modules.Questions.Domain.Entities2;
//using Estudaki.Modules.Questions.Domain.Repositories;
//using Estudaki.Modules.Questions.Domain.Repositories2;
//using Estudaki.Modules.Questions.Domain.ValueObjects;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace EstudaKi.Controllers;

//public class MigrationResult
//{
//    public int PublicNoticesMigrated { get; set; }
//    public int ExamsCreated { get; set; }
//    public int QuestionsMigrated { get; set; }
//    public int ExamQuestionsCreated { get; set; }
//    public List<string> Errors { get; set; } = new();
//}

//[Route("api/[controller]")]
//[ApiController]
//public class MigrationController : ControllerBase
//{
//    private readonly IPublicNoticeRepository _oldPublicNoticeRepository;
//    private readonly IQuestionRepository _oldQuestionRepository;
//    private readonly IPublicNoticeRepository _newPublicNoticeRepository;
//    private readonly IQuestionRepository _newQuestionRepository;
//    private readonly IExamQuestionRepository _examQuestionRepository;
//    private readonly ILogger<MigrationController> _logger;

//    public MigrationController(
//        IPublicNoticeRepository oldPublicNoticeRepository,
//        IQuestionRepository oldQuestionRepository,
//        IPublicNoticeRepository newPublicNoticeRepository,
//        IQuestionRepository newQuestionRepository,
//        IExamQuestionRepository examQuestionRepository,
//        ILogger<MigrationController> logger)
//    {
//        _oldPublicNoticeRepository = oldPublicNoticeRepository;
//        _oldQuestionRepository = oldQuestionRepository;
//        _newPublicNoticeRepository = newPublicNoticeRepository;
//        _newQuestionRepository = newQuestionRepository;
//        _examQuestionRepository = examQuestionRepository;
//        _logger = logger;
//    }

//    [HttpPost("migrate")]
//    [IgnoreAntiforgeryToken]
//    public async Task<IActionResult> MigrateData()
//    {
//        try
//        {
//            _logger.LogInformation("=== INICIANDO MIGRAÇÃO DE DADOS ===");

//            var migrationResult = new MigrationResult();

//            // Buscar todos os editais antigos
//            var oldPublicNotices = await _oldPublicNoticeRepository.GetAll();
//            _logger.LogInformation($"Encontrados {oldPublicNotices.Count()} editais para migrar");

//            foreach (var oldPublicNotice in oldPublicNotices)
//            {
//                try
//                {
//                    // Criar um Exam baseado nos dados do PublicNotice antigo
//                    var exam = new Exam
//                    {
//                        Phase = oldPublicNotice.ExamPhase ?? string.Empty,
//                        Position = oldPublicNotice.Position ?? string.Empty,
//                        Area = string.Empty,
//                        EducationLevel = string.Empty,
//                        ExamBookletUrl = string.Empty,
//                        AnswerKeyUrl = string.Empty,
//                        AnswerKeyItems = new List<AnswerKeyItem>()
//                    };

//                    // Criar novo PublicNotice com o Exam já incluído
//                    var newPublicNotice = new PublicNotice
//                    {
//                        Id = oldPublicNotice.Id, // Manter o mesmo ID
//                        Number = oldPublicNotice.Number,
//                        Year = oldPublicNotice.Year,
//                        ExaminerOrganization = oldPublicNotice.ExamBoard,
//                        ContractingOrganization = oldPublicNotice.ExamRequester,
//                        ExamCategory = oldPublicNotice.ExamCategory,
//                        IsReviewed = oldPublicNotice.IsReviewed,
//                        IsPublished = oldPublicNotice.IsPublished,
//                        CreatedAt = oldPublicNotice.CreatedAt,
//                        Exams = new List<Exam> { exam }
//                    };

//                    migrationResult.ExamsCreated++;

//                    _logger.LogInformation($"Edital {oldPublicNotice.Id} - Exam criado com ID {exam.Id}");

//                    // Migrar questões relacionadas a este edital
//                    var oldQuestions = await _oldQuestionRepository.GetByPublicNoticeId(oldPublicNotice.Id);
//                    _logger.LogInformation($"Encontradas {oldQuestions.Count} questões para o edital {oldPublicNotice.Id}");

//                    foreach (var oldQuestion in oldQuestions)
//                    {
//                        try
//                        {
//                            // Criar nova Question
//                            var newQuestion = new Question2
//                            {
//                                Id = oldQuestion.Id, // Manter o mesmo ID
//                                CreatedAt = oldQuestion.CreatedAt,
//                                IsPublished = oldQuestion.IsPublished,
//                                Number = oldQuestion.QuestionNumber,
//                                Type = oldQuestion.Type,
//                                MainArea = oldQuestion.MainArea,
//                                SubAreas = oldQuestion.SubAreas,
//                                QuestionSupports = oldQuestion.QuestionSupports,
//                                QuestionContents = oldQuestion.QuestionContents,
//                                Choices = oldQuestion.Choices
//                            };

//                            // Salvar a nova Question
//                            _newQuestionRepository.Add(newQuestion);
//                            migrationResult.QuestionsMigrated++;

//                            // Criar ExamQuestion (relacionamento)
//                            var examQuestion = new Estudaki.Modules.Questions.Domain.Entities2.ExamQuestion
//                            {
//                                ExamId = exam.Id,
//                                QuestionId = newQuestion.Id,
//                                IsNullified = oldQuestion.IsNullified ?? false,
//                                QuestionNumber = oldQuestion.QuestionNumber
//                            };

//                            // Salvar o ExamQuestion
//                            _examQuestionRepository.Add(examQuestion);
//                            migrationResult.ExamQuestionsCreated++;

//                            _logger.LogInformation($"Questão {oldQuestion.Id} migrada com sucesso");
//                        }
//                        catch (Exception ex)
//                        {
//                            var errorMsg = $"Erro ao migrar questão {oldQuestion.Id}: {ex.Message}";
//                            _logger.LogError(ex, errorMsg);
//                            migrationResult.Errors.Add(errorMsg);
//                        }
//                    }

//                    // Salvar o novo PublicNotice com todos os Exams
//                    _newPublicNoticeRepository.Add(newPublicNotice);
//                    migrationResult.PublicNoticesMigrated++;

//                    _logger.LogInformation($"Edital {oldPublicNotice.Id} migrado com sucesso");
//                }
//                catch (Exception ex)
//                {
//                    var errorMsg = $"Erro ao migrar edital {oldPublicNotice.Id}: {ex.Message}";
//                    _logger.LogError(ex, errorMsg);
//                    migrationResult.Errors.Add(errorMsg);
//                }
//            }

//            _logger.LogInformation("=== MIGRAÇÃO CONCLUÍDA ===");
//            _logger.LogInformation($"Editais migrados: {migrationResult.PublicNoticesMigrated}");
//            _logger.LogInformation($"Exames criados: {migrationResult.ExamsCreated}");
//            _logger.LogInformation($"Questões migradas: {migrationResult.QuestionsMigrated}");
//            _logger.LogInformation($"ExamQuestions criados: {migrationResult.ExamQuestionsCreated}");
//            _logger.LogInformation($"Erros: {migrationResult.Errors.Count}");

//            return Ok(migrationResult);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Erro crítico durante a migração");
//            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
//        }
//    }

//    [HttpGet("status")]
//    public async Task<IActionResult> GetMigrationStatus()
//    {
//        try
//        {
//            var oldPublicNotices = await _oldPublicNoticeRepository.GetAll();
//            var newPublicNotices = await _newPublicNoticeRepository.GetAll();
//            var oldQuestions = await _oldQuestionRepository.GetAll();
//            var newQuestions = await _newQuestionRepository.GetAll();

//            return Ok(new
//            {
//                OldData = new
//                {
//                    PublicNotices = oldPublicNotices.Count(),
//                    Questions = oldQuestions.Count()
//                },
//                NewData = new
//                {
//                    PublicNotices = newPublicNotices.Count(),
//                    Questions = newQuestions.Count()
//                }
//            });
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Erro ao obter status da migração");
//            return StatusCode(500, new { error = ex.Message });
//        }
//    }
//}
