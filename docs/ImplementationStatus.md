# Implementation Status

## Current Milestone: Final Gameplay Systems & Verification Completed

* **Active Scene:** `Assets/Project/Scenes/GamePlay.unity` (saved & verified operational in Play Mode)
* **Code Architecture:** 100% compliant with university instructor curriculum. Written in clean, readable beginner student C# style without advanced professional patterns (no singletons, no coroutines, no lambdas/System.Action, no custom HLSL shaders).
* **Audio Implementation:** 15 royalty-free 16-bit 44.1kHz WAV clips generated via `AudioSynthesizer.cs` and actively assigned across Player, Golem, Distraction, and Environment.
* **Console Status:** 0 compile errors, 0 runtime errors.

---

## 1. Finalized Gameplay Implementations

### A. Title Screen & Game Startup
- Created `Panel_TitleScreen` under `CrosshairAndStamina` Canvas with:
  - 9-sliced dark background (`Background.psd`, `Image.Type.Sliced`).
  - Title: **THE SILENT EXIT** (44pt bold).
  - Subtitle and controls legend:
    - `WASD`: Walk (Silent)
    - `SHIFT`: Sprint (Loud - Attracts Golem)
    - `F`: Throw Distraction Item (One-Shot)
    - `SPACE`: Jump | `CTRL`: Crouch | `L-CLICK`: Interact
  - Interactive "ENTER COMPLEX" Play button (`Button_Play`).
- Inspector `OnClick` event persistently wired to `GameManager.StartGame()`.
- Title Screen startup locks player movement and camera rotation (`playerCanMove = false`, `cameraCanMove = false`, `lockCursor = false`) and unlocks mouse cursor (`CursorLockMode.None`, `visible = true`).
- Pressing "ENTER COMPLEX" hides title screen, reveals HUD, locks cursor (`CursorLockMode.Locked`, `visible = false`), enables player movement and camera, and starts the 60-second level countdown timer.

### B. Global 60-Second Level Countdown Timer
- Completely decoupled `SprintBar` from player stamina.
- Converted `CrosshairAndStamina/SprintBar` into a global level countdown slider anchored to the top-center of the screen.
- Added child `Text_LevelTimer` displaying remaining seconds (`TIME: 60s` down to `TIME: 0s`).
- Single level-wide timer starts from 60 seconds when "ENTER COMPLEX" is pressed.
- Does NOT reset, pause, or extend when the player activates Checkpoints (CP1, CP2, CP3).
- Continues ticking uninterrupted during death and respawn.
- When timer reaches 0, triggers terminal fail state:
  - Activates `Panel_TimeExceeded` with 9-sliced dark red background, "TIME EXCEEDED" title, and horror ambiance.
  - Disables player movement and camera.
  - Unlocks mouse cursor with a functional "TRY AGAIN" button calling `GameManager.RestartGame()`.

### C. Death and Respawn Visual Fade
- Implemented a 1.5-second visual fade sequence in `GameManager.cs` driven entirely by `Time.deltaTime` in `Update()` (0 coroutines, 0 CanvasGroups):
  - **0.0s to 0.75s:** Screen fades from transparent to solid black (`Image_FadeOverlay.color.a` interpolates $0 \to 1$).
  - **At exactly 0.75s:** Player teleports to initial spawn position (`-12.0, 0.5, 2.0`), checkpoint progress resets to 0 (per updated game mechanics), and checkpoint triggers re-arm. The level countdown timer continues running without pause.
  - **0.75s to 1.5s:** Screen fades from solid black to transparent (`Image_FadeOverlay.color.a` interpolates $1 \to 0$).
  - **At 1.5s:** Respawn concludes, overlay disables, and player movement is re-enabled.

### D. One-Shot Throwable Distraction Item
- Created `DistractionItem.prefab` in `Assets/Project/Prefabs/`:
  - Visual: Sphere mesh with dark matte exit-sign material (`M_ExitSign.mat`).
  - Physics: `SphereCollider` (radius $0.15\text{m}$) and `Rigidbody` (mass $0.5\text{kg}$, continuous collision detection).
  - Audio: 3D `AudioSource` with synthesized impact sound `sfx_distraction_impact.wav`.
  - Component: `ThrowableItem.cs` emitting `SoundSystem.EmitSound(transform.position, 24.0f, SoundType.Distraction)` on `OnCollisionEnter`.
- Created `ThrowableDistraction.cs` on `FirstPersonController`:
  - Triggers throw via key `F`.
  - Throws forward with an upward arc using `ForceMode.Impulse` ($14.0\text{ N}\cdot\text{s}$).
  - Strictly one-shot per run (`hasItem` flag prevents duplicate throws).
- Updated `SoundType` enum with `Distraction` and extended `GolemAI.cs`:
  - When Golem hears `SoundType.Distraction` within $24\text{m}$, it enters `Investigate` state and sets its NavMesh destination directly to the item's landing coordinate.
  - Does NOT track player position, maintaining complete blindness.

### E. Level Traversal Balance
- Player walk speed set to $2.4\text{ m/s}$.
- Start-to-Exit total path distance: $\approx 53.2\text{m}$ to $72.5\text{m}$ depending on room routing.
- Pure walking traversal time: $\approx 22.2\text{s}$ to $30.2\text{s}$, leaving $\approx 30\text{s}$ to $38\text{s}$ of buffer within the 60-second limit.
- Walking alone easily completes the level without sprinting or altering Golem hearing thresholds.

