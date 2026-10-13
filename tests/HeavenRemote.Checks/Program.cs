using System.Text.Json;
using HeavenRemote.Core;
using PhoneSeat.Logic;

var passed = 0;
void Check(string name, Action assertion)
{
    try { assertion(); Console.WriteLine("PASS " + name); passed++; }
    catch (Exception e) { Console.Error.WriteLine("FAIL " + name + ": " + e); Environment.ExitCode = 1; }
}
void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected {expected}, received {actual}"); }
void Throws<T>(Action action) where T : Exception { try { action(); } catch (T) { return; } throw new Exception("Missing " + typeof(T).Name); }

Check("1920x1200 is the default, not silent 1080p", () => { var p = RemotePolicy.Preset(); Equal(1920, p.Width); Equal(1200, p.Height); Equal(60, p.Fps); Equal(false, p.Hdr); });
Check("120 FPS changes rate, not geometry", () => { var p = RemotePolicy.Preset(120); Equal(1200, p.Height); Equal(40, p.BitrateMbps); });
Check("unknown preset fails", () => Throws<ArgumentOutOfRangeException>(() => RemotePolicy.Preset(240)));
Check("host Ethernet cannot certify client route", () => Equal(true, RemotePolicy.WiredAssessment(new(true)).StartsWith("Unverified", StringComparison.Ordinal)));
Check("no host Ethernet cannot certify route", () => Equal(true, RemotePolicy.WiredAssessment(new(false, true, true)).StartsWith("Unverified", StringComparison.Ordinal)));
Check("client failure remains failure", () => Equal(true, RemotePolicy.WiredAssessment(new(true, true, false)).StartsWith("Not established", StringComparison.Ordinal)));
Check("operator observation is not latency proof", () => Equal(true, RemotePolicy.WiredAssessment(new(true, true, true)).Contains("unmeasured", StringComparison.Ordinal)));
Check("unknown capture rejected", () => Throws<InvalidOperationException>(() => RemotePolicy.RequireCaptureEvidence(new(true))));
Check("wrong capture rejected", () => Throws<InvalidOperationException>(() => RemotePolicy.RequireCaptureEvidence(new(true, true, true, false))));
Check("explicit observed capture accepted", () => RemotePolicy.RequireCaptureEvidence(new(true, true, true, true)));
Check("two screens are shared session", () => Equal("Shared Windows Session", RemotePolicy.SessionLabel(WorkspaceMode.Game)));
Check("independent seat not fabricated", () => Equal(true, RemotePolicy.SessionLabel(WorkspaceMode.IndependentSeat).Contains("not provisioned", StringComparison.Ordinal)));
Check("loopback controller preserved", () => Equal("http://127.0.0.1:7331/", Destinations.Resolve("dashboard")));
Check("arbitrary action rejected", () => Throws<ArgumentException>(() => Destinations.Resolve("https://example.invalid/execute")));
Check("public report contains only allowlisted fields", () => {
    using var d = JsonDocument.Parse(PublicDiagnostics.Export(2, true, null, false));
    var keys = d.RootElement.EnumerateObject().Select(p => p.Name).Order().ToArray();
    Equal("clientRoute,dashboardPortOpen,displayCount,hostEthernetUp,installedAcceptance,performance,schema,serviceIdentity,streaming,sunshinePortOpen", string.Join(',', keys));
    Equal(JsonValueKind.Null, d.RootElement.GetProperty("dashboardPortOpen").ValueKind);
});
Check("invalid report geometry count rejected", () => Throws<ArgumentOutOfRangeException>(() => PublicDiagnostics.Export(-1, false, null, null)));
Check("recovered refresh picks highest available", () => Equal<int?>(120, DisplayModeSelector.ChooseRefresh(new[] { 60, 120 }, null)));
Check("recovered refresh filters invalid entries", () => Equal<int?>(null, DisplayModeSelector.ChooseRefresh(new[] { 0, 1 }, 60)));
Check("recovered refresh prefers upper tie", () => Equal<int?>(120, DisplayModeSelector.ChooseRefresh(new[] { 60, 120 }, 90)));
Check("recovered quote handles empty", () => Equal("\"\"", WindowsCommandLine.QuoteArgument("")));
Check("recovered quote handles spaces", () => Equal("\"a b\"", WindowsCommandLine.QuoteArgument("a b")));
Check("recovered quote escapes embedded quote", () => Equal("\"a\\\"b\"", WindowsCommandLine.QuoteArgument("a\"b")));
Check("recovered quote doubles trailing slash", () => Equal("\"a b\\\\\"", WindowsCommandLine.QuoteArgument("a b\\")));
Check("turbo history is per-controller", () => {
    var t = new TurboDetector(edgeThreshold: 2); var now = DateTime.UnixEpoch;
    Equal(false, t.Observe(0, true, now)); Equal(true, t.Observe(0, false, now.AddMilliseconds(100)));
    Equal(false, t.Observe(1, false, now.AddMilliseconds(200))); t.Reset(0); Equal(false, t.Observe(0, false, now.AddMilliseconds(300)));
});
Check("turbo edges expire", () => {
    var t = new TurboDetector(edgeThreshold: 2); var now = DateTime.UnixEpoch;
    t.Observe(0, true, now); Equal(false, t.Observe(0, false, now.AddSeconds(2)));
});
Check("tailscale direct/relay counts ignore inactive peer", () => {
    var p = TailscaleStatusParser.Parse("""{"Peer":{"a":{"Active":true,"CurAddr":"test-endpoint"},"b":{"Active":true,"Relay":"test-region"},"c":{"Active":false,"CurAddr":"test-endpoint"}}}""");
    Equal(1, p.DirectPeers); Equal(1, p.RelayPeers);
});
Check("tailscale aggregate is not used as route evidence", () => Equal(true, RemotePolicy.WiredAssessment(new(true)).StartsWith("Unverified", StringComparison.Ordinal)));
Console.WriteLine($"{passed}/27 checks passed");
