using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.Controllers;
using SmartHiring.Core.Entities;

public class AIRecommendationSystemController : APIBaseController
{
    private readonly HttpClient _httpClient;

    public AIRecommendationSystemController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpPost("evaluate-cv")]
    public async Task<IActionResult> EvaluateCv([FromBody] ResumeRequest resumeRequest)
    {
        var json = JsonSerializer.Serialize(resumeRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:8000/evaluate_cv/", content);
        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

        var result = await response.Content.ReadAsStringAsync();
        return Ok(result);
    }
}