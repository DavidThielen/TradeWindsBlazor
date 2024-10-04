
// Copyright (c) 2024 Trade Winds Studios (David Thielen)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using TradeWindsBlazor.Extensions;

namespace TradeWindsBlazor.Loggers
{
    /// <summary>
    /// Provides a logger where each log will have a scope with the User's AspNetId and email.
    /// </summary>
    public class ScopedLoggerFactory<T>
    {

        private readonly ILoggerFactory _factory;
        private readonly AuthenticationStateProvider _provider;

        public ScopedLoggerFactory(ILoggerFactory factory, AuthenticationStateProvider provider)
        {
            _factory = factory;
            _provider = provider;
        }

        /// <summary>
        /// Get the ILogger.
        /// </summary>
        /// <typeparam name="T">The class the logger is going to log in.</typeparam>
        /// <returns>AN ILogger with the scope set on each Log call.</returns>
        public async Task<ScopedLogger<T>> GetLogger()
        {
            var logger = _factory.CreateLogger<T>();

            var principal = (await _provider.GetAuthenticationStateAsync()).User;

            return new ScopedLogger<T>(logger, principal.UserId(), principal.Identity?.Name ?? ClaimsPrincipalExtensions.AnonymousName);
        }
    }

    /// <summary>
    /// An ILogger where each Log() call will be in a scope with the user information.
    /// </summary>
    /// <typeparam name="T">The class the logger is going to log in.</typeparam>
    public class ScopedLogger<T> : ILogger<T>
    {

        private readonly ILogger<T> _logger;
        private readonly string? _aspNetId;
        private readonly string? _username;

        public ScopedLogger(ILogger<T> logger, string? aspNetId, string? username)
        {
            _logger = logger;
            _aspNetId = aspNetId;
            _username = username;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            using (_logger.BeginScope("User:{username}, {aspNetId}", _username, _aspNetId))
            {
                _logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _logger.BeginScope(state);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }
    }
}
