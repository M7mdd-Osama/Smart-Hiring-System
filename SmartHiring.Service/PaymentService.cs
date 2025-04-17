using Microsoft.Extensions.Configuration;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Core.Services;
using SmartHiring.Core.Specifications;
using Stripe;
using System.Diagnostics;

namespace SmartHiring.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IGenericRepo<Post> _postRepository;

        public PaymentService(IConfiguration configuration, IGenericRepo<Post> postRepository)
        {
            _configuration = configuration;
            _postRepository = postRepository;
        }

        public async Task<Post> CreateOrUpdatePaymentIntent(int PostId)
        {
            try
            {
                string secretKey = _configuration["StripeSettings:Secretkey"];

                if (string.IsNullOrEmpty(secretKey))
                {
                    secretKey = _configuration["StripeSettings:SecretKey"];
                }

                if (string.IsNullOrEmpty(secretKey))
                {
                    secretKey = "sk_test_51Qz1VQEUH1yUA15MYMuwehbdxzNFHHW7DINrgaRdk5OcW2mbAH8xo2nF49M8R4du3ltNiktrWGlJaDdrpy07zwb000DHRdKn8F";
                }

                StripeConfiguration.ApiKey = secretKey;

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
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en CreateOrUpdatePaymentIntent: {ex.Message}");
                throw;
            }
        }

        public async Task<Post> UpdatePaymentStatus(string PaymentIntentId, bool isSuccess)
        {
            try
            {
                var spec = new PostWithPaymentIntentSpec(PaymentIntentId);
                var post = await _postRepository.GetByEntityWithSpecAsync(spec);

                if (post == null) return null;

                post.PaymentStatus = isSuccess ? "Paid" : "Failed";

                if (isSuccess)
                {
                    post.PaymentIntentId = null;
                    post.ClientSecret = null;
                }

                await _postRepository.UpdateAsync(post);
                return post;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en UpdatePaymentStatus: {ex.Message}");
                throw;
            }
        }
    }
}