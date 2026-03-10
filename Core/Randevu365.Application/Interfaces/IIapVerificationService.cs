namespace Randevu365.Application.Interfaces;

public class IapVerificationResult
{
    public bool IsValid { get; set; }
    public string? TransactionId { get; set; }
    public string? ProductId { get; set; }
    public string? ErrorMessage { get; set; }
}

public interface IIapVerificationService
{
    Task<IapVerificationResult> VerifyAppleReceiptAsync(string receiptData);
    Task<IapVerificationResult> VerifyGooglePurchaseAsync(string packageName, string productId, string purchaseToken);
}
