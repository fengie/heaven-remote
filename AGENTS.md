# Agent instructions — heaven-remote

Canonical product repository: `fengie/heaven-remote@main`. All remote-desktop, Sunshine/Moonlight, network, operator UI and other **project-specific** research, evidence, designs, code, tests and release plans belong here, not in the shared agent factory.

Read `AGENTS.md` and `GLOBAL_GIT_DIRECTIVE.md` at the reviewed Toolbox revision in the subscription below for generic instructions and verified ownership before mutable work. Preserve this repository's exact source and incoming work boundaries; do not assume a repo's visibility or GitHub permission establishes user ownership.

Only independently reusable engineering lessons, general agent/Bridge/bootstrap platform implementation and generic regression controls should be promoted to Toolbox, with a product-local source link and an explicit benefit over existing shared controls. Do not copy project state, device-specific data, logs, credentials or private artifacts into generic training. New repository identities follow the current Toolbox knowledge-routing bootstrap; unresolved identity or missing write authorization is a HOLD, never permission to use Toolbox as a default product output bucket.

For product work, research and compare, implement and test in this repository, persist dated source-backed findings and an exact remote checkpoint, then follow actual CI/review/main/deployment acceptance. Creating a branch or PR does not prove integration or installed behavior. No unsolicited automation, schedules, external communications, or changes to unrelated machines/projects.

## Research at task start

Use the reviewed revision and repository identity in [the subscription](.heaven/toolbox-subscriptions.json). Run the following from the child repository root with an authorized, clean checkout of the canonical Toolbox repository in `TOOLBOX_ROOT`. Set `TASK` to the actual task. The PowerShell example verifies the checkout revision before running the lookup.

```powershell
$subscription = Get-Content .heaven/toolbox-subscriptions.json -Raw | ConvertFrom-Json
$toolbox = $env:TOOLBOX_ROOT
if (-not $toolbox -or -not $env:TASK) { throw 'Set TOOLBOX_ROOT and TASK first.' }
$revision = git -C $toolbox rev-parse HEAD
if ($LASTEXITCODE -ne 0 -or $revision -ne $subscription.toolboxRevision) { throw 'Check out the reviewed Toolbox revision.' }
node (Join-Path $toolbox 'tools/agent-control/research-preflight.mjs') --root $toolbox --source-sha $revision --scope $subscription.repository --task $env:TASK
if ($LASTEXITCODE -ne 0) { throw 'Shared lookup failed. Reconcile the parent checkout before using its findings.' }
```

Inspect matching evidence against this repository's source and tests. When there is no match or private parent access is unavailable, continue with the child sources and record the gap. During research compare reuse, a small change and keeping the present behavior. On interruption preserve the exact source, checks and next action in the existing issue or PR. Verify user-visible outcomes locally and send only independently verified transferable findings through the parent's existing child-feedback route.

Keep the private lookup output in the authorized workspace. Public checkpoints may cite public child evidence and sanitized outcomes.

## Recovered companion acceptance and safety

Read `docs/RECOVERY.md` and `docs/STATUS.md` before changing the recovered companion. Archive provenance is not fresh runtime evidence. Verify `recovery-manifest.json` using the existing extraction/privacy gate and its tests, then run the exact native .NET/Windows checks. This source-only guard is not installed host authority. Never label guidance selection as an active streaming-mode transition.

Keep the physical screen usable and preserve the 1920x1200 preference. A display is not a separate input session; a cable is not a verified stream route; a reachable port is not authenticated service identity. Profile/driver changes require backup, rollback, exact target and native readback. Do not silently fall back to capturing another monitor. Preserve local Bridge approval/HMAC boundaries. Do not add an unsolicited public controller listener, startup agent, driver migration or automatic rollout. Public source must not contain private raw logs, client addresses, user paths, credentials or Bridge receipts.

The recovered companion remains subject to its recorded PR-specific release and installed-acceptance hold; reconciling source does not clear that hold. Follow current operator direction and native admission rather than treating the historical October incident freeze as a global rule.
