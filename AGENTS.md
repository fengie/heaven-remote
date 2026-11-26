# Agent instructions — heaven-remote

Canonical product repository: `fengie/heaven-remote@main`. All remote-desktop, Sunshine/Moonlight, network, operator UI and other **project-specific** research, evidence, designs, code, tests and release plans belong here, not in the shared agent factory.

Read the live canonical Toolbox `AGENTS.md` and `GLOBAL_GIT_DIRECTIVE.md` for current generic instructions and verified ownership before mutable work. The subscription below retains the separately reviewed inherited capability revision. Preserve this repository's exact source and incoming work boundaries; do not assume a repo's visibility or GitHub permission establishes user ownership.

Only independently reusable engineering lessons, general agent/Bridge/bootstrap platform implementation and generic regression controls should be promoted to Toolbox, with a product-local source link and an explicit benefit over existing shared controls. Do not copy project state, device-specific data, logs, credentials or private artifacts into generic training. New repository identities follow the current Toolbox knowledge-routing bootstrap; unresolved identity or missing write authorization is a HOLD, never permission to use Toolbox as a default product output bucket.

For product work, research and compare, implement and test in this repository, persist dated source-backed findings and an exact remote checkpoint, then follow actual CI/review/main/deployment acceptance. Creating a branch or PR does not prove integration or installed behavior. No unsolicited automation, schedules, external communications, or changes to unrelated machines/projects.

## Research at task start — live shared knowledge

The [subscription](.heaven/toolbox-subscriptions.json) identifies reviewed inherited capability lineage. It is not a freshness pin for general policies, engineering lessons, research or reusable practices. Before each task, verify the live canonical `fengie/heaven-toolbox@main` SHA using the existing authorized GitHub/Toolbox route, read the current shared entry/training and task-relevant knowledge, and preserve the separate capability pin. Follow the [shared knowledge lifecycle and portfolio lookup](https://github.com/fengie/heaven-toolbox/blob/main/tools/knowledge-lifecycle/README.md).

For an authorized clean checkout in `TOOLBOX_ROOT`, set `TOOLBOX_SOURCE_SHA` from the independently verified live main for this task (not from a prior task or the subscription), and `TASK` to the actual task. The existing shared reader binds the result to that exact source:

```powershell
$subscription = Get-Content .heaven/toolbox-subscriptions.json -Raw | ConvertFrom-Json
$toolbox = $env:TOOLBOX_ROOT
$expected = [string]$env:TOOLBOX_SOURCE_SHA
if (-not $toolbox -or -not $env:TASK -or $expected -cnotmatch '^[a-f0-9]{40}$') {
    throw 'Set TOOLBOX_ROOT, TASK and the freshly verified canonical TOOLBOX_SOURCE_SHA.'
}
$revision = git -C $toolbox rev-parse HEAD
if ($LASTEXITCODE -ne 0 -or $revision -cne $expected) {
    throw 'Reconcile the authorized Toolbox checkout to verified current main before shared lookup.'
}
node (Join-Path $toolbox 'tools/agent-control/research-preflight.mjs') --root $toolbox --source-sha $revision --scope $subscription.repository --task $env:TASK
if ($LASTEXITCODE -ne 0) { throw 'Shared lookup failed; do not use unverified research results.' }
```

Connector-only callers use the same authorized exact-source evidence; do not claim this command executed when it did not. If access is unavailable, record the specific knowledge gap and continue authorized child-owned work without inventing latest research or bypassing managed admission. Compare relevant source/tests and actual applicability before reuse. Keep private lookup data and raw evidence in the authorized workspace, not public commits or logs. The reader does not grant source transfer, credentials, host access or mutation authority.

At meaningful task/incident closeout, return compact source-linked good practices, failures, rejected alternatives, unverified findings and local-only dispositions via the existing [Child Feedback contract](https://github.com/fengie/heaven-toolbox/blob/main/tools/app-platform/CHILD_FEEDBACK.md) and original issue. Keep the actual child commit and the actual inherited subscription pin in the packet; record the live parent consulted separately. Only verified, reviewed transferable practices can become shared policy or capability implementations. Never advance the subscription merely to retrieve knowledge. Preserve child-specific data, source, acceptance and safe update/release boundaries.

## Recovered companion acceptance and safety

Read `docs/RECOVERY.md` and `docs/STATUS.md` before changing the recovered companion. Archive provenance is not fresh runtime evidence. Verify `recovery-manifest.json` using the existing extraction/privacy gate and its tests, then run the exact native .NET/Windows checks. This source-only guard is not installed host authority. Never label guidance selection as an active streaming-mode transition.

Keep the physical screen usable and preserve the 1920x1200 preference. A display is not a separate input session; a cable is not a verified stream route; a reachable port is not authenticated service identity. Profile/driver changes require backup, rollback, exact target and native readback. Do not silently fall back to capturing another monitor. Preserve local Bridge approval/HMAC boundaries. Do not add an unsolicited public controller listener, startup agent, driver migration or automatic rollout. Public source must not contain private raw logs, client addresses, user paths, credentials or Bridge receipts.

The recovered companion remains subject to its recorded PR-specific release and installed-acceptance hold; reconciling source does not clear that hold. Follow current operator direction and native admission rather than treating the historical October incident freeze as a global rule.
