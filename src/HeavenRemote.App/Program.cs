using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using HeavenRemote.Core;

namespace HeavenRemote.App;

internal static class Program
{
    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.SequenceEqual(new[] { "--self-test" }))
        {
            using var form = new RemoteWindow();
            form.CreateControl();
            return form.Text == "Heaven Remote" && form.Controls.Count > 0 ? 0 : 1;
        }
        if (args.Length != 0) return 2;
        Application.Run(new RemoteWindow());
        return 0;
    }
}

internal sealed class RemoteWindow : Form
{
    private readonly Label status = new() { AutoSize = true, MaximumSize = new Size(800, 0) };
    private readonly TextBox guidance = new() { Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, Dock = DockStyle.Fill };
    private readonly Button refresh = new() { Text = "Inspect this PC", AutoSize = true };
    private readonly CancellationTokenSource lifetime = new();
    private readonly NotifyIcon tray;
    private readonly ContextMenuStrip trayMenu;
    private bool resourcesDisposed;
    private string? report;
    private bool inspecting;

    public RemoteWindow()
    {
        Text = "Heaven Remote";
        MinimumSize = new Size(640, 520);
        Size = new Size(880, 670);
        StartPosition = FormStartPosition.CenterScreen;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(16) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var heading = new Label { Text = "iPad companion • source candidate", AutoSize = true };
        var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, WrapContents = true };
        Add(buttons, "Work guide", () => Guide(WorkspaceMode.Work));
        Add(buttons, "Game guide", () => Guide(WorkspaceMode.Game));
        Add(buttons, "Independent seat", () => Guide(WorkspaceMode.IndependentSeat));
        Add(buttons, "Toolbox dashboard", () => Open("dashboard"));
        Add(buttons, "Sunshine settings", () => Open("sunshine"));
        Add(buttons, "Display settings", () => Open("display"));
        Add(buttons, "USB instructions", () => Open("spacedesk"));
        Add(buttons, "Apple device support", () => Open("apple"));
        Add(buttons, "Moonlight instructions", () => Open("moonlight"));
        refresh.Click += async (_, _) => await InspectAsync();
        buttons.Controls.Add(refresh);
        Add(buttons, "Save safe report", SaveReport);
        Add(buttons, "Create Heaven Tools shortcut", CreateShortcut);
        Add(buttons, "Minimize to tray", () => { Hide(); tray!.Visible = true; });
        status.Text = "Not inspected. No display, network, driver, service or pairing changes are made.";
        layout.Controls.Add(heading, 0, 0);
        layout.Controls.Add(buttons, 0, 1);
        layout.Controls.Add(status, 0, 2);
        layout.Controls.Add(guidance, 0, 3);
        Controls.Add(layout);
        var menu = new ContextMenuStrip();
        trayMenu = menu;
        menu.Items.Add("Open", null, (_, _) => { Show(); WindowState = FormWindowState.Normal; });
        menu.Items.Add("Exit", null, (_, _) => Close());
        tray = new NotifyIcon { Text = "Heaven Remote", Icon = SystemIcons.Application, ContextMenuStrip = menu };
        tray.DoubleClick += (_, _) => { Show(); WindowState = FormWindowState.Normal; };
        Shown += async (_, _) => await InspectAsync();
        FormClosing += (_, _) => lifetime.Cancel();
        Guide(WorkspaceMode.Work);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !resourcesDisposed)
        {
            resourcesDisposed = true;
            lifetime.Cancel();
            tray.Visible = false;
            tray.Dispose();
            trayMenu.Dispose();
            lifetime.Dispose();
        }
        base.Dispose(disposing);
    }

    private static void Add(Control parent, string text, Action action)
    {
        var button = new Button { Text = text, AutoSize = true };
        button.Click += (_, _) =>
        {
            try { action(); }
            catch (Exception error) when (error is not OutOfMemoryException)
            { MessageBox.Show("Action did not complete (" + error.GetType().Name + "). No retry was started.", "Heaven Remote"); }
        };
        parent.Controls.Add(button);
    }

    private void Guide(WorkspaceMode mode)
    {
        var p = RemotePolicy.Preset();
        guidance.Text = RemotePolicy.SessionLabel(mode) + "\r\n\r\n" + (mode switch
        {
            WorkspaceMode.Work => "Unlock and trust the iPad. In spacedesk Driver Console enable USB Cable iOS, reconnect, then connect the iPad Viewer. Use Windows Extend, not Duplicate. Keep the physical panel primary.\r\n\r\nOpen Toolbox in this PC's browser and move that window onto the iPad display. Loopback in iPad Safari points to the iPad, not this PC.\r\n\r\nVerify the cable path by temporarily disabling iPad Wi-Fi/cellular in Settings. This application cannot verify iPad touch or USB transport from Windows alone.",
            WorkspaceMode.Game => $"Disconnect the spacedesk viewing session; run native Moonlight on the iPad. Reuse the current Sunshine host and pairing. Start at {p.Width}x{p.Height}, {p.Fps} FPS, {p.BitrateMbps} Mbps, {p.Codec}, HDR off. 120 FPS / 40 Mbps is an optional test, not a measured optimum.\r\n\r\nFor a wired baseline connect BOTH devices to Ethernet. USB charging or a host Ethernet adapter alone proves nothing about Moonlight's route. Verify the client Ethernet address and stream survival with client radios disabled. Inspect client statistics separately.\r\n\r\nCheck the actual capture display. Never silently substitute the physical desktop. Clicking another Windows application can change input focus; two displays do not create independent sessions.",
            _ => "Not provisioned. Apollo/SudoVDA can separate display identities but that does not establish independent input/audio sessions. A separate Windows-seat backend needs current compatibility, licensing, rollback and native acceptance. No RDP patch, injection or automatic driver migration is performed."
        });
        guidance.Text += "\r\n\r\nThese buttons select guidance, NOT an active streaming mode. No software is installed or reconfigured. Unknown measurements remain unknown.";
    }

    private static void Open(string action) => Process.Start(new ProcessStartInfo(Destinations.Resolve(action)) { UseShellExecute = true });

    private async Task InspectAsync()
    {
        if (inspecting || lifetime.IsCancellationRequested) return;
        inspecting = true; refresh.Enabled = false;
        var cancellation = lifetime.Token;
        try
        {
            // Capture UI-thread display facts, then perform bounded network work off the UI thread.
            var screens = Screen.AllScreens;
            var geometry = string.Join("; ", screens.Select(s => $"{s.Bounds.Width}x{s.Bounds.Height}{(s.Primary ? " primary" : "")}"));
            var ethernet = NetworkInterface.GetAllNetworkInterfaces().Any(n => n.OperationalStatus == OperationalStatus.Up &&
                n.NetworkInterfaceType is NetworkInterfaceType.Ethernet or NetworkInterfaceType.GigabitEthernet or NetworkInterfaceType.FastEthernetFx or NetworkInterfaceType.FastEthernetT);
            var probes = await Task.WhenAll(PortOpenAsync(7331, cancellation), PortOpenAsync(47990, cancellation));
            if (cancellation.IsCancellationRequested || IsDisposed) return;
            report = PublicDiagnostics.Export(screens.Length, ethernet, probes[0], probes[1]);
            status.Text = $"Windows desktop geometry: {geometry}. Host Ethernet Up: {ethernet}. " +
                $"Loopback ports — Toolbox: {Label(probes[0])}; Sunshine: {Label(probes[1])}. " +
                RemotePolicy.WiredAssessment(new Readiness(ethernet)) + ". Ports are not authenticated service health.";
        }
        catch (OperationCanceledException) { report = null; }
        catch (Exception error) when (error is NetworkInformationException or InvalidOperationException or System.ComponentModel.Win32Exception)
        { report = null; if (!IsDisposed) status.Text = "Inspection incomplete; some observations unavailable. No changes made."; }
        finally
        { inspecting = false; if (!IsDisposed && !cancellation.IsCancellationRequested) refresh.Enabled = true; }
    }

    private static string Label(bool? value) => value switch { true => "reachable", false => "closed", _ => "unknown" };

    private static async Task<bool?> PortOpenAsync(int port, CancellationToken cancellation)
    {
        using var client = new TcpClient(AddressFamily.InterNetwork);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        timeout.CancelAfter(TimeSpan.FromMilliseconds(750));
        try { await client.ConnectAsync(IPAddress.Loopback, port, timeout.Token); return true; }
        catch (OperationCanceledException) when (!cancellation.IsCancellationRequested) { return null; }
        catch (SocketException error) when (error.SocketErrorCode == SocketError.ConnectionRefused) { return false; }
        catch (SocketException) { return null; }
    }

    private void SaveReport()
    {
        if (report is null) { MessageBox.Show("Run a successful inspection first.", "Heaven Remote"); return; }
        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HeavenRemote", "diagnostics");
        Directory.CreateDirectory(folder);
        var destination = Path.Combine(folder, $"readiness-{DateTime.UtcNow:yyyyMMddTHHmmssZ}-{Guid.NewGuid():N}.json");
        using var stream = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        using var writer = new StreamWriter(stream);
        writer.Write(report); writer.Flush();
        MessageBox.Show("Saved an allowlisted readiness report under LocalAppData\\HeavenRemote\\diagnostics. No IPs, machine names, paths, credentials or raw logs are included.", "Heaven Remote");
    }

    private static void CreateShortcut()
    {
        var exe = Environment.ProcessPath;
        if (exe is null || !string.Equals(Path.GetFileName(exe), "HeavenRemote.exe", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Run the published native application to create its shortcut.");
        var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        if (string.IsNullOrWhiteSpace(desktop)) throw new InvalidOperationException("Desktop unavailable.");
        var folder = Path.Combine(desktop, "Heaven Tools"); Directory.CreateDirectory(folder);
        var link = Path.Combine(folder, "Heaven Remote.lnk");
        if (File.Exists(link)) { MessageBox.Show("Existing shortcut preserved; it was not overwritten.", "Heaven Remote"); return; }
        object? shell = null, shortcut = null;
        var temporary = Path.Combine(folder, $".heaven-remote-{Guid.NewGuid():N}.lnk");
        try
        {
            var type = Type.GetTypeFromProgID("WScript.Shell") ?? throw new InvalidOperationException("Shortcut provider unavailable.");
            shell = Activator.CreateInstance(type) ?? throw new InvalidOperationException("Shortcut provider unavailable.");
            dynamic api = shell;
            shortcut = api.CreateShortcut(temporary);
            dynamic item = shortcut;
            item.TargetPath = exe; item.WorkingDirectory = Path.GetDirectoryName(exe); item.Description = "Heaven Remote companion"; item.Save();
            File.Move(temporary, link, false); // No overwrite if another writer wins.
        }
        finally
        {
            try { if (shortcut is not null) Marshal.FinalReleaseComObject(shortcut); }
            catch (InvalidComObjectException) { Debug.WriteLine("Shortcut COM cleanup incomplete."); }
            try { if (shell is not null) Marshal.FinalReleaseComObject(shell); }
            catch (InvalidComObjectException) { Debug.WriteLine("Shell COM cleanup incomplete."); }
            try { if (File.Exists(temporary)) File.Delete(temporary); }
            catch (IOException) { Debug.WriteLine("Owned temporary shortcut cleanup incomplete."); }
            catch (UnauthorizedAccessException) { Debug.WriteLine("Owned temporary shortcut cleanup denied."); }
        }
    }
}
