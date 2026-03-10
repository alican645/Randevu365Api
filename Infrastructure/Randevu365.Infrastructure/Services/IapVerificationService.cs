using System.Text;
using System.Text.Json;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Randevu365.Application.Interfaces;

namespace Randevu365.Infrastructure.Services;

public class IapVerificationService : IIapVerificationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public IapVerificationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<IapVerificationResult> VerifyAppleReceiptAsync(string receiptData)
    {
        var verifyUrl = _configuration["InAppPurchase:Apple:VerifyUrl"]!;
        var sandboxUrl = _configuration["InAppPurchase:Apple:SandboxVerifyUrl"]!;
        var sharedSecret = _configuration["InAppPurchase:Apple:SharedSecret"]!;

        var result = await SendAppleVerificationRequest(verifyUrl, receiptData, sharedSecret);

        // status 21007 = sandbox receipt sent to production, retry with sandbox
        if (result.status == 21007)
            result = await SendAppleVerificationRequest(sandboxUrl, receiptData, sharedSecret);

        if (result.status != 0)
        {
            return new IapVerificationResult
            {
                IsValid = false,
                ErrorMessage = $"Apple doğrulama başarısız. Status: {result.status}"
            };
        }

        var latestReceipt = result.latestReceiptInfo?.FirstOrDefault();
        return new IapVerificationResult
        {
            IsValid = true,
            TransactionId = latestReceipt?.transactionId ?? result.receipt?.transactionId,
            ProductId = latestReceipt?.productId ?? result.receipt?.productId
        };
    }

    public async Task<IapVerificationResult> VerifyGooglePurchaseAsync(
        string packageName, string productId, string purchaseToken)
    {
        var keyPath = _configuration["InAppPurchase:Google:ServiceAccountKeyPath"]!;

        try
        {
            var credential = GoogleCredential.FromFile(keyPath)
                .CreateScoped(AndroidPublisherService.Scope.Androidpublisher);

            var service = new AndroidPublisherService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Randevu365"
            });

            var request = service.Purchases.Products.Get(packageName, productId, purchaseToken);
            var purchase = await request.ExecuteAsync();

            if (purchase.PurchaseState != 0)
            {
                return new IapVerificationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Google satın alma durumu geçersiz. PurchaseState: {purchase.PurchaseState}"
                };
            }

            return new IapVerificationResult
            {
                IsValid = true,
                TransactionId = purchase.OrderId,
                ProductId = productId
            };
        }
        catch (Exception ex)
        {
            return new IapVerificationResult
            {
                IsValid = false,
                ErrorMessage = $"Google doğrulama hatası: {ex.Message}"
            };
        }
    }

    private async Task<AppleVerifyResponse> SendAppleVerificationRequest(
        string url, string receiptData, string sharedSecret)
    {
        var client = _httpClientFactory.CreateClient("AppleIAP");

        var payload = JsonSerializer.Serialize(new
        {
            receipt_data = receiptData,
            password = sharedSecret
        });

        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<AppleVerifyResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new AppleVerifyResponse();
    }

    // Apple response models
    private class AppleVerifyResponse
    {
        public int status { get; set; }
        public AppleReceipt? receipt { get; set; }
        public List<AppleReceiptInfo>? latestReceiptInfo { get; set; }

        // Support snake_case from Apple's API
        public List<AppleReceiptInfo>? latest_receipt_info
        {
            set => latestReceiptInfo = value;
        }
    }

    private class AppleReceipt
    {
        public string? transactionId { get; set; }
        public string? productId { get; set; }

        public string? transaction_id { set => transactionId = value; }
        public string? product_id { set => productId = value; }
    }

    private class AppleReceiptInfo
    {
        public string? transactionId { get; set; }
        public string? productId { get; set; }

        public string? transaction_id { set => transactionId = value; }
        public string? product_id { set => productId = value; }
    }
}
