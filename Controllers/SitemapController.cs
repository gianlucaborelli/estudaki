using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ProvaOnline.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SitemapController : ControllerBase
    {
        [HttpGet]
        [Produces("application/xml")]
        public IActionResult Get()
        {
            var sitemap = new StringBuilder();
            sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Página principal
            sitemap.AppendLine("  <url>");
            sitemap.AppendLine("    <loc>https://estudaki.com.br/</loc>");
            sitemap.AppendLine($"    <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            sitemap.AppendLine("    <changefreq>weekly</changefreq>");
            sitemap.AppendLine("    <priority>1.0</priority>");
            sitemap.AppendLine("  </url>");

            // Página de resultados
            sitemap.AppendLine("  <url>");
            sitemap.AppendLine("    <loc>https://estudaki.com.br/result</loc>");
            sitemap.AppendLine($"    <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>");
            sitemap.AppendLine("    <changefreq>daily</changefreq>");
            sitemap.AppendLine("    <priority>0.8</priority>");
            sitemap.AppendLine("  </url>");

            sitemap.AppendLine("</urlset>");

            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}
