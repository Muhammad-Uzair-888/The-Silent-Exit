# Asset Inventory

## Environment & Art Assets

| Category | Source Path | Available Assets / Elements | Compatibility / Status |
| :--- | :--- | :--- | :--- |
| **Backrooms Pack 1** | `Assets/Project/Art/Backrooms/BackroomsLikeAsset` | Walls, Doorways, Holes, Ceiling, Concrete, Carpet, Office Tiles, Debris, Props | Standard shader used; needs URP material verification/remapping |
| **Backrooms Pack 2** | `Assets/Project/Art/Backrooms/LoafbrrAssets/BackroomsLikeAssetRe` | High-fidelity modular tiles, FBX meshes, textures, wall & pillar variations | Pre-configured URP materials |
| **Poolrooms Kit** | `Assets/Project/Art/Poolrooms/FBX` | 53 architectural modules (walls, archways, stairs, columns, portals, circular flooded portal MOD_51, water surface) | Clean modular geometry (4m grid/height), clean white tile textures |
| **Poolrooms Props** | `Assets/Project/Art/Poolrooms/FBX` | Pool ladder (PRP_01), drains, grates, vents, railings, wall & ceiling light fixtures, tiled bench | Ready for prop placement |
| **Generated Prefabs**| `Assets/Project/Prefabs/Environment/Generated` | Prefabs for both Backrooms and Poolrooms modules (`PF_B_*`, `PF_P_*`) | Pre-assembled prefabs with colliders |
| **Pre-built Assemblies**| `Assets/Things` | `Map.prefab`, `PillarsBackroom.prefab`, `PillarsPoolrooms.prefab` | Existing blockouts/templates |

## Character & Monster Assets

| Asset | Source Path | Components / Animations | Status |
| :--- | :--- | :--- | :--- |
| **Player Controller** | `Assets/Project/ThirdParty/ModularFirstPersonController` | `FirstPersonController.cs`, `FirstPersonController.prefab`, variant in `Assets/Things` | Legacy input; must preserve intact |
| **Golem Monster Model** | `Assets/Project/Art/Monster/SiuniaevCharacters/02_Golem` | Skinned mesh FBX, textured model, prefab `GolemPrefab.prefab` | Standard shader material (`Golem.mat`) needs URP upgrade |
| **Monster Animations** | Embedded in `Golem.FBX` | `idle`, `idle_action`, `walk`, `rage`, `hit`, `hit2`, `damage`, `die`, `jump`, `fly`, `land`, `sleep_*` | 13 embedded animation clips verified |

## Shaders, Rendering & Audio

| Asset | Path | Details |
| :--- | :--- | :--- |
| **URP Render Pipeline** | `Assets/Project/Settings` | `URP-Balanced`, `URP-HighFidelity`, `URP-Performant` & Renderers configured |
| **Post-Processing** | `Assets/Project/Settings/SilentExit_VolumeProfile.asset` | Volume profile ready for fog, color grading, vignette, bloom |
| **Water Material** | `Assets/Project/Materials/Poolrooms/MAT_Water_AssignEngineShader.mat` | Needs URP-compatible water shader graph or transparent ripple shader |
| **Audio Slots** | `Assets/Project/Audio/` (Environment, Monster, Player) | Folders organized; ready for footstep/splashing and atmospheric audio |
