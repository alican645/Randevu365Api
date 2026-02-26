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
        { "kuafor", BusinessCategory.Kuafor },
        { "guzellik", BusinessCategory.Guzellik },
        { "saglik", BusinessCategory.Saglik },
        { "fitness", BusinessCategory.Fitness },
        { "dis", BusinessCategory.Dis },
        { "masaj", BusinessCategory.Masaj },
        { "veteriner", BusinessCategory.Veteriner }
    };

    public static string ToJson(this BusinessCategory category) => category switch
    {
        BusinessCategory.Kuafor => "kuafor",
        BusinessCategory.Guzellik => "guzellik",
        BusinessCategory.Saglik => "saglik",
        BusinessCategory.Fitness => "fitness",
        BusinessCategory.Dis => "dis",
        BusinessCategory.Masaj => "masaj",
        BusinessCategory.Veteriner => "veteriner",
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
