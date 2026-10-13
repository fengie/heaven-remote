import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import os from 'node:os';
import path from 'node:path';
import { sha256, safeFile, assertPublicText, verifyExtraction, scanPublicTree } from './extraction-gate.mjs';
function fixture(t) {
  const root = fs.realpathSync(fs.mkdtempSync(path.join(os.tmpdir(), 'extraction-')));
  t.after(() => fs.rmSync(root, { recursive: true, force: true }));
  fs.mkdirSync(path.join(root, 'src'));
  fs.writeFileSync(path.join(root, 'src', 'sample.cs'), '// public source\n');
  const digest = sha256(Buffer.from('// public source\n'));
  const manifest = { schema: 'heaven/extraction/v1', visibility: 'public', delivery: 'source-candidate', imports: [
    { path: 'src/sample.cs', source: 'fixture', sourcePath: 'sample.cs', sourceSha256: digest, sha256: digest, transformation: 'identity' }
  ] };
  return { root, manifest };
}
test('matching bytes prove source only, never installed behavior', t => {
  const {root,manifest}=fixture(t);
  assert.deepEqual(verifyExtraction(root,manifest), {scope:'source-only', importedFiles:1, installationVerified:false, behaviorVerified:false});
  assert.equal(scanPublicTree(root).files,1);
});
test('changed bytes fail before publication', t => {
  const {root,manifest}=fixture(t); fs.appendFileSync(path.join(root,'src/sample.cs'),'changed');
  assert.throws(()=>verifyExtraction(root,manifest),/EXTRACTION_HASH/);
});
test('case-colliding import identities fail', t => {
  const {root,manifest}=fixture(t); manifest.imports.push({...manifest.imports[0],path:'src/SAMPLE.cs'});
  assert.throws(()=>verifyExtraction(root,manifest),/EXTRACTION_DUPLICATE/);
});
test('path traversal, Windows alias and empty components fail', t => {
  const {root}=fixture(t);
  for(const value of ['../file','/file','src//sample.cs','src/./sample.cs','src\\sample.cs','C:sample.cs'])
    assert.throws(()=>safeFile(root,value),/EXTRACTION_PATH/);
});
test('malformed identity hash fails', t => {
  const {root,manifest}=fixture(t); manifest.imports[0].sourceSha256='unknown';
  assert.throws(()=>verifyExtraction(root,manifest),/EXTRACTION_PROVENANCE/);
});
test('deployment claim cannot be inferred from extraction', t => {
  const {root,manifest}=fixture(t); manifest.delivery='installed';
  assert.throws(()=>verifyExtraction(root,manifest),/EXTRACTION_CLAIM/);
});
test('empty import list is not a recovery', t => {
  const {root,manifest}=fixture(t); manifest.imports=[];
  assert.throws(()=>verifyExtraction(root,manifest),/EXTRACTION_IMPORTS/);
});
test('symlinked parent and file are refused', t => {
  const {root}=fixture(t);
  try { fs.symlinkSync(path.join(root,'src'),path.join(root,'alias'),'junction'); }
  catch(error) { if(error.code==='EPERM') { t.skip('OS denies symlink fixture'); return; } throw error; }
  assert.throws(()=>safeFile(root,'alias/sample.cs'),/EXTRACTION_LINK/);
  assert.throws(()=>scanPublicTree(root),/EXTRACTION_LINK/);
});
test('semantic credentials and private paths are caught regardless of field name', () => {
  const canaries=['gh'+'p_'+'X'.repeat(36),'github_'+'pat_'+'X'.repeat(48),
    '-----BEGIN '+'PRIVATE KEY-----','http'+'s://u'+':pw@example.invalid',
    'Bearer '+'Y'.repeat(32),'C:'+'\\Users\\fixture\\state.json',
    [192,168,8,9].join('.'),[10,2,3,4].join('.'),[172,16,2,3].join('.')];
  for(const value of canaries) assert.throws(()=>assertPublicText(JSON.stringify({message:value}).replaceAll('\\\\','\\')),/EXTRACTION_PRIVATE_VALUE/);
});
test('loopback documentation remains publishable', () => {
  assert.doesNotThrow(()=>assertPublicText('http://127.0.0.1:7331/ and https://localhost:47990/'));
});
test('private report artifacts are excluded independently of contents', t => {
  const {root}=fixture(t); fs.writeFileSync(path.join(root,'BRIDGE-CHECK.txt'),'no secret');
  assert.throws(()=>scanPublicTree(root),/EXTRACTION_PRIVATE_ARTIFACT/);
});
test('invalid UTF8 and oversized files fail closed', t => {
  const {root}=fixture(t); const file=path.join(root,'bad.txt');
  fs.writeFileSync(file,Buffer.from([0xff])); assert.throws(()=>safeFile(root,'bad.txt'));
  fs.writeFileSync(file,Buffer.alloc(1024*1024+1)); assert.throws(()=>safeFile(root,'bad.txt'),/EXTRACTION_FILE/);
});
