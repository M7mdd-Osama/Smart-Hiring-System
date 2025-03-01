using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
	public class PostWithCompanySpecifications : BaseSpecifications<Post>
	{
		public PostWithCompanySpecifications():base()
		{
			Includes.Add(P=>P.HR.HRCompany);
		}
		public PostWithCompanySpecifications(int id):base(P => P.Id == id)
		{
			Includes.Add(P => P.HR.HRCompany);
		}
	}
}
