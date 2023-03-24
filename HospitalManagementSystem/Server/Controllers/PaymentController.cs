using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Identity.Client;
using Stripe;
using Stripe.Checkout;
using System.Text;

namespace HospitalManagementSystem.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public PaymentController()
        {
            StripeConfiguration.ApiKey = "sk_test_51M6VuGSEVEH46znALqeCQfXrgUc0MGnhBdKS7ewLLvQCFszM7zlwm325krHsNNPI8QTxEXhohUzhC7K3Fsm76LLZ00sQ5cdMKf";
        }

        [HttpPost("checkout/{id}")]
        public ActionResult CreateCheckoutSession(List<PrescripedMedicine> prescripedMedicines, int id)
        {
            var lineItems = new List<SessionLineItemOptions>();
            prescripedMedicines.ForEach(pm => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = pm.Price * 100,
                    Currency = "inr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = pm.Name,
                    },
                },
                Quantity = pm.Quantity,
            }));
            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"https://localhost:7105/paymentsuccess/{id}",
                CancelUrl = $"https://localhost:7105/viewbill/{id}",
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return Ok(session.Url);
        }
    }
}
