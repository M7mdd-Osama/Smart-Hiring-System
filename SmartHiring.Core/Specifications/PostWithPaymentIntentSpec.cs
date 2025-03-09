using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
	public class PostWithPaymentIntentSpec : BaseSpecifications<Post>
	{
		public PostWithPaymentIntentSpec(string paymentIntentId)
			: base(p => p.PaymentIntentId == paymentIntentId)
		{
		}
	}
}
