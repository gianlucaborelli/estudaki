using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProvaOnline.Data.Context;
using ProvaOnline.Models;
using ProvaOnline.ModelsV2;

namespace ProvaOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly MongoContext _oldContext;
        private readonly MongoContextV2 _newContext;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _publicNoticeCache = new();

        public MigrationController(IConfiguration configuration)
        {
            _configuration = configuration;
            _oldContext = new MongoContext(configuration);
            _newContext = new MongoContextV2(configuration);
        }

        /// <summary>
        /// Migra todos os QuestionDocument para QuestionDocumentV2
        /// Endpoint: POST /api/migration/migrate-questions
        /// </summary>
        [HttpPost("migrate-questions")]
        public async Task<IActionResult> MigrateQuestions()
        {
            try
            {
                var oldCollection = _oldContext.GetCollection<QuestionDocument>("Questions");
                var newQuestionsCollection = _newContext.GetCollection<QuestionDocumentV2>("Questions");
                var publicNoticesCollection = _newContext.GetCollection<PublicNoticeDocumentV2>("PublicNotices");

                var oldQuestions = await oldCollection.Find(_ => true).ToListAsync();
                var migratedQuestions = new List<QuestionDocumentV2>();
                var errors = new List<string>();

                foreach (var oldQuestion in oldQuestions)
                {
                    try
                    {
                        var newQuestion = await ConvertToV2Async(oldQuestion, publicNoticesCollection);
                        migratedQuestions.Add(newQuestion);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Erro ao migrar questão {oldQuestion._id}: {ex.Message}");
                    }
                }

                if (migratedQuestions.Any())
                {
                    await newQuestionsCollection.InsertManyAsync(migratedQuestions);
                }

                return Ok(new
                {
                    TotalOldQuestions = oldQuestions.Count,
                    MigratedQuestions = migratedQuestions.Count,
                    TotalPublicNotices = _publicNoticeCache.Count,
                    Errors = errors,
                    Message = $"Migração concluída! {migratedQuestions.Count} de {oldQuestions.Count} questões migradas com {_publicNoticeCache.Count} editais."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Erro na migração: {ex.Message}" });
            }
        }

        private async Task<QuestionDocumentV2> ConvertToV2Async(
            QuestionDocument oldQuestion,
            IMongoCollection<PublicNoticeDocumentV2> publicNoticesCollection)
        {
            string? publicNoticeId = null;

            if (oldQuestion.PublicNotice != null)
            {
                publicNoticeId = await GetOrCreatePublicNoticeAsync(oldQuestion.PublicNotice, publicNoticesCollection);
            }

            var newQuestion = new QuestionDocumentV2
            {
                Id = oldQuestion._id.ToString(),
                PublicNoticeId = publicNoticeId,
                CreatedAt = oldQuestion.CreatedAt,
                IsPublished = oldQuestion.IsPublished,
                IsNullified = oldQuestion.IsNullified,
                QuestionNumber = oldQuestion.QuestionNumber,
                QuestionType = oldQuestion.QuestionType,
                MainArea = oldQuestion.MainArea,
                SubAreas = oldQuestion.SubAreas,
                QuestionContents = ConvertSupportsToContents(oldQuestion.QuestionBody, oldQuestion.Supports),
                Choices = ConvertChoices(oldQuestion.Choices)
            };

            return newQuestion;
        }

        private async Task<string> GetOrCreatePublicNoticeAsync(
            PublicNotice publicNotice,
            IMongoCollection<PublicNoticeDocumentV2> collection)
        {
            // Cria uma chave única para o PublicNotice baseada em suas propriedades
            var cacheKey = $"{publicNotice.Year}|{publicNotice.Number}|{publicNotice.ExamPhase}|{publicNotice.ExamBoard}|{publicNotice.Position}";

            // Verifica se já existe no cache
            if (_publicNoticeCache.TryGetValue(cacheKey, out var cachedId))
            {
                return cachedId;
            }

            // Busca no banco se já existe um PublicNotice com as mesmas propriedades
            var filter = Builders<PublicNoticeDocumentV2>.Filter.And(
                Builders<PublicNoticeDocumentV2>.Filter.Eq(x => x.Year, publicNotice.Year),
                Builders<PublicNoticeDocumentV2>.Filter.Eq(x => x.Number, publicNotice.Number),
                Builders<PublicNoticeDocumentV2>.Filter.Eq(x => x.ExamPhase, publicNotice.ExamPhase),
                Builders<PublicNoticeDocumentV2>.Filter.Eq(x => x.ExamBoard, publicNotice.ExamBoard),
                Builders<PublicNoticeDocumentV2>.Filter.Eq(x => x.Position, publicNotice.Position)
            );

            var existing = await collection.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                // Adiciona ao cache e retorna o ID
                _publicNoticeCache[cacheKey] = existing.Id!;
                return existing.Id!;
            }

            // Cria um novo PublicNotice
            var newPublicNotice = new PublicNoticeDocumentV2
            {
                Number = publicNotice.Number,
                Year = publicNotice.Year,
                ExamPhase = publicNotice.ExamPhase,
                ExamBoard = publicNotice.ExamBoard,
                Position = publicNotice.Position,
                ExamBookletURL = publicNotice.ExamBookletURL,
                ExamAnswerKeyURL = publicNotice.ExamAnswerKeyURL,
                CreatedAt = DateTime.UtcNow
            };

            await collection.InsertOneAsync(newPublicNotice);

            // Adiciona ao cache e retorna o ID
            _publicNoticeCache[cacheKey] = newPublicNotice.Id!;
            return newPublicNotice.Id!;
        }

        private List<ContentBlock> ConvertSupportsToContents(string questionBody, List<QuestionSupport> supports)
        {
            var contents = new List<ContentBlock>();
            int order = 0;

            // Adiciona o corpo da questão como primeiro parágrafo
            if (!string.IsNullOrEmpty(questionBody))
            {
                contents.Add(new ParagraphBlock
                {
                    Order = order++,
                    Inlines = new List<InlineContent>
                    {
                        new TextInline
                        {
                            Type = "text",
                            Text = questionBody,
                            Bold = false,
                            Italic = false
                        }
                    }
                });
            }

            // Converte os supports para ContentBlocks
            if (supports != null)
            {
                foreach (var support in supports)
                {
                    if (support is TextSupport textSupport)
                    {
                        contents.Add(new ParagraphBlock
                        {
                            Order = order++,
                            Inlines = new List<InlineContent>
                            {
                                new TextInline
                                {
                                    Type = "text",
                                    Text = textSupport.Text ?? string.Empty,
                                    Bold = false,
                                    Italic = false
                                }
                            }
                        });
                    }
                    else if (support is ImageSupport imageSupport)
                    {
                        // Ignora imagens sem conteúdo Base64
                        if (string.IsNullOrWhiteSpace(imageSupport.Base64))
                            continue;

                        // Para ImageSupport com Base64, vamos criar um ImageBlock
                        // Nota: você precisará implementar o upload das imagens para um storage
                        // Por enquanto, vamos criar uma chave temporária
                        contents.Add(new ImageBlock
                        {
                            Order = order++,
                            Key = $"migration/{Guid.NewGuid()}.{GetExtensionFromContentType(imageSupport.ContentType)}",
                            Title = imageSupport.Title,
                            Source = imageSupport.Source,
                            Description = imageSupport.Description
                        });
                    }
                }
            }

            return contents;
        }

        private List<ChoiceV2> ConvertChoices(List<Choice>? oldChoices)
        {
            if (oldChoices == null)
                return new List<ChoiceV2>();

            var newChoices = new List<ChoiceV2>();

            foreach (var oldChoice in oldChoices)
            {
                var inlines = new List<InlineContent>();

                // Adiciona o texto da alternativa
                if (!string.IsNullOrEmpty(oldChoice.Text))
                {
                    inlines.Add(new TextInline
                    {
                        Type = "text",
                        Text = oldChoice.Text,
                        Bold = false,
                        Italic = false
                    });
                }

                // Adiciona imagem de suporte se existir e tiver conteúdo Base64
                if (oldChoice.SupportImage != null && !string.IsNullOrWhiteSpace(oldChoice.SupportImage.Base64))
                {
                    inlines.Add(new ImageInline
                    {
                        Type = "image",
                        Key = $"migration/{Guid.NewGuid()}.{GetExtensionFromContentType(oldChoice.SupportImage.ContentType)}",
                        Alt = oldChoice.SupportImage.Title
                    });
                }

                newChoices.Add(new ChoiceV2
                {
                    Option = oldChoice.Option,
                    Content = inlines,
                    IsCorrect = oldChoice.IsCorrect
                });
            }

            return newChoices;
        }

        private string GetExtensionFromContentType(string contentType)
        {
            return contentType?.ToLower() switch
            {
                "image/jpeg" or "image/jpg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                "image/webp" => "webp",
                _ => "jpg"
            };
        }
    }
}
