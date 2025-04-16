using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHiring.APIs.DTOs;
using SmartHiring.APIs.Errors;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Services;
using Stripe;
using System.Diagnostics;

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
            try
            {
                var post = await _paymentService.CreateOrUpdatePaymentIntent(postId);
                if (post is null) return BadRequest(new ApiResponse(400, "There is a problem with your Job Posting"));

                var MappedPost = _mapper.Map<Post, PostPaymentDto>(post);
                return Ok(MappedPost);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateOrUpdatePaymentIntent: {ex.Message}");
                return StatusCode(500, new ApiResponse(500, "Internal server error occurred while processing payment"));
            }
        }
        [HttpPost("UpdatePaymentStatus")]
        public async Task<ActionResult<PostPaymentDto>> UpdatePaymentStatus([FromBody] PaymentUpdateDto paymentUpdate)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentUpdate.PaymentIntentId))
                {
                    return BadRequest(new ApiResponse(400, "Payment Intent ID is required"));
                }

                var post = await _paymentService.UpdatePaymentStatus(paymentUpdate.PaymentIntentId, paymentUpdate.IsSuccess);
                if (post is null) return NotFound(new ApiResponse(404, "Payment intent not found"));

                var mappedPost = _mapper.Map<Post, PostPaymentDto>(post);
                return Ok(mappedPost);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdatePaymentStatus: {ex.Message}");
                return StatusCode(500, new ApiResponse(500, "Internal server error occurred while updating payment status"));
            }
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
            catch (StripeException ex)
            {
                Debug.WriteLine($"Stripe webhook error: {ex.Message}");
                return BadRequest(new ApiResponse(400));
            }
        }
    }

}