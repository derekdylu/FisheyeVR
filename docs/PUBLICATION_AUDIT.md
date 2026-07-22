# Public-release audit

This ledger records the local working-tree review performed while consolidating
the historical Unity and analysis repositories. It does not cover GitHub-hosted
settings or artifacts that are not present in this checkout.

| ID | Severity | Surface | Finding | Evidence | Planned action | Status | Verification |
| --- | --- | --- | --- | --- | --- | --- | --- |
| PUB-001 | Critical | Research data | Raw and derived participant records used identifying filenames and were unsuitable for a public source tree. | Unity experiment-log and analysis result directories; executed notebook output | Exclude all records and outputs; ignore private data paths. | Closed in candidate tree | No experiment-result directories; notebooks have zero outputs and null execution counts. |
| PUB-002 | High | Unity assets | The working prototype contained Asset Store and license-unclear third-party source. | Local asset notices and third-party asset roots | Exclude source assets and document restoration requirements. | Closed in candidate tree | Excluded paths absent; exact paths are ignored. |
| PUB-003 | High | Reproducibility | Retained scenes reference removed licensed dependencies. | Scene YAML and project scripts reference BNG components and asset GUIDs. | Publish as a partial archival source snapshot; do not claim a turnkey build. | Accepted limitation | README and Unity README disclose the limitation. |
| PUB-004 | High | Historical repositories | The two source histories still contain participant data and third-party source. | Historical Git object inventory | Do not merge, push, or publish the old histories. Use a new clean publication history. | Closed locally; remote verification pending | The new repository starts from one sanitized root commit; confirm the publication remote contains only this history. |
| PUB-005 | High | License | A project-level license was initially unspecified. | Owner selected MIT on 2026-07-21. | Add the standard OSI MIT text and make README consistent without relicensing third-party content. | Closed | `LICENSE`, README, and third-party notice language consistently identify MIT and its scope. |
| PUB-006 | Medium | Analysis dependencies | The original dependency list was unpinned and contained an invalid standard-library entry. | Historical `requirements.txt` | Normalize package names and document the unpinned archival environment. | Closed with limitation | Requirements cover detected third-party imports; end-to-end execution remains pending. |
| PUB-007 | Medium | Code execution | Two notebooks constructed shell commands from paths. | `subprocess.run(..., shell=True)` | Invoke the current Python executable with an argument list. | Closed | No `shell=True` remains in analysis code. |
| PUB-008 | Medium | Unity generated files | Per-user Unity settings and build artifacts should not be public source. | `UserSettings` and packaged APK archive | Remove them and add Unity-aware ignore rules. | Closed | Generated paths and binary build archive absent. |
| PUB-009 | Medium | Remote hosting | GitHub alerts, Actions logs/artifacts, releases, Pages, wiki, packages, issue attachments, and repository visibility were not reviewed. | Not available in the local checkout | Review all hosting surfaces before changing visibility. | Open | Complete a host-side checklist immediately before release. |
| PUB-010 | Medium | Verification | A licensed Unity import/build and full analysis run cannot be performed from the sanitized snapshot alone. | Required assets and private data intentionally absent | Validate privately with authorized dependencies and pseudonymized data. | Open | Record Unity import/build and analysis results without publishing private inputs. |
| PUB-011 | Critical | Unity project settings | A non-default-looking PS4 passcode field and content identifier were present in the historical working copy. | Redacted field-level review found one unchanged value in the old `main` history from March 2024 through its final checkout. | Clear the identifier, reset the public-tree passcode, keep old repositories private, and confirm whether external rotation is required. | Closed | Public tree is sanitized; on 2026-07-21 the owner confirmed that the historical PS4 code requires no further action. |
| PUB-012 | High | Unity log writer | Serialized study fields could enter a filename and logs were written inside `Assets`. | `LogWriter.cs` path construction | Require a `P###` participant ID, validate filename tokens, and write beneath `Application.persistentDataPath`. | Closed | Source review confirms allowlisted tokens and no write to `Application.dataPath`. |
| PUB-013 | High | Analysis images | Three retained map images needed an explicit publication-rights decision. | `analysis/CHI24/Assets/city.png`, `game.png`, and `maze.png` | Confirm rights before including the images in the MIT-licensed artifact. | Closed | On 2026-07-21 the owner confirmed all three images are cleared for public inclusion. |

## Required actions before changing visibility

1. Create the public repository from this clean root history only. Do not merge
   either historical source repository.
2. Review all GitHub-hosted security, release, workflow, wiki, Pages, package,
   issue, and pull-request surfaces.
3. Run a private Unity import/build with properly licensed dependencies and a
   private analysis smoke test with approved pseudonymized data.
4. Scan the final commit and a fresh clone again before enabling public access.

## Local verification record

Verification performed on 2026-07-21:

- The publication inventory found no sensitive-path candidates, nested
  repositories, submodules, LFS paths, tags, remotes, or existing history in
  the new root repository.
- Structured credential patterns and private-key markers returned no matches.
  A complementary high-entropy scan produced only two manually reviewed Unity
  input-binding names; no non-default passcode remains.
- Known participant aliases, masked names, absolute user paths, notebook
  outputs, and notebook execution counts returned zero matches.
- Both Python report scripts passed `compileall`. All 14 notebooks passed
  `nbformat` validation, and their ordinary Python cells passed syntax parsing.
- The analysis requirements cover every detected third-party Python import.
- Unity's manifest and lockfile are valid JSON; all 47 direct manifest packages
  have lock entries.
- The retained Unity tree has no missing or orphaned `.meta` file. All four
  build-settings scene entries exist and match their committed GUIDs.
- Markdown has no broken local links and the reviewed text sources have no
  trailing whitespace.
- A Git-ignore-based fresh export contained 272 files (about 53 MB) and repeated
  the Python, notebook, JSON, secret-pattern, and Unity metadata checks.

Not verified locally: Unity import/build (the required editor and licensed
assets are absent), end-to-end numerical analysis (private data and a pinned
environment are absent), remote hosting surfaces, and post-push fresh clone.
