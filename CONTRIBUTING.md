# Contributing

This repository is an archived research artifact. Small fixes that improve
documentation, safety, or preservation are welcome; major feature development
is outside its current scope.

Before submitting a change:

1. Do not include participant data, recruitment records, re-identification
   keys, notebook outputs, credentials, local paths, or private URLs.
2. Do not add Unity Asset Store packages or other third-party source unless the
   repository has a documented redistribution grant for those exact files.
3. Keep every Unity asset paired with its `.meta` file and preserve GUIDs.
4. Clear notebook outputs and execution counts.
5. Run `python3 scripts/check_public_tree.py`.
6. Run `python -m compileall analysis/CHI24` and validate notebook JSON with
   `nbformat` when the dependency is available.
7. Run `git diff --check` and review the complete diff before committing.

Numerical result changes require a separately reviewed, privacy-safe test
fixture; do not use a real participant record as a regression test.
