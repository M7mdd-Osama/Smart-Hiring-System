using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Services;

namespace SmartHiring.Service
{
    public class ResumeEvaluationService : IResumeEvaluationService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<PredictionResult?> _retryPolicy;
        private readonly ILogger<ResumeEvaluationService> _logger;

        public ResumeEvaluationService(HttpClient httpClient, ILogger<ResumeEvaluationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromMinutes(3);

            // Configure Retry Policy
            _retryPolicy = Policy<PredictionResult?>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<PredictionResult?> EvaluateResumeAsync(int postId, string resumeText)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    var request = new ResumeRequest
                    {
                        job_id = postId,
                        resume_text = resumeText
                    };

                    var json = JsonSerializer.Serialize(request);
                    _logger.LogInformation($"Sending request to AI model: {json}");

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("http://localhost:8000/evaluate_cv/", content);

                    // Don't use EnsureSuccessStatusCode here, handle the response manually
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError($"API Error: {response.StatusCode} - {errorContent}");
                        throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}). Error: {errorContent}");
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Received response from AI model: {result}");

                    return JsonSerializer.Deserialize<PredictionResult>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in EvaluateResumeAsync");
                    throw;
                }
            });
        }
    }
}