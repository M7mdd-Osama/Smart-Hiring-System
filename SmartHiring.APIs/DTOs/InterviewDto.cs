﻿using System;

namespace SmartHiring.APIs.DTOs
{
    public class InterviewDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; } // تحويل TimeSpan إلى string في الـ response
        public string Location { get; set; }
        public string InterviewStatus { get; set; } = "Pending";
        public double Score { get; set; }
        public string HRId { get; set; }
        public int PostId { get; set; }
        public int ApplicantId { get; set; }
    }
}
