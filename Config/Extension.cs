using CounterStrikeSharp.API.Core;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;


namespace Reveal_Last_Alive;

public static class Extension
{
    public static bool IsValid([NotNullWhen(true)] this CCSPlayerController? player, bool IncludeBots = false, bool IncludeHLTV  = false)
    {
        if (player == null || !player.IsValid)
            return false;

        if (!IncludeBots && player.IsBot)
            return false;

        if (!IncludeHLTV && player.IsHLTV)
            return false;

        return true;
    }

    public static float ToPercentageFloat(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return 1f;
        }

        input = input.Replace("%", "").Trim();

        if (!float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            return 1f;
        }

        return Math.Clamp(result / 100f, 0f, 1f);
    }
}