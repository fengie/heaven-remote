# Verification evidence — 2026-10-12 America/New_York

## First native baseline

GitHub run `38340505806`, Windows job `115110185527`, attached to PR #1 candidate `208a3f096d343c9672f2bd22fb11e7b5b9031857`, completed successfully on a hosted Windows runner. Log readback recorded .NET SDK 10.0.401, 0 compilation warnings, 0 errors, 27/27 core checks, framework-dependent publish and successful native UI construction/disposal. The Ubuntu source job passed the 12 Node regressions and import/privacy scan.

Important correction: the PR workflow's default checkout was GitHub's synthetic merge `5a75c2b2cf91867d8b6701873d4f5a89d0da6a9a`, not the bare candidate commit. A check-run head label is not enough to establish the checked-out revision. This evidence is a native baseline associated with that candidate, not a standalone exact-head receipt.

## Prevention now in source

Both source and Windows jobs explicitly request the candidate SHA, then compare `git rev-parse HEAD` against the GitHub event SHA before testing. PR commits run once, rather than duplicate push and PR builds, and superseded runs are cancelled within the same PR only. Current acceptance must read the newer exact-head run and its verification step; the older green run must not be reused as proof for changed source.

The same lesson is promoted to Toolbox under existing lesson 34 (exact-head green) alongside source/installation distinction. No CI job mutates a user's display, pairing, driver, network, firewall or startup configuration. Passing native UI construction is not live streaming, input isolation, performance, publication or installed-product acceptance.
