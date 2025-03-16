using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Services;
using Stripe;

namespace SmartHiring.APIs.Controllers
{
	public class PaymentsController : APIBaseController
	{
		private readonly IPaymentService _paymentService;
		private readonly IMapper _mapper;
		private const string EndpointSecret = "whsec_d8b3075293cdf9c58d11ca41a974cbb9c7bd1b9fc6c6040df3911f881dad14f3";

		public PaymentsController(IPaymentService paymentService, IMapper mapper)
		{
			_paymentService = paymentService;
			_mapper = mapper;
		}
		[Authorize]
		[HttpPost("{postId}")]
		public async Task<ActionResult<PostPaymentDto>> CreateOrUpdatePaymentIntent(int postId)
		{
			var post = await _paymentService.CreateOrUpdatePaymentIntent(postId);
			if (post is null) return BadRequest(new ApiResponse(400, "There is a problem with your Job Posting"));

			var MappedPost = _mapper.Map<Post, PostPaymentDto>(post);
			return Ok(MappedPost);
		}

		[HttpPost("webhook")]
		public async Task<IActionResult> StripeWebHook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], EndpointSecret);
				var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

				if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
				{
					await _paymentService.UpdatePaymentStatus(paymentIntent.Id, true);
				}
				else if (stripeEvent.Type == EventTypes.PaymentIntentPaymentFailed)
				{
					await _paymentService.UpdatePaymentStatus(paymentIntent.Id, false);
				}

				return Ok();
			}
			catch (StripeException)
			{
				return BadRequest(new ApiResponse(400));
			}
		}
	}
}