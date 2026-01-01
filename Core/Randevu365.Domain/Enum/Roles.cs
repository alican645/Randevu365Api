namespace Randevu365.Domain.Enum;

public enum RolesEnum
{
    BusinessOwner = 1,
    Customer = 2,
    Administrator = 3,
}

/// <summary>
/// Authorize attribute'larında kullanılmak üzere rol isimleri
/// </summary>
public static class Roles
{
    public const string BusinessOwner = nameof(RolesEnum.BusinessOwner);
    public const string Customer = nameof(RolesEnum.Customer);
    public const string Administrator = nameof(RolesEnum.Administrator);
}