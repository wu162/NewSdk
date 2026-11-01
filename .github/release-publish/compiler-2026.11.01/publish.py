import hashlib
import json
import mimetypes
import os
from pathlib import Path
import shutil
import urllib.parse
import urllib.request
import zipfile

REPO = 'wu162/NewSdk'
TAG = 'compiler-2026.11.01'
RELEASE = 430534056
TARGET = '31ce106c90b90416855187768b22313dc1ad610c'
API = 'https://api.github.com/repos/' + REPO
headers = {'Authorization': 'Bearer ' + os.environ['GH_TOKEN'], 'Accept': 'application/vnd.github+json', 'X-GitHub-Api-Version': '2022-11-28'}


def call(url, method='GET', data=None, content_type=None):
    request_headers = dict(headers)
    if content_type:
        request_headers['Content-Type'] = content_type
    request = urllib.request.Request(url, headers=request_headers, method=method, data=data)
    with urllib.request.urlopen(request, timeout=120) as response:
        return json.load(response)


def need(value, message):
    if not value:
        raise ValueError(message)


assets = Path('work/assets')
publication = Path('.github/release-publish/compiler-2026.11.01')
for name in ('release-metadata.json', 'USAGE.zh-CN.md', 'SHA256SUMS.txt'):
    shutil.copyfile(publication / name, assets / name)
allowed = {'NewModCompiler-Win32-Debug.zip', 'NewModCompiler-Win32-Release.zip', 'NewModCompiler-Win32-Debug.manifest.json', 'NewModCompiler-Win32-Release.manifest.json', 'release-metadata.json', 'USAGE.zh-CN.md', 'SHA256SUMS.txt'}
need({path.name for path in assets.iterdir()} == allowed, 'Publication asset inventory differs')
for line in (assets / 'SHA256SUMS.txt').read_text().splitlines():
    digest, name = line.split('  ', 1)
    need(hashlib.sha256((assets / name).read_bytes()).hexdigest() == digest, 'Publication checksum differs: ' + name)
approved = {'Debug': '8c1b84505185d877a40bc8e7401a3b8335fbb04e7d0880e25017f94f0bc1506e', 'Release': '1ac1e6fac89aeab0e12035b7894760c2ac243fef17506c9f7f40f3acde8d18e6'}
for mode, expected in approved.items():
    path = assets / ('NewModCompiler-Win32-' + mode + '.zip')
    need(hashlib.sha256(path.read_bytes()).hexdigest() == expected, 'Approved runtime bytes differ')
    with zipfile.ZipFile(path) as runtime:
        need(len(runtime.infolist()) == 3 and set(runtime.namelist()) == {'NewModCompiler.exe', 'NewModCompiler.exe.config', 'antlr.runtime.dll'} and runtime.testzip() is None, 'Runtime ZIP inventory or CRC differs')
tag = call(API + '/git/ref/tags/' + TAG)['object']
if tag['type'] == 'tag':
    tag = call(API + '/git/tags/' + tag['sha'])['object']
need(tag['type'] == 'commit' and tag['sha'] == TARGET, 'Release tag target differs')
commit = call(API + '/git/commits/' + TARGET)
need(not commit['parents'], 'Source-free tag unexpectedly includes parent history')
tree = call(API + '/git/trees/' + commit['tree']['sha'])['tree']
need(len(tree) == 2 and {entry['path'] for entry in tree} == {'README.md', 'release-metadata.json'} and all(entry['type'] == 'blob' for entry in tree), 'Release snapshot contains unexpected files')
release = call(API + '/releases/' + str(RELEASE))
need(release['draft'] and release['tag_name'] == TAG and release['target_commitish'] == TARGET, 'Expected approved draft release differs')
need({item['name'] for item in release['assets']} <= allowed, 'Draft includes unexpected assets')
existing = {item['name']: item for item in release['assets']}
expected_assets = {}
for path in sorted(assets.iterdir()):
    data = path.read_bytes()
    digest = 'sha256:' + hashlib.sha256(data).hexdigest()
    expected_assets[path.name] = (len(data), digest)
    if path.name in existing:
        need(existing[path.name]['size'] == len(data) and existing[path.name]['digest'] == digest, 'Existing asset bytes differ')
        continue
    url = 'https://uploads.github.com/repos/' + REPO + '/releases/' + str(RELEASE) + '/assets?' + urllib.parse.urlencode({'name': path.name})
    uploaded = call(url, 'POST', data, mimetypes.guess_type(path.name)[0] or 'application/octet-stream')
    need(uploaded['state'] == 'uploaded' and uploaded['size'] == len(data) and uploaded['digest'] == digest, 'Upload digest differs: ' + path.name)
    print('Verified uploaded asset: ' + path.name, flush=True)
release = call(API + '/releases/' + str(RELEASE))
need({item['name'] for item in release['assets']} == allowed, 'Final asset inventory differs')
for item in release['assets']:
    need((item['size'], item['digest']) == expected_assets[item['name']] and item['state'] == 'uploaded', 'Final asset bytes differ')
release = call(API + '/releases/' + str(RELEASE), 'PATCH', json.dumps({'draft': False, 'prerelease': False, 'make_latest': 'true', 'body': (publication / 'RELEASE_NOTES.md').read_text()}).encode(), 'application/json')
need(not release['draft'] and not release['prerelease'], 'Release did not publish')
receipt = dict(releaseId=release['id'], url=release['html_url'], publishedAt=release['published_at'], tag=TAG, tagCommit=TARGET,
               binarySourceCommit='a75840ef7d0351cedc11c91b1b4510d295b911a7', approvedRuntimeZipsUnchanged=True,
               autoArchivesContainOnly=['README.md', 'release-metadata.json'], assets=[{key: asset[key] for key in ('id', 'name', 'size', 'digest', 'browser_download_url')} for asset in release['assets']])
Path('work/publication-receipt.json').write_text(json.dumps(receipt, indent=2) + '\n')
print(json.dumps(receipt, indent=2))
