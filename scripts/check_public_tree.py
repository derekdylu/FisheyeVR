#!/usr/bin/env python3
"""Run the repository's local, non-networked public-release checks."""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]

REQUIRED_PATHS = (
    "LICENSE",
    "README.md",
    "SECURITY.md",
    "THIRD_PARTY_NOTICES.md",
)

BLOCKED_PATHS = (
    "FisheyeVR-Unity",
    "FisheyeVR-Analysis",
    "analysis/CHI24/ExpResults",
    "analysis/UIST24/ExpResults",
    "unity/Assets/ExperimentLogs",
    "unity/Assets/AK Studio Art",
    "unity/Assets/BNG Framework",
    "unity/Assets/Cinema Suite",
    "unity/Assets/Plugins/RootMotion",
    "unity/Assets/SimplePoly City - Low Poly Assets",
)

TEXT_SUFFIXES = {
    ".asset",
    ".cs",
    ".gitignore",
    ".ipynb",
    ".json",
    ".mat",
    ".md",
    ".meta",
    ".prefab",
    ".py",
    ".shader",
    ".txt",
    ".unity",
    ".yaml",
    ".yml",
}

SECRET_PATTERNS = {
    "private-key block": re.compile(r"-----BEGIN (?:[A-Z ]+)?PRIVATE KEY-----"),
    "AWS access key": re.compile(r"\b(?:AKIA|ASIA)[0-9A-Z]{16}\b"),
    "GitHub token": re.compile(
        r"\b(?:github_pat_[A-Za-z0-9_]{20,}|gh[pousr]_[A-Za-z0-9]{20,})\b"
    ),
    "Slack token/webhook": re.compile(
        r"\bxox[baprs]-[A-Za-z0-9-]{10,}"
        r"|https://hooks\.slack\.com/services/[A-Za-z0-9/_-]+"
    ),
    "Google API key": re.compile(r"\bAIza[0-9A-Za-z_-]{30,}\b"),
    "OpenAI-style key": re.compile(r"\bsk-(?:proj-)?[A-Za-z0-9_-]{20,}\b"),
    "absolute home path": re.compile(
        "/" + "Users/" + "|" + r"[A-Za-z]:\\" + "Users" + r"\\"
    ),
}


def public_files() -> list[Path]:
    return sorted(
        path
        for path in ROOT.rglob("*")
        if path.is_file() and ".git" not in path.relative_to(ROOT).parts
    )


def text_files(files: list[Path]) -> list[Path]:
    return [
        path
        for path in files
        if path.suffix.lower() in TEXT_SUFFIXES
        or path.name in {".gitignore", "LICENSE"}
    ]


def check_blocked_paths(errors: list[str]) -> None:
    for relative in REQUIRED_PATHS:
        if not (ROOT / relative).is_file():
            errors.append(f"required public file is missing: {relative}")

    for relative in BLOCKED_PATHS:
        if (ROOT / relative).exists():
            errors.append(f"blocked path exists: {relative}")

    nested_git = [path for path in ROOT.rglob(".git") if path != ROOT / ".git"]
    for path in nested_git:
        errors.append(f"nested Git repository exists: {path.relative_to(ROOT)}")


def check_text(files: list[Path], errors: list[str]) -> None:
    for path in text_files(files):
        text = path.read_text(errors="ignore")
        relative = path.relative_to(ROOT)
        for label, pattern in SECRET_PATTERNS.items():
            if pattern.search(text):
                errors.append(f"{label} candidate: {relative}")

        if path.suffix == ".py" and re.search(r"shell\s*=\s*True", text):
            errors.append(f"unsafe shell execution: {relative}")


def check_notebooks(errors: list[str]) -> int:
    notebooks = sorted((ROOT / "analysis").rglob("*.ipynb"))
    for path in notebooks:
        try:
            notebook = json.loads(path.read_text())
        except (OSError, json.JSONDecodeError) as exc:
            errors.append(f"invalid notebook JSON: {path.relative_to(ROOT)} ({exc})")
            continue

        for index, cell in enumerate(notebook.get("cells", [])):
            if cell.get("cell_type") != "code":
                continue
            if cell.get("outputs"):
                errors.append(
                    f"notebook output present: {path.relative_to(ROOT)} cell {index}"
                )
            if cell.get("execution_count") is not None:
                errors.append(
                    f"notebook execution count present: {path.relative_to(ROOT)} cell {index}"
                )
    return len(notebooks)


def check_unity_metadata(errors: list[str]) -> None:
    assets = ROOT / "unity" / "Assets"
    for path in assets.rglob("*"):
        if path.name.endswith(".meta"):
            asset = Path(str(path)[:-5])
            if not asset.exists():
                errors.append(f"orphaned Unity metadata: {path.relative_to(ROOT)}")
        elif path != assets and not Path(str(path) + ".meta").exists():
            errors.append(f"missing Unity metadata: {path.relative_to(ROOT)}")

    settings_path = ROOT / "unity" / "ProjectSettings" / "EditorBuildSettings.asset"
    settings = settings_path.read_text()
    scene_entries = re.findall(
        r"path: (Assets/Scenes/[^\n]+)\n\s+guid: ([0-9a-f]{32})",
        settings,
    )
    for relative, guid in scene_entries:
        scene = ROOT / "unity" / relative
        meta = Path(str(scene) + ".meta")
        if not scene.exists() or not meta.exists():
            errors.append(f"build scene or metadata missing: {relative}")
        elif f"guid: {guid}" not in meta.read_text():
            errors.append(f"build scene GUID mismatch: {relative}")

    player_settings = (
        ROOT / "unity" / "ProjectSettings" / "ProjectSettings.asset"
    ).read_text()
    match = re.search(r"^  ps4Passcode:\s*(\S*)", player_settings, re.MULTILINE)
    if match and match.group(1) not in {"", "0" * 32}:
        errors.append("non-default PS4 passcode remains in ProjectSettings.asset")


def main() -> int:
    errors: list[str] = []
    files = public_files()

    check_blocked_paths(errors)
    check_text(files, errors)
    notebook_count = check_notebooks(errors)
    check_unity_metadata(errors)

    license_text = (ROOT / "LICENSE").read_text(errors="ignore")
    if not license_text.startswith("MIT License\n"):
        errors.append("LICENSE is not identified as the MIT License")
    if "[MIT License](LICENSE)" not in (ROOT / "README.md").read_text():
        errors.append("README does not link to the MIT License")

    oversized = [path for path in files if path.stat().st_size > 50 * 1024 * 1024]
    for path in oversized:
        errors.append(f"file exceeds 50 MiB: {path.relative_to(ROOT)}")

    if errors:
        print("Public-tree checks failed:", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 1

    print(f"Public-tree checks passed ({len(files)} files, {notebook_count} notebooks).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
