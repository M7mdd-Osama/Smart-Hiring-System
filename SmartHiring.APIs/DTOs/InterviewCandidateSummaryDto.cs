namespace SmartHiring.APIs.DTOs
{
    public class InterviewCandidateSummaryDto
    {
        public string Name { get; set; }                  // اسم المتقدم
        public int TotalApplications { get; set; }        // عدد كل الـ Interviews اللي اتعملت له
        public int Accepted { get; set; }                 // عدد الـ Interviews اللي اتقبل فيها
        public int Rejected { get; set; }                 // عدد الـ Interviews اللي اترفض فيها
        public double AverageScore { get; set; }          // متوسط السكور
        public string Status { get; set; }                // الحالة الأخيرة (مثلاً Pending)
    }
}
