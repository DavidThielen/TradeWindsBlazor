
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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TradeWindsBlazor.Extensions;
using TradeWindsBlazor.Loggers;

namespace TradeWindsBlazor
{
    /// <summary>
    /// The base class for any component other than a simple component.
    /// </summary>
    internal class ExComponentBase : ComponentBase
	{
		[Inject]
		private ScopedLoggerFactoryEx ScopedLoggerFactoryEx { get; set; } = default!;

		/// <summary>
		/// This logger is for use in the methods in this base class and not for the subclass.
		/// </summary>
		[Inject]
		private ILogger<ExComponentBase> Logger { get; set; } = default!;

		[CascadingParameter] 
		protected Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

		/// <summary>
		/// THe HttpContext for the current session (circuit). Only valid in OnInitializedAsync,
		/// </summary>
        [CascadingParameter]
        private HttpContext? HttpContext { get; set; }
		
		/// <summary>
		/// Built from AuthenticationStateTask - the logged in Principal (user).
		/// </summary>
		protected ClaimsPrincipal Principal { get; set; } = ClaimsPrincipalExtensions.Anonymous;

		/// <summary>
		/// The logger for the subclass. This is for use in the subclass and not in this base class.
		/// </summary>
		protected ScopedLoggerEx LoggerEx { get; set; } = default!;

        /// <summary>
        /// true if this is the pre-render call to OnInitializedAsync. Do <b>not</b> call this anywhere other
        /// than inside OnInitializedAsync!
        /// </summary>
        protected bool IsPreRender => HttpContext is not null;

		/// <summary>
		/// Returns the User-Agent from the request. This is only valid in OnInitializedAsync in the PreRender pass. Will return
		/// null for other calls.
		/// </summary>
		protected string? RequestUserAgent => HttpContext?.Request.Headers["User-Agent"];

		/// <inheritdoc />
		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();

			// get the Principal
			Principal = (await AuthenticationStateTask).User;

			// set up the logger for this component. Passes GetType() so it is a logger for
			// this object, not for the base class.
			LoggerEx = await ScopedLoggerFactoryEx.GetLogger(GetType());
		}
	}
}
