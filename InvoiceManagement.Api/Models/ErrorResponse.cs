namespace InvoiceManagement.Api.Models
{
    /// <summary>
    /// Intercepts the HTTP request and handles exceptions globally.
    /// </summary>
    /// <param>Current HTTP context.</param>
    /// <returns>Task representing the request pipeline execution.</returns>
    public class ErrorResponse
    {
        /// <summary>
        /// Standard API error response model.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