---

## 2. Component & Scene Hierarchy Matrix

| Component / System | Hierarchy Path / Asset | Status | Verification Details |
| :--- | :--- | :--- | :--- |
| **Title Screen Panel** | `CrosshairAndStamina/Panel_TitleScreen` | Operational | 9-sliced panel, 44pt title, controls legend, "ENTER COMPLEX" button wired to `GameManager.StartGame()` |
| **Global Timer Slider** | `CrosshairAndStamina/SprintBar` | Operational | Top-center level countdown slider with `Text_LevelTimer` |
| **Time Exceeded Panel** | `CrosshairAndStamina/Panel_TimeExceeded` | Operational | 9-sliced red panel, "TIME EXCEEDED", "TRY AGAIN" button wired to `GameManager.RestartGame()` |
| **Death Fade Overlay** | `CrosshairAndStamina/Image_FadeOverlay` | Operational | Fullscreen black image driven by `Time.deltaTime` ($0 \to 1 \to 0$ over 1.5s) |
| **Throwable Distraction** | `FirstPersonController` | Operational | `ThrowableDistraction.cs` throwing `DistractionItem.prefab` on `F` with `ForceMode.Impulse` |
| **Distraction Prefab** | `Assets/Project/Prefabs/DistractionItem.prefab` | Operational | Rigidbody, SphereCollider, 3D AudioSource, `ThrowableItem.cs` |
| **Player Controller** | `FirstPersonController` | Operational | `walkSpeed = 2.4 m/s`, `PM_PlayerFooting` collider material, `ForceMode.Acceleration` & `Impulse` |
| **Player Sound Emitter** | `FirstPersonController` | Operational | Carpet steps ($X < 0$), tile steps ($X \ge 0$), water splashes |
| **Monster Golem** | `Environment/Monster_Golem` | Operational | Completely blind; investigates distraction landing coordinate; searches 4s; chases water splashes |
| **Checkpoints (1..3)** | `Environment/*/*/Trigger_CP1..3` | Operational | Sequential activation; does NOT reset or extend level timer |
| **Exit Trigger (F1)** | `Environment/Poolrooms/F1_Exit/Trigger_Exit` | Operational | Unlocks at CP3; triggers 9-sliced Victory panel |
| **Mute Toggle** | `CrosshairAndStamina/Toggle_AudioMute` | Operational | UI Toggle controlling `AudioListener.pause` |

---

## 3. Play Mode Empirical Verification Results

All tests executed and verified in Unity Play Mode on `Assets/Project/Scenes/GamePlay.unity`:

1. **Title Screen & Startup:**
   - On scene load: `isGameStarted = false`, `isTimerRunning = false`, `Panel_TitleScreen` active, HUD inactive, player movement and camera locked, cursor unlocked (`CursorLockMode.None`, `visible = true`).
   - Clicking "ENTER COMPLEX": `isGameStarted = true`, `isTimerRunning = true`, Title Screen hides, HUD shows, timer starts at 60s, player moves, camera rotates, cursor locks (`CursorLockMode.Locked`, `visible = false`).
2. **Global 60s Level Timer:**
   - Ticks continuously in real-time ($60\text{s} \to 51\text{s} \to 33\text{s} \to \dots$).
   - Slider value matches percentage ($1.0 \to 0.0$).
   - Activating Checkpoint 1 and Checkpoint 2 preserves remaining countdown time without reset or extension.
3. **Death & Respawn Visual Fade:**
   - On caught: lives decrement from 3 to 2, 3rd heart turns gray, player movement locks.
   - Screen fades $0 \to 1$ black over 0.75s.
   - At 0.75s: player teleports to start spawn `(-12.0, 0.5, 2.0)`, checkpoint index resets to 0, and level countdown timer continues running uninterrupted.
   - Screen fades $1 \to 0$ over 0.75s.
   - At 1.5s: respawn ends, movement and camera restore, full gameplay resumes.
4. **Time Exceeded Fail State:**
   - When countdown reaches 0: `isTimeExceeded = true`, `isTimerRunning = false`, movement and camera lock, `Panel_TimeExceeded` appears, cursor unlocks, terminal fail state triggers without checkpoint respawn.
5. **Throwable Distraction:**
   - Key `F` throws `DistractionItem` forward in a parabolic arc via `ForceMode.Impulse`.
   - `hasItem` set to `false`; subsequent presses do nothing (one-shot per run).
   - On collision, plays `sfx_distraction_impact.wav` and emits `SoundType.Distraction`.
   - Golem enters `Investigate` and moves to the exact landing coordinates ($16.83, 0.05, 31.17$); does not track player coordinates.
6. **Traversal Feasibility:**
   - Walking speed $2.4\text{ m/s}$ covers the entire level in $\approx 22.2\text{s}$ ($\approx 30.2\text{s}$ with turns), comfortably beating the 60s limit with over $30\text{s}$ of buffer.
7. **Game Restart:**
   - Clicking "TRY AGAIN" on Time Exceeded or Game Over cleanly reloads the scene: resets lives to 3, timer to 60s, restores distraction item, and returns to Title Screen.
8. **Unity Console:**
   - 0 compile errors, 0 runtime errors across all verification runs.
