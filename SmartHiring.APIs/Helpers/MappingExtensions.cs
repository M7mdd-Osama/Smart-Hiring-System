using SmartHiring.Core.Entities.Identity;
using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
    public static class MappingExtensions
    {
        public static string GetFullName(this Applicant a) => $"{a.FName} {a.LName}";
        public static string GetFullName(this AppUser u) => $"{u.FirstName} {u.LastName}";
    }
}
