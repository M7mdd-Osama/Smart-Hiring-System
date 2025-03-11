using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Services
{
	public interface IPaymentService
	{
		Task<Post> CreateOrUpdatePaymentIntent(int PostId);
		Task<Post> UpdatePaymentStatus(string PaymentIntentId, bool isSuccess);
	}
}
