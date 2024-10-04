
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

namespace TradeWindsBlazor.Extensions
{
	public static class ClaimsPrincipalExtensions
	{
		/// <summary>
		/// The name used for the anonymous user.
		/// </summary>
		public static string AnonymousName { get; } = "Anonymous";

		/// <summary>
		/// Returns the IdentityUser.Id of the passed in principal.
		/// </summary>
		/// <param name="principal">Normally the logged in user.</param>
		/// <returns>The IdentityUser.Id of the passed in principal. null if the user is not logged in (anonymous).</returns>
		public static string? UserId (this ClaimsPrincipal principal)
		{ 
			var claim = principal.FindFirst(u => u.Type.Contains("nameidentifier"));
			return claim?.Value;
		}

		public static List<Claim> AppClaims(this ClaimsPrincipal principal)
		{
			var listClaims = new List<Claim>();
			foreach (var claim in principal.Claims)
			{
				if (claim.Type.StartsWith("http") || claim.Type.StartsWith("AspNet"))
					continue;
				listClaims.Add(claim);
			}

			return listClaims;
		}

        /// <summary>
        /// Returns true if this Principal is an anonymous user.
        /// </summary>
        /// <param name="principal">Normally the logged in user.</param>
        /// <returns>true if this Principal is an anonymous user.</returns>
        public static bool IsAnonymous(this ClaimsPrincipal principal)
		{
			return principal.Identity == null || principal.Identity.IsAuthenticated == false;
		}

		/// <summary>
		/// An anonymous user.
		/// </summary>
		public static ClaimsPrincipal Anonymous => new(new ClaimsIdentity());
	}
}
