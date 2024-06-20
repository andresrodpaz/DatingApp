using System.Text.Json;
using Microsoft.AspNetCore.Http; // Ensure you have the correct namespace for HttpResponse
using API.Helpers; // Assuming PaginationHeader is defined in this namespace

namespace API.Extensions
{
    /// <summary>
    /// Extension methods for HTTP-related functionality.
    /// </summary>
    public static class HttpExtensions
    {
        /// <summary>
        /// Adds a pagination header to the HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response to add headers to.</param>
        /// <param name="header">The pagination header containing pagination information.</param>
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
        {
            // Configure JsonSerializer options to use camelCase for property names.
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            
            // Serialize the PaginationHeader object to JSON and add it to the response headers.
            response.Headers.Add("Pagination", JsonSerializer.Serialize(header, jsonOptions));
            
            // Ensure the 'Pagination' header is exposed to clients in CORS scenarios.
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
