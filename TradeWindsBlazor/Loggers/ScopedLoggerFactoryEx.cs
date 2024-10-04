
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

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using TradeWindsBlazor.Extensions;

namespace TradeWindsBlazor.Loggers
{

    /// <summary>
    /// Provides a logger where each log will have a scope with the User's AspNetId and email.
    /// </summary>
    public class ScopedLoggerFactoryEx
    {

        private readonly ILoggerFactory _factory;
        private readonly AuthenticationStateProvider _provider;
        private ClaimsPrincipal? _principal;

        public ScopedLoggerFactoryEx(ILoggerFactory factory, AuthenticationStateProvider provider)
        {
            _factory = factory;
            _provider = provider;
            // need to set this in GetLogger as it's an async call
            _principal = null;
        }

        /// <summary>
        /// Get the ILogger.
        /// </summary>
        /// <param name="type">The class the logger is going to log in.</param>
        /// <returns>An ILogger with the scope set on each Log call.</returns>
        public async Task<ScopedLoggerEx> GetLogger(Type type)
        {
            var logger = _factory.CreateLogger(type);

            // we don't bother tracking changes as a user can't change their AspNetId.
            // and its not worth the overhead to track changes to the email as it's rare and the old
            // email still identifies them.
            _principal ??= (await _provider.GetAuthenticationStateAsync()).User;

            return new ScopedLoggerEx(logger, _principal.UserId(), _principal.Identity?.Name ?? ClaimsPrincipalExtensions.AnonymousName);
        }
    }
}