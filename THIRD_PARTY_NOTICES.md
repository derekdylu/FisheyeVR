# Third-party dependencies and excluded assets

The original local research prototype used third-party Unity content. This
public snapshot does **not** include the source files from the following local
asset roots:

- `AK Studio Art/Business Office`
- `BNG Framework` / VR Interaction Framework by Bearded Ninja Games
- `Cinema Suite`
- `Plugins/RootMotion`
- `SimplePoly City - Low Poly Assets`
- the copied `TextMesh Pro` asset directory

The BNG Framework copy explicitly stated that it was governed by the Unity Asset
Store terms. No repository-level redistribution grant was found for the other
excluded asset roots during the local audit. Their removal is conservative and
does not determine the legal status of any independently acquired copy.

The Unity scenes and project scripts may still refer to these dependencies by
GUID, namespace, or serialized component. Anyone attempting to restore the
prototype must acquire their own valid, compatible copies and comply with the
applicable license terms. Do not copy a locally installed package back into this
repository.

Unity Package Manager dependencies remain declared in
[`unity/Packages/manifest.json`](unity/Packages/manifest.json) and are subject to
their respective upstream terms.
