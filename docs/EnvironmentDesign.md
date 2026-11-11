# Environment Design Specification

## 1. Visual Style & Atmosphere (SilentExit Reference Guide)
* **Backrooms Regions (B1–B4, Final F1):**
  * Monotonous, sickly yellow-tinted wallpaper (`M_Backrooms_Wall`), moist stained carpeting (`M_Backrooms_Carpet_Stained`), and drop-acoustic ceilings.
  * Lighting: Low-frequency warm/buzzing fluorescent fixtures (`PF_B_PROP_01_FluorescentFixture`), cast harsh shadows and patchy gloom.
  * Atmosphere: Mild volumetric fog/haze, subtle vignette, liminal isolation.
* **Poolrooms Regions (P1–P9):**
  * Clinical, pristine square white ceramic tiles (`MAT_Ceramic_25cm_PristineWhite`), columns (`PF_P_MOD_36_Column_Round`), arched passageways (`MOD_08_Archway`), circular flooded tunnels (`MOD_51`).
  * Water features: Shallow flooded pathways in P4 & P6 with turquoise refraction, gentle surface caustics, and water splash cues.
  * Lighting: Diffuse, clinical white/cool bounce lighting, ceiling recessed lighting.
  * Visual lore match: Clean surreal architecture contrasting with the looming dread of the blind Golem ("It can't see you. It hears the water.").

## 2. Spatial Mapping & Zone Layout (Map Reference)

| Zone | Rooms | Theme / Assets | Key Features & Landmarks |
| :--- | :--- | :--- | :--- |
| **Zone 1: Backrooms Entrance** | B1 (Start), B2 | Yellow Backrooms Kit | Player start in B1, modular partition maze in B2, Side Pillars |
| **Zone 2: Backrooms Upper** | B3 (CP1), B4 | Yellow Backrooms Kit | Checkpoint 1 (B3), transitional doorway connecting to P1 |
| **Zone 3: Poolrooms Upper** | P1, P2 (CP2), P3 (CP3) | White Tile Poolrooms Kit | P1 transition room, CP2 in P2, CP3 in P3, recessed alcoves |
| **Zone 4: Flooded Core** | P4, P5, P6 | Flooded Poolrooms Kit | Submerged pool in P6, flooded corridor in P4, central hub P5 |
| **Zone 5: Lower Poolrooms** | P7, P8, P9 | White Tile Poolrooms Kit | Narrow passages, decorative pillars, drainage grates |
| **Zone 6: Exit Chamber** | F1 (Exit) | Dark Industrial Backrooms | Final transition gate from P4, dark floor texture, illuminated Red EXIT sign |

## 3. Performance & Low-Spec PC Optimization
* **Prefabs:** Use `Assets/Project/Prefabs/Environment/Generated` modular units snapped to 4m grid.
* **Static Batching:** Mark non-moving architecture as `Static` for batching and occlusion culling.
* **Lighting Strategy:** Baked / Mixed lighting for low-end GPU overhead; minimal unshadowed real-time fill lights.
* **Colliders:** Simplified box colliders on wall modules, avoiding heavy mesh colliders.
