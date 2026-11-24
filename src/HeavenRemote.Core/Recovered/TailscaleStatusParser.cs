namespace PhoneSeat.Logic;

public readonly record struct TailscaleStatusSummary(int DirectPeers, int RelayPeers)
{
    public string Description =>
        DirectPeers > 0 ? $"Active direct peer(s): {DirectPeers}" :
        RelayPeers > 0 ? $"Active relay peer(s): {RelayPeers}" :
        "Tailnet connected";
}

public static class TailscaleStatusParser
{
    public static TailscaleStatusSummary Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        using var doc = JsonDocument.Parse(json);

        var direct = 0;
        var relayed = 0;
        if (!doc.RootElement.TryGetProperty("Peer", out var peers) || peers.ValueKind != JsonValueKind.Object)
            return new TailscaleStatusSummary(0, 0);

        foreach (var peer in peers.EnumerateObject())
        {
            var value = peer.Value;
            if (!value.TryGetProperty("Active", out var active) || active.ValueKind != JsonValueKind.True)
                continue;

            var cur = value.TryGetProperty("CurAddr", out var currentAddress)
                ? currentAddress.GetString()
                : null;
            var relay = value.TryGetProperty("Relay", out var relayElement)
                ? relayElement.GetString()
                : null;

            if (!string.IsNullOrWhiteSpace(cur))
                direct++;
            else if (!string.IsNullOrWhiteSpace(relay))
                relayed++;
        }

        return new TailscaleStatusSummary(direct, relayed);
    }
}
