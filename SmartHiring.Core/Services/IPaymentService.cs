using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Services
{
	public interface IPaymentService
	{
		Task<Post> CreateOrUpdatePaymentIntent(int PostId);
		Task<Post> UpdatePaymentStatus(string PaymentIntentId, bool isSuccess);
	}
}
