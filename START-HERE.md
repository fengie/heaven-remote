# Heaven Remote — recovered companion source

This repository owns remote display, Sunshine/Moonlight integration, client profiles and product-specific research. Reusable engineering prevention belongs in Heaven Toolbox.

**Current delivery: source candidate, not an installed or fully automated streaming product.** The Toolbox merge freeze remains active. See [recovery and research](docs/RECOVERY.md), [requirements](docs/REQUIREMENTS.md), [status](docs/STATUS.md), and [import provenance](recovery-manifest.json).

## Implemented increment

The native .NET 10 Windows companion offers Work/Game instructions, fixed local Toolbox/Sunshine links, asynchronous bounded loopback-port inspection, display geometry and Ethernet presence, a privacy-minimized report, a non-overwriting shortcut under Desktop/Heaven Tools, and explicit tray controls. It never treats guidance selection as an active mode switch. It does not install software, create virtual displays, change topology, migrate drivers, alter pairing, change firewall/routing, launch an independent seat, or prove a live client route.

Four pure PhoneSeat v1.9 helpers are imported with original/destination hashes: display-rate selection, Windows argument quoting, per-controller turbo-edge detection and Tailscale peer parsing. Their presence is not a claim that their corresponding native controllers/backends are deployed. The old UI, mutable services and scripts remain inventory-only pending safe reconciliation with the inaccessible v1.13 source.

## Developer verification

Requires a supported .NET 10 SDK and Windows for the native app. No third-party runtime package is required.

```text
node --test tools/extraction-gate.test.mjs
node tools/extraction-gate.mjs .
dotnet build src/HeavenRemote.App/HeavenRemote.App.csproj -c Release --warnaserror
dotnet run --project tests/HeavenRemote.Checks/HeavenRemote.Checks.csproj -c Release
dotnet run --project src/HeavenRemote.App/HeavenRemote.App.csproj
```

Windows CI compiles the actual emitted source, runs the core checks, publishes a candidate locally on the disposable runner, and constructs/disposes the native UI with `--self-test`. That test deliberately does not modify host display/network settings and cannot replace device acceptance. CI uses hosted runners and read-only permissions, never private machines for public PR code. There is no download/execute bootstrap or unattended updater. A signed, rollback-capable release channel remains to be designed and verified before deployment.
