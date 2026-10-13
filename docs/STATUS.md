# Source recovery checkpoint — 2026-10-12 America/New_York

Owner scope: `feat/recover-remote-companion-20261012` in this repository. Baseline main: `b36c9617f96bfa9cb87d578e7db8130853dab9c2`. Research checkpoint: `6c1bf02b882efd7d66a0a211ca5cec421a2d044e`. Toolbox basis: `5a14ce8f25628d4a0ce59b431ae353400d0004cc`.

Recovered: exact PhoneSeat v1.9 and Moonlight v20 archives, the full September specification and the superseded iPad preparation helper. Selected PhoneSeat source is hash-bound in recovery-manifest.json. The unretrievable v1.13 archive is an explicit gap, not presumed equivalent. No raw archive, private log, Bridge receipt or credential is published.

Implemented: native companion UI, read-only observations, fixed destination actions, privacy-minimized export, safe shortcut creation, tray lifecycle, 1920x1200 policy, four recovered helpers, 27 dependency-free C# checks and a portable extraction/privacy source gate with 12 Node regressions. This companion guides setup; it does not perform unattended switching or provision an independent gaming seat.

Verification at authoring: 12 Node regressions passed in the isolated Linux authoring environment. Native .NET/Windows execution is not available there; the checked-in hosted Windows CI is the required independent build/test surface. Read exact PR-head checks rather than treating this document as a green badge. Any failing/pending job remains unverified. Source is not installation, hardware acceptance or a release.

Reusable lessons are proposed back to Toolbox on `fix/remote-extraction-lessons-20261012`, using existing lesson IDs rather than a second policy authority. The same standalone source guard is vendored here; runtime enforcement or signed judicial admission is not claimed. Record exact CI outcomes on the PR without rewriting release metadata.

Remaining external boundaries: inaccessible PhoneSeat v1.13 bytes; native CI results until executed; real iPad/controller/display/network tests; independent safety review; active operator merge freeze; signed release/update/rollback and installed acceptance. Do not bypass any boundary to label this complete.
