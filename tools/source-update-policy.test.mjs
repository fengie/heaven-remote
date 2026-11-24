// Source-delivery contract only: a checkout is not an installed runtime.
import test from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';

test('canonical source sync is fail-closed and grants no unattended runtime activation', () => {
  const policy = JSON.parse(readFileSync(new URL('../.heaven/update-policy.json', import.meta.url), 'utf8'));
  assert.deepEqual(policy, {
    "schema": "heaven-toolbox/update-policy/v1",
    "repository": "fengie/heaven-remote",
    "branch": "main",
    "kind": "source",
    "source_update": {
      "required": true,
      "strategy": "ff-only",
      "poll_interval_minutes": 1,
      "controller": "heaven-toolbox/repo-autoupdate"
    },
    "runtime_update": {
      "mode": "none"
    }
  });
});
