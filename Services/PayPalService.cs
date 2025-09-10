using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace LaboGrocery.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _config;
        public PayPalService(IConfiguration config) => _config = config;

        private PayPalEnvironment GetEnvironment()
        {
            var env = _config["PayPal:Environment"] ?? "Sandbox";
            var clientId = _config["PayPal:ClientId"] ?? "";
            var secret = _config["PayPal:Secret"] ?? "";
            return env.Equals("Sandbox", StringComparison.OrdinalIgnoreCase)
                ? new SandboxEnvironment(clientId, secret)
                : new LiveEnvironment(clientId, secret);
        }

        private PayPalHttpClient GetClient() => new PayPalHttpClient(GetEnvironment());

        public async Task<string?> CreateOrderAsync(decimal total, string currency = "CAD")
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = currency,
                            Value = total.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                        }
                    }
                }
            });

            var response = await GetClient().Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();
            return result.Id;
        }

        public async Task<string?> CaptureOrderAsync(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());
            var response = await GetClient().Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();
            var capture = result.PurchaseUnits?.FirstOrDefault()?.Payments?.Captures?.FirstOrDefault();
            return capture?.Id;
        }
    }
}
