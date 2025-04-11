using SmartHiring.Core.Entities;

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
