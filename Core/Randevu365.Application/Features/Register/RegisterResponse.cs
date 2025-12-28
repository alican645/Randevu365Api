

namespace Randevu365.Application.Features.Register;


public record RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}