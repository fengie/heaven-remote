namespace PhoneSeat.Logic;

public static class DisplayModeSelector
{
    public static int? ChooseRefresh(IEnumerable<int> supportedRefreshRates, int? preferredHz)
    {
        ArgumentNullException.ThrowIfNull(supportedRefreshRates);
        var supported = supportedRefreshRates.Where(x => x > 1).Distinct().ToArray();
        if (supported.Length == 0)
            return null;

        if (!preferredHz.HasValue)
            return supported.Max();

        return supported
            .OrderBy(x => Math.Abs(x - preferredHz.Value))
            .ThenByDescending(x => x)
            .First();
    }
}
