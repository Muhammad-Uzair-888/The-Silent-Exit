# Lessons Learned

## Pattern 1: FBX Import Rotation and Modular Kit Co-Location
* **Symptom:** Modular walls and archways appeared flat on the floor (`size.y = 0.20m`), resulting in visible gaps between rooms and missing collisions.
* **Root Cause:** Imported FBX models required `X = 270.02°` rotation (Z-up / Y-up axis conversion). Setting Euler angles to `(0, y, 0)` caused pieces to lie flat.
* **Prevention Rule:** When instantiating modular kit prefabs, always preserve their prefab rotation conventions (or multiply yaw around world vertical axis: `Quaternion.Euler(0, yaw, 0) * prefabRotation`) and verify 3D bounding extents (`Renderer.bounds.size.y >= 3.0m`).

## Pattern 2: Co-Located Solid Boundary Walls at Doorway Transitions
* **Symptom:** Designated doors and archways were completely blocked to player traversal.
* **Root Cause:** Separate rooms each placed closed boundary walls at shared room edges without removing solid wall tiles where doors and archways were installed.
* **Prevention Rule:** At every designated transition opening or doorway, ensure any overlapping solid wall pieces from both connecting rooms are excised so that the passage is unobstructed.

## Pattern 3: Water Shaders in URP Projects
* **Symptom:** Water rendered opaque or unshaded in URP.
* **Root Cause:** Built-in shaders without `UniversalForward` passes are skipped or error-rendered by URP. In addition, URP pipeline assets default to `supportsCameraDepthTexture = false` and `supportsCameraOpaqueTexture = false`.
* **Prevention Rule:** In URP projects, always verify pipeline asset depth and opaque texture toggles and ensure water shaders use UniversalForward passes with `MixFog` support.
