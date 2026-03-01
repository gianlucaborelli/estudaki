using Microsoft.AspNetCore.Mvc;
using ProvaOnline.Models.DTO;
using ProvaOnline.Services;
using System.Text;

namespace ProvaOnline.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SitemapController : ControllerBase
    {
        private readonly IQuestionServices _questionService;
        private readonly ILogger<SitemapController> _logger;

        public SitemapController(IQuestionServices questionService, ILogger<SitemapController> logger)
        {
            _questionService = questionService;
            _logger = logger;
        }

        [HttpGet]
        [HttpGet("sitemap.xml")]
        [Route("/sitemap.xml")]
        [Produces("application/xml")]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("=== GERANDO SITEMAP ===");

            var sitemap = new StringBuilder();
            sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Página principal
            AddUrl(sitemap, "https://estudaki.com.br/", DateTime.UtcNow, "weekly", "1.0");
            _logger.LogInformation("Adicionada página principal ao sitemap");

            // Página de resultados
            AddUrl(sitemap, "https://estudaki.com.br/result", DateTime.UtcNow, "daily", "0.8");
            _logger.LogInformation("Adicionada página de resultados ao sitemap");

            try
            {
                _logger.LogInformation("Buscando questões para o sitemap...");

                // Buscar todas as questões publicadas (com paginação grande)
                var searchParameters = new SearchParameters
                {
                    IsPublished = true,
                    CurrentPage = 1,
                    PageSize = 10000 // Pega todas as questões publicadas
                };

                _logger.LogInformation($"Parâmetros de busca - IsPublished: {searchParameters.IsPublished}, PageSize: {searchParameters.PageSize}");

                var result = await _questionService.SearchQuestionsPaginatedAsync(searchParameters);

                _logger.LogInformation($"Resultado da busca - TotalItems: {result.TotalItems}, Items.Count: {result.Items.Count}");

                if (result.Items.Count == 0)
                {
                    _logger.LogWarning("⚠️ Nenhuma questão retornada pela busca! Verifique se IsPublished está funcionando.");
                }

                int addedCount = 0;
                foreach (var question in result.Items)
                {
                    var questionUrl = $"https://estudaki.com.br/questao/{question._id}";
                    AddUrl(sitemap, questionUrl, question.CreatedAt, "monthly", "0.7");
                    addedCount++;
                }

                _logger.LogInformation($"✅ {addedCount} questões adicionadas ao sitemap");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERRO ao buscar questões para o sitemap");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"StackTrace: {ex.StackTrace}");
                // Se falhar, continua com sitemap básico
            }

            sitemap.AppendLine("</urlset>");

            var sitemapContent = sitemap.ToString();
            _logger.LogInformation($"Sitemap gerado com {sitemapContent.Split("<url>").Length - 1} URLs");

            return Content(sitemapContent, "application/xml", Encoding.UTF8);
        }

        private void AddUrl(StringBuilder sitemap, string loc, DateTime lastmod, string changefreq, string priority)
        {
            sitemap.AppendLine("  <url>");
            sitemap.AppendLine($"    <loc>{loc}</loc>");
            sitemap.AppendLine($"    <lastmod>{lastmod:yyyy-MM-dd}</lastmod>");
            sitemap.AppendLine($"    <changefreq>{changefreq}</changefreq>");
            sitemap.AppendLine($"    <priority>{priority}</priority>");
            sitemap.AppendLine("  </url>");
        }
    }
}
