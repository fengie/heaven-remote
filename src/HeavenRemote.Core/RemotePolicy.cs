namespace HeavenRemote.Core;

public enum WorkspaceMode { Work, Game, IndependentSeat }
public sealed record StreamPreset(int Width, int Height, int Fps, int BitrateMbps, string Codec, bool Hdr);
public sealed record Readiness(bool HostEthernetUp, bool? ClientEthernetUp = null,
    bool? StreamSurvivesRadiosOff = null, bool? CorrectCaptureDisplay = null);

public static class RemotePolicy
{
    public static StreamPreset Preset(int fps = 60)
    {
        if (fps is not (60 or 120)) throw new ArgumentOutOfRangeException(nameof(fps));
        return new(1920, 1200, fps, fps == 60 ? 20 : 40, "HEVC", false);
    }

    public static string WiredAssessment(Readiness evidence)
    {
        ArgumentNullException.ThrowIfNull(evidence);
        if (evidence.ClientEthernetUp == false || evidence.StreamSurvivesRadiosOff == false)
            return "Not established: client wired-path test failed";
        if (evidence.HostEthernetUp && evidence.ClientEthernetUp == true &&
            evidence.StreamSurvivesRadiosOff == true)
            return "Operator-observed wired path; latency remains unmeasured";
        return "Unverified: host Ethernet alone is not client route evidence";
    }

    public static string SessionLabel(WorkspaceMode mode) => mode switch
    {
        WorkspaceMode.Work or WorkspaceMode.Game => "Shared Windows Session",
        WorkspaceMode.IndependentSeat => "Independent Gaming Seat — not provisioned",
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };

    public static void RequireCaptureEvidence(Readiness evidence)
    {
        ArgumentNullException.ThrowIfNull(evidence);
        if (evidence.CorrectCaptureDisplay != true)
            throw new InvalidOperationException("CAPTURE_TARGET_UNVERIFIED: do not fall back to the physical screen");
    }
}

// No arbitrary URLs, command lines, ports or shell commands accepted from profiles.
public static class Destinations
{
    public static string Resolve(string action) => action switch
    {
        "dashboard" => "http://127.0.0.1:7331/",
        "sunshine" => "https://localhost:47990/",
        "display" => "ms-settings:display",
        "spacedesk" => "https://manual.spacedesk.net/iOSUSBCableConnection.html",
        "moonlight" => "https://github.com/moonlight-stream/moonlight-docs/wiki/Setup-Guide",
        "apple" => "https://support.apple.com/guide/devices-windows/welcome/windows",
        _ => throw new ArgumentException("Unknown destination", nameof(action))
    };
}

// Public export is an allowlist projection, never a scrubbed copy of private logs.
public static class PublicDiagnostics
{
    public static string Export(int displayCount, bool hostEthernetUp, bool? dashboardPortOpen, bool? sunshinePortOpen)
    {
        if (displayCount is < 0 or > 64) throw new ArgumentOutOfRangeException(nameof(displayCount));
        return JsonSerializer.Serialize(new
        {
            schema = "heaven-remote/public-readiness/v1",
            displayCount, hostEthernetUp, dashboardPortOpen, sunshinePortOpen,
            serviceIdentity = "not-authenticated", clientRoute = "unverified",
            streaming = "unverified", performance = "unmeasured", installedAcceptance = "pending"
        }, new JsonSerializerOptions { WriteIndented = true });
    }
}
