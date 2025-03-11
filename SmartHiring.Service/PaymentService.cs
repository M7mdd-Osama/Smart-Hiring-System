using Microsoft.Extensions.Configuration;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Services;
using SmartHiring.Core.Specifications;
using Stripe;
using System.Threading.Tasks;

namespace SmartHiring.Service
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IGenericRepository<Post> _postRepository;

		public PaymentService(IConfiguration configuration, IGenericRepository<Post> postRepository)
		{
			_configuration = configuration;
			_postRepository = postRepository;
		}

		public async Task<Post> CreateOrUpdatePaymentIntent(int PostId)
		{
			StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

			var post = await _postRepository.GetByIdAsync(PostId);
			if (post == null) return null;

			const long postPrice = 2000;
			var amount = postPrice;

			var service = new PaymentIntentService();
			PaymentIntent paymentIntent;

			if (string.IsNullOrEmpty(post.PaymentIntentId))
			{
				var options = new PaymentIntentCreateOptions
				{
					Amount = amount,
					Currency = "usd",
					PaymentMethodTypes = new List<string> { "card" }
				};
				paymentIntent = await service.CreateAsync(options);
				post.PaymentIntentId = paymentIntent.Id;
				post.ClientSecret = paymentIntent.ClientSecret;
			}
			else
			{
				var options = new PaymentIntentUpdateOptions { Amount = amount };
				paymentIntent = await service.UpdateAsync(post.PaymentIntentId, options);
				post.PaymentIntentId = paymentIntent.Id;
				post.ClientSecret = paymentIntent.ClientSecret;
			}

			await _postRepository.UpdateAsync(post);
			return post;
		}

		public async Task<Post> UpdatePaymentStatus(string PaymentIntentId, bool isSuccess)
		{
			var spec = new PostWithPaymentIntentSpec(PaymentIntentId);
			var post = await _postRepository.GetByEntityWithSpecAsync(spec);

			if (post == null) return null;

			post.PaymentStatus = isSuccess ? "Paid" : "Failed";

			await _postRepository.UpdateAsync(post);
			return post;
		}
	}
}