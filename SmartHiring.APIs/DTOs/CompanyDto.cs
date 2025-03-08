namespace SmartHiring.APIs.DTOs
{
    public class CompanyDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int HRCount { get; set; }
        public int PostCount { get; set; }
        public string BusinessEmail { get; set; } // ✅ إضافة الحقل المفقود
        public string Password { get; set; } // ✅ إضافة كلمة المرور




    }
}