using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Estudaki.Modules.Identity.Infrastructure.Middleware;

/// <summary>
/// Middleware que remove cookies de autenticação para endpoints públicos ([AllowAnonymous]).
/// Isso evita que o middleware de autenticação valide os cookies desnecessariamente,
/// melhorando drasticamente a performance de endpoints não autenticados.
/// </summary>
public class SkipAuthenticationCookieMiddleware
{
    private readonly RequestDelegate _next;

    public SkipAuthenticationCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verifica se o endpoint tem AllowAnonymous
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            // Verifica se há AllowAnonymousAttribute
            var allowAnonymous = endpoint.Metadata.GetOrderedMetadata<IAllowAnonymous>().Any();

            if (allowAnonymous)
            {
                // Remove cookies de autenticação para não serem validadas
                // Isso previne chamadas desnecessárias ao banco de dados
                context.Request.Headers.Remove("Cookie");
            }
        }

        await _next(context);
    }
}
