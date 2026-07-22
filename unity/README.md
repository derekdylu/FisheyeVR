# Unity prototype

This is a partial Unity `2021.3.16f1` project containing FisheyeVR's
project-owned scripts, shader, settings, and selected research scenes.

## Important limitation

The original working copy included licensed Unity Asset Store packages and
other third-party content. Their source files were removed from this public
snapshot. The selected scenes retain their original `.meta` GUIDs but include
references to missing assets and scripts, including the BNG/VR interaction
framework. The project is therefore useful for source inspection, but it is not
a clean, reproducible build until compatible licensed dependencies are restored.

Do not regenerate or discard the committed `.meta` files. When moving an asset
outside the Unity Editor, move its `.meta` file with it.

## Retained structure

- `Assets/Scripts/`: project interaction and experiment code.
- `Assets/Shaders/`: the FisheyeVR image-effect shader.
- `Assets/Scenes/`: selected project scenes moved with their original metadata.
- `Packages/`: Unity Package Manager manifest and lockfile.
- `ProjectSettings/`: project configuration with analytics disabled.

The logger accepts only pseudonymous IDs in the form `P001` and writes to the
platform-specific `Application.persistentDataPath/ResearchLogs` directory. It
does not write into `Assets`. The legacy `Assets/ExperimentLogs` path remains
ignored as a safeguard. Unity-generated `Library`, `Temp`, `Obj`, `Logs`,
`UserSettings`, and build directories are also ignored.

Review [`../THIRD_PARTY_NOTICES.md`](../THIRD_PARTY_NOTICES.md) before restoring
dependencies or distributing a build.
