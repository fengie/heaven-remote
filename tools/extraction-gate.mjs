import fs from 'node:fs';
import path from 'node:path';
import { createHash } from 'node:crypto';
import { fileURLToPath } from 'node:url';

// Source-only extraction check. NOT a signer, installation receipt or universal DLP.
const HEX = /^[a-f0-9]{64}$/;
const MAX_FILE = 1024 * 1024;
const fail = code => { throw new Error(code); };
export const sha256 = bytes => createHash('sha256').update(bytes).digest('hex');

export function safeFile(root, relative) {
  if (typeof relative !== 'string' || !relative || relative.length > 240 ||
      /[\\:\x00-\x1f]/u.test(relative) || path.posix.isAbsolute(relative) ||
      relative.split('/').some(part => !part || part === '.' || part === '..')) fail('EXTRACTION_PATH');
  const base = path.resolve(root);
  if (fs.lstatSync(base).isSymbolicLink() || fs.realpathSync(base) !== base) fail('EXTRACTION_ROOT_ALIAS');
  let current = base;
  const parts = relative.split('/');
  for (let i = 0; i < parts.length; i++) {
    current = path.join(current, parts[i]);
    const st = fs.lstatSync(current);
    if (st.isSymbolicLink()) fail('EXTRACTION_LINK');
    if (i < parts.length - 1 && !st.isDirectory()) fail('EXTRACTION_PARENT');
    if (i === parts.length - 1 && (!st.isFile() || st.size > MAX_FILE)) fail('EXTRACTION_FILE');
  }
  const bytes = fs.readFileSync(current);
  if (bytes.length > MAX_FILE) fail('EXTRACTION_FILE');
  new TextDecoder('utf-8', { fatal: true }).decode(bytes);
  return bytes;
}

export function assertPublicText(text) {
  const patterns = [
    /gh[pousr]_[A-Za-z0-9]{30,}/u,
    /github_pat_[A-Za-z0-9_]{40,}/u,
    /-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----/u,
    /https?:\/\/[^\s/:@]+:[^\s/@]+@/iu,
    /\bBearer\s+[A-Za-z0-9._~-]{24,}/iu,
    /[A-Z]:\\(?:Users|Documents and Settings)\\[^\s\\"']+/iu,
    /\b(?:192\.168\.[0-9]{1,3}\.[0-9]{1,3}|10\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}|172\.(?:1[6-9]|2[0-9]|3[01])\.[0-9]{1,3}\.[0-9]{1,3})\b/u
  ];
  if (patterns.some(pattern => pattern.test(text))) fail('EXTRACTION_PRIVATE_VALUE');
}

export function verifyExtraction(root, manifest) {
  if (!manifest || manifest.schema !== 'heaven/extraction/v1' || manifest.visibility !== 'public' ||
      manifest.delivery !== 'source-candidate') fail('EXTRACTION_CLAIM');
  if (!Array.isArray(manifest.imports) || manifest.imports.length < 1 || manifest.imports.length > 256)
    fail('EXTRACTION_IMPORTS');
  const seen = new Set();
  for (const item of manifest.imports) {
    if (!item || typeof item.path !== 'string') fail('EXTRACTION_IMPORT');
    const key = item.path.toLowerCase();
    if (seen.has(key)) fail('EXTRACTION_DUPLICATE');
    seen.add(key);
    if (!HEX.test(item.sha256 ?? '') || !HEX.test(item.sourceSha256 ?? '') ||
        typeof item.source !== 'string' || !item.source.trim() ||
        typeof item.sourcePath !== 'string' || !item.sourcePath.trim() ||
        typeof item.transformation !== 'string' || !item.transformation.trim()) fail('EXTRACTION_PROVENANCE');
    const bytes = safeFile(root, item.path);
    if (sha256(bytes) !== item.sha256) fail('EXTRACTION_HASH');
    assertPublicText(bytes.toString('utf8'));
  }
  return { scope: 'source-only', importedFiles: seen.size, installationVerified: false, behaviorVerified: false };
}

export function scanPublicTree(root) {
  const ignore = new Set(['.git', 'bin', 'obj', 'artifacts', 'node_modules']);
  let files = 0, bytes = 0;
  function visit(relative) {
    const dir = relative ? path.join(root, relative) : root;
    for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
      if (ignore.has(entry.name.toLowerCase())) continue;
      const rel = relative ? relative + '/' + entry.name : entry.name;
      if (entry.isSymbolicLink()) fail('EXTRACTION_LINK');
      if (entry.isDirectory()) { visit(rel); continue; }
      if (/\.(?:zip|exe|dll|log|binlog|key|pfx)$/iu.test(entry.name) ||
          /^\.env(?:\.|$)/iu.test(entry.name) || /^bridge-check\.txt$/iu.test(entry.name)) fail('EXTRACTION_PRIVATE_ARTIFACT');
      const content = safeFile(root, rel);
      files++; bytes += content.length;
      if (files > 3000 || bytes > 8 * 1024 * 1024) fail('EXTRACTION_SCAN_BOUND');
      assertPublicText(content.toString('utf8'));
    }
  }
  visit('');
  return { files, bytes, scope: 'source-only', privacyReviewStillRequired: true };
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  try {
    if (process.argv.length > 3) fail('EXTRACTION_ARGUMENTS');
    const root = path.resolve(process.argv[2] ?? '.');
    const manifest = JSON.parse(safeFile(root, 'recovery-manifest.json').toString('utf8'));
    console.log(JSON.stringify({ ...verifyExtraction(root, manifest), scan: scanPublicTree(root) }, null, 2));
  } catch (error) { console.error(error.message); process.exitCode = 1; }
}
