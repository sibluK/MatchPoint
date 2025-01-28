using Microsoft.Extensions.Localization;

namespace TournamentManager.Utility;

public static class EnumExtensions
{
    public static string GetLocalizedName<TEnum>(this TEnum valueValue, IStringLocalizer localizer) where TEnum : Enum
    {
        string key = $"{typeof(TEnum).Name}_{valueValue}";
        return localizer[key];
    }
}