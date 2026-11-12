# Environment Design Specification

## 1. Visual Style & Atmosphere (SilentExit Reference Guide)
* **Backrooms Regions (B1–B4):**
  * Monotonous, sickly yellow wallpaper (`Wall_Painted_Mat`), damp stained carpeting (`Floor_Carpet_Mat`), acoustic drop-ceiling tiles (`Ceiling_Office_Mat`), structural baseboards/pillars (`Wall_trim_mat`).
  * All Backrooms materials upgraded and mapped to URP Lit shaders with appropriate roughness and normal maps.
  * Internal partitions and pillars reproduce the original map's labyrinthine structure.
* **Poolrooms Regions (P1–P9):**
  * Pristine 25 cm white ceramic tiles (`MAT_Ceramic_25cm_PristineWhite`), round columns (`MOD_36_Column_Round`), arched passageways (`MOD_08_Archway`), and clean modular geometry snapped on a 4m grid.
  * Flooded areas (P6 submerged pool basin and P4 flooded corridor) feature deep pool floor geometry with URP transparent water surface planes (`WTR_01_Surface_4m`).
* **Final Exit Chamber (F1):**
  * Faithfully implements the dark poolroom concept using Poolrooms architecture with dark atmospheric treatment (`M_Pool_Dark`), situated east of P4.
  * Equipped with an illuminated red EXIT sign (`M_ExitSign`) casting an ominous red glow.

## 2. Spatial Mapping & Zone Layout (Constructed)

| Zone | Rooms | Modular Bounds (4m Grid) | Features & Connections |
| :--- | :--- | :--- | :--- |
| **Zone 1: Backrooms Entrance** | B1 (Start), B2 | X: [-16, -8], Z: [0, 24] | Starting spawn at B1, complex partitions in B2, doorway to B3 |
| **Zone 2: Backrooms Upper** | B3 (CP1), B4 | X: [-16, -4], Z: [24, 32] | CP1 location in B3, doorway to B4, transitional portal to P1 |
| **Zone 3: Poolrooms Upper** | P1, P2 (CP2), P3 (CP3) | X: [0, 24], Z: [24, 32] | P1 transition room, CP2 in P2, CP3 in P3, arched portals |
| **Zone 4: Flooded Core** | P4, P5, P6 | X: [0, 24], Z: [12, 24] | P6 submerged basin (water), P5 central hub, P4 flooded corridor |
| **Zone 5: Lower Poolrooms** | P7, P8, P9 | X: [0, 24], Z: [0, 8] | Lower corridors, columns, interconnected archways |
| **Zone 6: Exit Chamber** | F1 (Exit) | X: [24, 32], Z: [12, 20] | Dark poolroom, red illuminated EXIT sign, doorway to P4 |

## 3. Collision, Batching & Low-Spec Performance
* **Colliders:** Mesh colliders attached to all solid floor, wall, column, and ceiling modules; water surfaces left non-solid/trigger-ready.
* **Batching:** Complete `Environment` hierarchy flagged `Static` for static batching and occlusion culling.
* **Zero Missing Shaders:** Built-in standard shaders upgraded to URP Lit.
