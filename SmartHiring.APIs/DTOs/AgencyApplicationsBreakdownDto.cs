﻿namespace SmartHiring.APIs.DTOs
{
    public class AgencyApplicationsBreakdownDto
    {
        public string AgencyName { get; set; }
        public int ApplicationsCount { get; set; }
        public List<ApplicationDetailDto> Applications { get; set; }
    }

}
