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

## Pattern 4: University Curriculum Alignment and Legacy Input
* **Symptom:** Rule conflict between AGENTS.md mandating New Input System while university curriculum and imported controller use Legacy Input Manager.
* **Root Cause:** Incomplete alignment between general project guidelines and course curriculum requirements.
* **Prevention Rule:** For university academic projects, adhere strictly to the instructor-approved input system (Legacy Input Manager) and ensure all scripts use consistent input methods without unsupported packages.

## Pattern 5: Unity Background Compilation and Play Mode Frame Ticking
* **Symptom:** Unity Editor remains stuck with `EditorApplication.isCompiling == true`, or game loop frame count fails to advance during headless Play Mode testing.
* **Root Cause:** Orphaned IL post-processor runner processes (`Unity.ILPP.Runner`) can stall compilation named pipes, and unfocused Unity Editors throttle frame execution when `Application.runInBackground` is false.
* **Prevention Rule:** Clear stalled compiler runner processes before domain reload and enforce `Application.runInBackground = true` during headless automated Play Mode test runs.
