namespace Randevu365.Domain.Enum;

public enum BusinessCategory
{
    Kuafor = 1,
    Guzellik = 2,
    Saglik = 3,
    Fitness = 4,
    Dis = 5,
    Masaj = 6,
    Veteriner = 7
}

public static class BusinessCategoryExtensions
{
    private static readonly Dictionary<string, BusinessCategory> _fromJson = new()
    {
        { "Kuaför", BusinessCategory.Kuafor },
        { "Güzellik", BusinessCategory.Guzellik },
        { "Sağlık", BusinessCategory.Saglik },
        { "Fitness", BusinessCategory.Fitness },
        { "Diş", BusinessCategory.Dis },
        { "Masaj", BusinessCategory.Masaj },
        { "Veteriner", BusinessCategory.Veteriner }
    };

    public static string ToJson(this BusinessCategory category) => category switch
    {
        BusinessCategory.Kuafor => "Kuaför",
        BusinessCategory.Guzellik => "Güzellik",
        BusinessCategory.Saglik => "Sağlık",
        BusinessCategory.Fitness => "Fitness",
        BusinessCategory.Dis => "Diş",
        BusinessCategory.Masaj => "Masaj",
        BusinessCategory.Veteriner => "Veteriner",
        _ => throw new ArgumentOutOfRangeException(nameof(category))
    };

    public static bool TryFromJson(string? value, out BusinessCategory result)
    {
        if (value != null && _fromJson.TryGetValue(value, out result))
            return true;
        result = default;
        return false;
    }

    public static IEnumerable<string> ValidValues => _fromJson.Keys;
}
