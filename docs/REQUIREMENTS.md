# Recovered requirements and acceptance boundaries

Derived from the complete September 25 Standalone Phone Monitor / Remote Gaming Seat specification and the October 12 iPad request. This is a requirements extraction, not a claim of existing feature coverage. Hardware/backend assertions in the earlier specification are researched separately in RECOVERY.md.

| Area | Preserved requirement | Current state / next acceptance |
| --- | --- | --- |
| Display | Companion mode defaults to a separate client display; physical panel remains active, primary and usable. Prefer 1920x1200; refresh 60/120 separately. No 800x600 or physical-screen fallback. | Geometry policy and capture guard tested; native topology creation, exact target and rollback not implemented in new app. |
| iPad desk | Wired extended display with touch and real Toolbox window; keep privileged controller on loopback. | Guides and fixed links; live spacedesk USB/Extend/touch acceptance pending. |
| Streaming | Reuse pairing; distinguish USB display data, Ethernet LAN and Tailscale routes; report measured rather than guessed quality. | Host observations remain explicitly insufficient. Client radios-off survival and Moonlight statistics still required. |
| Independent seat | Shared Windows session is not isolated input. Clicking local browser must not steal remote game controller/rendering; advanced seat only with a real separately verified backend. | No independent seat provisioned; no foreground-stealing loops or OS session patches. |
| Game launching | Per-game executable/App ID, display/mode/rate/controller/audio profile; identify actual render process; no moving unrelated windows. | Recovered requirement; launcher and exact-window routing not yet ported. |
| Controller | Proper virtual gamepad, optional type, buttons/sticks/triggers/turbo diagnostics; no unsafe global injection or anti-cheat interference. | Turbo helper recovered/tested; actual controller backend and focus acceptance pending. |
| Audio | Client-only/PC-only/both, restore prior endpoint on disconnect; show actual endpoint. | Not implemented in new app; requires session-scoped snapshot/restore. |
| Lifecycle | Event-driven connect/disconnect, per-client identity, 10-20 second configurable reconnect grace, avoid duplicate displays/restarts, do not kill game by default. | Not ported; first implementation must include timeout, cancellation, idempotency and crash recovery. |
| Backend | Compare Sunshine+VDD with Apollo/SudoVDA; protect existing configuration and pairing; do not stack conflicting drivers. | Research decision preserved; no automatic migration. |
| Recovery | Last-known-good topology before mutation; no destructive GraphicsDrivers cache reset; verify physical display after any error. | Legacy scripts deliberately excluded. Native fault/rollback proof required before enabling changes. |
| User interface | Small native tray app, near-zero routine manual intervention, responsive UI, visible failures and optional advanced diagnostics. | Native companion increment; not the full automation UI. |
| Delivery | Source in owning repo, signed exact-version update/rollback, startup only when requested, quiet processes and Heaven Tools launchers. | Source/CI candidate; release, installation and update channel pending. |

The longer-term request also includes remembered client/game profiles, send/bring-window controls, per-game compatibility history, audio selection, reconnect to running games, sleep inhibition only while streaming, optional Wake-on-LAN, screenshots, favorites, HDR/scaling and warnings. These remain documented backlog, not silently discarded and not auto-enabled by this recovery.

## Native acceptance order

1. Recover accessible latest PhoneSeat source and reconcile differences; keep v1.9 provenance rather than call it v1.13.
2. Build/test exact candidate and independently review hardware/privacy boundaries.
3. On the intended machine, identify current installed Sunshine/VDD/spacedesk and display topology without mutation.
4. Prove wired desk mode, touch and loopback dashboard; retain the physical panel.
5. Prove client Ethernet route with radios disabled, correct capture and stable 60 FPS, then separately measure 120 FPS. Record encode/decode/network measurements without calling them full input-to-photon latency.
6. Exercise work/game/work, disconnect/reconnect, failed startup and rollback; test controller input while local browser is focused. Mark incompatible shared-session games honestly.
7. Independently authorize integration after the operator lifts the freeze; release/install only with exact artifact and rollback readback.
