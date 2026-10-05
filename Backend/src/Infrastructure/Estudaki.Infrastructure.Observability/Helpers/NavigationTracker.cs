using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Estudaki.Infrastructure.Observability.Helpers
{
    public class NavigationTracker : IDisposable
    {
        private readonly NavigationManager _nav;
        private readonly ILogger<NavigationTracker> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private DateTime _lastNavigationTime = DateTime.UtcNow;
        private string _currentPath = "";

        public NavigationTracker(
            NavigationManager nav,
            ILogger<NavigationTracker> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _nav = nav;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            _nav.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            var now = DateTime.UtcNow;
            var newPath = _nav.ToBaseRelativePath(e.Location);

            if (string.IsNullOrWhiteSpace(newPath) ||
                newPath.StartsWith("Error"))
                return;

            var duration = now - _lastNavigationTime;

            var sessionId = _httpContextAccessor
                .HttpContext?
                .Request
                .Cookies["sid"];

            if (!string.IsNullOrEmpty(_currentPath))
            {
                _logger.LogInformation(
                    "BlazorTimeOnPage {@Data}",
                    new
                    {
                        Path = _currentPath,
                        DurationMs = duration.TotalMilliseconds,
                        SessionId = sessionId
                    });
            }

            _logger.LogInformation(
                "BlazorNavigation {@Data}",
                new
                {
                    Path = newPath,
                    FullUrl = e.Location,
                    Intercepted = e.IsNavigationIntercepted,
                    SessionId = sessionId,
                    Timestamp = now
                });

            _currentPath = newPath;
            _lastNavigationTime = now;
        }

        public void Dispose()
        {
            _nav.LocationChanged -= OnLocationChanged;
        }
    }
}
