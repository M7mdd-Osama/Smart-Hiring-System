using SmartHiring.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
    public class UsersByRoleSpecification : BaseSpecifications<AppUser>
    {
        public UsersByRoleSpecification(string roleName)
        : base(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
        {
            AddInclude(u => u.UserRoles);
            AddThenInclude(ur => ur.Role);
        }
    }
}
