using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Services
{
    public interface IResumeEvaluationService
    {
        Task<PredictionResult?> EvaluateResumeAsync(int postId, string resumeText);
    }
}
