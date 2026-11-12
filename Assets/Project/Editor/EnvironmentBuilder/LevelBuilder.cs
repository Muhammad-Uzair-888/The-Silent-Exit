using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace SilentExit.Editor
{
    public static class LevelBuilder
    {
        private const float GRID = 4.0f;

        // Prefab paths
        private const string B_TILE = "Assets/Project/Art/Backrooms/BackroomsLikeAsset/prefab/Tiles/Tiles_01_Fill.prefab";
        private const string B_WALL = "Assets/Project/Art/Backrooms/BackroomsLikeAsset/prefab/Walls/Wall.prefab";
        private const string B_WALL_DOOR = "Assets/Project/Art/Backrooms/BackroomsLikeAsset/prefab/Walls/WallDoor_A.prefab";
        private const string B_PILLAR = "Assets/Project/Art/Backrooms/BackroomsLikeAsset/prefab/Walls/Wall_Pillar_A.prefab";

        private const string P_WALL = "Assets/Project/Art/Poolrooms/FBX/MOD_01_Wall_Standard.fbx";
        private const string P_DOOR = "Assets/Project/Art/Poolrooms/FBX/MOD_07_Wall_NarrowOpening.fbx";
        private const string P_ARCH = "Assets/Project/Art/Poolrooms/FBX/MOD_08_Archway.fbx";
        private const string P_FLOOR_DRY = "Assets/Project/Art/Poolrooms/FBX/MOD_15_Floor_Dry.fbx";
        private const string P_FLOOR_SHALLOW = "Assets/Project/Art/Poolrooms/FBX/MOD_17_Floor_Shallow.fbx";
        private const string P_FLOOR_DEEP = "Assets/Project/Art/Poolrooms/FBX/MOD_19_Floor_Deep.fbx";
        private const string P_CEILING = "Assets/Project/Art/Poolrooms/FBX/MOD_44_Ceiling_Standard.fbx";
        private const string P_COLUMN_ROUND = "Assets/Project/Art/Poolrooms/FBX/MOD_36_Column_Round.fbx";
        private const string P_WATER = "Assets/Project/Art/Poolrooms/FBX/WTR_01_Surface_4m.fbx";

        private const string MAT_DARK_POOL = "Assets/Project/Materials/Poolrooms/M_Pool_Dark.mat";
        private const string MAT_EXIT_SIGN = "Assets/Project/Materials/Backrooms/M_ExitSign.mat";

        public static string BuildFullEnvironment()
        {
            var oldEnv = GameObject.Find("Environment");
            if (oldEnv != null)
            {
                Object.DestroyImmediate(oldEnv);
            }

            GameObject rootEnv = new GameObject("Environment");
            GameObject bRoot = new GameObject("Backrooms");
            bRoot.transform.SetParent(rootEnv.transform);
            GameObject pRoot = new GameObject("Poolrooms");
            pRoot.transform.SetParent(rootEnv.transform);
            GameObject propRoot = new GameObject("Environment Props");
            propRoot.transform.SetParent(rootEnv.transform);
            GameObject waterRoot = new GameObject("Water");
            waterRoot.transform.SetParent(rootEnv.transform);

            // Backrooms Room Parents
            GameObject b1 = new GameObject("B1_Start"); b1.transform.SetParent(bRoot.transform);
            GameObject b2 = new GameObject("B2_Maze"); b2.transform.SetParent(bRoot.transform);
            GameObject b3 = new GameObject("B3_CP1"); b3.transform.SetParent(bRoot.transform);
            GameObject b4 = new GameObject("B4_Transition"); b4.transform.SetParent(bRoot.transform);

            // Poolrooms Room Parents
            GameObject p1 = new GameObject("P1_Transition"); p1.transform.SetParent(pRoot.transform);
            GameObject p2 = new GameObject("P2_CP2"); p2.transform.SetParent(pRoot.transform);
            GameObject p3 = new GameObject("P3_CP3"); p3.transform.SetParent(pRoot.transform);
            GameObject p4 = new GameObject("P4_FloodedCorridor"); p4.transform.SetParent(pRoot.transform);
            GameObject p5 = new GameObject("P5_CentralHub"); p5.transform.SetParent(pRoot.transform);
            GameObject p6 = new GameObject("P6_SubmergedPool"); p6.transform.SetParent(pRoot.transform);
            GameObject p7 = new GameObject("P7_LowerLeft"); p7.transform.SetParent(pRoot.transform);
            GameObject p8 = new GameObject("P8_LowerCenter"); p8.transform.SetParent(pRoot.transform);
            GameObject p9 = new GameObject("P9_LowerRight"); p9.transform.SetParent(pRoot.transform);
            GameObject f1 = new GameObject("F1_Exit"); f1.transform.SetParent(pRoot.transform);

            // Load assets
            GameObject bTileGo = AssetDatabase.LoadAssetAtPath<GameObject>(B_TILE);
            GameObject bWallGo = AssetDatabase.LoadAssetAtPath<GameObject>(B_WALL);
            GameObject bWallDoorGo = AssetDatabase.LoadAssetAtPath<GameObject>(B_WALL_DOOR);
            GameObject bPillarGo = AssetDatabase.LoadAssetAtPath<GameObject>(B_PILLAR);

            GameObject pWallGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_WALL);
            GameObject pDoorGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_DOOR);
            GameObject pArchGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_ARCH);
            GameObject pFloorDryGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_FLOOR_DRY);
            GameObject pFloorShallowGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_FLOOR_SHALLOW);
            GameObject pFloorDeepGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_FLOOR_DEEP);
            GameObject pCeilGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_CEILING);
            GameObject pColGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_COLUMN_ROUND);
            GameObject pWaterGo = AssetDatabase.LoadAssetAtPath<GameObject>(P_WATER);

            Material darkPoolMat = AssetDatabase.LoadAssetAtPath<Material>(MAT_DARK_POOL);
            Material exitSignMat = AssetDatabase.LoadAssetAtPath<Material>(MAT_EXIT_SIGN);

            // 1. BACKROOMS ASSEMBLY
            // B1: 3x2 at gx in [-4..-2], gz in [0..1]
            BuildBackroomsRoom(b1, -4, -2, 0, 1, bTileGo, bWallGo, bWallDoorGo, bPillarGo);
            // B2: 3x4 at gx in [-4..-2], gz in [2..5]
            BuildBackroomsRoom(b2, -4, -2, 2, 5, bTileGo, bWallGo, bWallDoorGo, bPillarGo);
            // B3: 2x2 at gx in [-4..-3], gz in [6..7]
            BuildBackroomsRoom(b3, -4, -3, 6, 7, bTileGo, bWallGo, bWallDoorGo, bPillarGo);
            // B4: 2x2 at gx in [-2..-1], gz in [6..7]
            BuildBackroomsRoom(b4, -2, -1, 6, 7, bTileGo, bWallGo, bWallDoorGo, bPillarGo);

            // Internal Backrooms Partitions for B2 (The maze)
            // Horizontal partitions in B2
            InstantiateBackroomsWall(b2, -3.5f * GRID, 3.5f * GRID, 0, bWallGo);
            InstantiateBackroomsWall(b2, -2.5f * GRID, 3.5f * GRID, 0, bWallGo);
            InstantiateBackroomsWall(b2, -3.5f * GRID, 4.5f * GRID, 0, bWallGo);
            // Vertical partitions in B2
            InstantiateBackroomsWall(b2, -3.0f * GRID, 4.0f * GRID, 90, bWallGo);
            InstantiateBackroomsWall(b2, -2.0f * GRID, 4.0f * GRID, 90, bWallGo);

            // Backrooms Pillars
            InstantiateBackroomsPillar(b1, -3.5f * GRID, 0.5f * GRID, bPillarGo);
            InstantiateBackroomsPillar(b2, -3.5f * GRID, 2.5f * GRID, bPillarGo);
            InstantiateBackroomsPillar(b2, -2.5f * GRID, 2.5f * GRID, bPillarGo);
            InstantiateBackroomsPillar(b2, -3.5f * GRID, 5.0f * GRID, bPillarGo);
            InstantiateBackroomsPillar(b3, -3.5f * GRID, 6.5f * GRID, bPillarGo);

            // 2. POOLROOMS ASSEMBLY
            // P1: 2x2 at [0..1], [6..7]
            BuildPoolroomsRoom(p1, 0, 1, 6, 7, false, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, null);
            // P2: 2x2 at [2..3], [6..7]
            BuildPoolroomsRoom(p2, 2, 3, 6, 7, false, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, null);
            // P3: 2x2 at [4..5], [6..7]
            BuildPoolroomsRoom(p3, 4, 5, 6, 7, false, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, null);

            // P6: 2x3 at [0..1], [3..5] (Flooded basin)
            BuildPoolroomsRoom(p6, 0, 1, 3, 5, true, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, waterRoot);
            // P5: 2x3 at [2..3], [3..5] (Central Hub)
            BuildPoolroomsRoom(p5, 2, 3, 3, 5, false, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, null);
            // P4: 2x3 at [4..5], [3..5] (Flooded corridor)
            BuildPoolroomsRoom(p4, 4, 5, 3, 5, true, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, waterRoot);

            // P7: 2x2 at [0..1], [0..1]
            BuildPoolroomsRoom(p7, 0, 1, 0, 1, false, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, null);
            // P8: 2x2 at [2..3], [0..1]
            BuildPoolroomsRoom(p8, 2, 3, 0, 1, false, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, null);
            // P9: 2x2 at [4..5], [0..1]
            BuildPoolroomsRoom(p9, 4, 5, 0, 1, false, pFloorDryGo, pFloorDeepGo, pCeilGo, pWallGo, pDoorGo, pArchGo, null);

            // Columns in Poolrooms (P1, P2, P3, P5, P7, P8, P9)
            InstantiatePoolColumn(p1, 0.5f * GRID, 6.5f * GRID, pColGo);
            InstantiatePoolColumn(p2, 2.5f * GRID, 6.5f * GRID, pColGo);
            InstantiatePoolColumn(p3, 4.5f * GRID, 6.5f * GRID, pColGo);
            InstantiatePoolColumn(p5, 2.5f * GRID, 4.0f * GRID, pColGo);
            InstantiatePoolColumn(p5, 3.5f * GRID, 4.0f * GRID, pColGo);
            InstantiatePoolColumn(p7, 0.5f * GRID, 0.5f * GRID, pColGo);
            InstantiatePoolColumn(p8, 2.5f * GRID, 0.5f * GRID, pColGo);
            InstantiatePoolColumn(p9, 4.5f * GRID, 0.5f * GRID, pColGo);

            // 3. F1 EXIT CHAMBER (Darkest Poolroom according to game concept)
            // F1: 2x2 at [6..7], [3..4]
            BuildF1ExitRoom(f1, 6, 7, 3, 4, pFloorDryGo, pCeilGo, pWallGo, pDoorGo, darkPoolMat, exitSignMat);

            // 4. TRANSITIONAL DOORWAYS BETWEEN ZONES
            // B1 to B2 doorway at gz=2
            InstantiateBackroomsDoor(b1, -3.0f * GRID, 2.0f * GRID, 0, bWallDoorGo);
            // B2 to B3 doorway at gz=6
            InstantiateBackroomsDoor(b2, -3.5f * GRID, 6.0f * GRID, 0, bWallDoorGo);
            // B3 to B4 doorway at gx=-2
            InstantiateBackroomsDoor(b3, -2.0f * GRID, 6.5f * GRID, 90, bWallDoorGo);
            // B4 to P1 opening at gx=0, gz=6.5
            InstantiateBackroomsDoor(b4, 0.0f * GRID, 6.5f * GRID, 90, bWallDoorGo);

            // Poolrooms internal doors / arches:
            // P1 to P2 arch
            InstantiatePoolArch(p1, 2.0f * GRID, 6.5f * GRID, 90, pArchGo);
            // P2 to P3 arch
            InstantiatePoolArch(p2, 4.0f * GRID, 6.5f * GRID, 90, pArchGo);
            // P1 to P6 opening
            InstantiatePoolArch(p1, 1.0f * GRID, 6.0f * GRID, 0, pArchGo);
            // P2 to P5 opening
            InstantiatePoolArch(p2, 3.0f * GRID, 6.0f * GRID, 0, pArchGo);
            // P3 to P4 opening
            InstantiatePoolArch(p3, 5.0f * GRID, 6.0f * GRID, 0, pArchGo);

            // P6 to P5 opening
            InstantiatePoolArch(p6, 2.0f * GRID, 4.5f * GRID, 90, pArchGo);
            // P5 to P4 opening
            InstantiatePoolArch(p5, 4.0f * GRID, 4.5f * GRID, 90, pArchGo);

            // P6 to P7 opening
            InstantiatePoolArch(p6, 1.0f * GRID, 3.0f * GRID, 0, pArchGo);
            // P5 to P8 opening
            InstantiatePoolArch(p5, 3.0f * GRID, 3.0f * GRID, 0, pArchGo);
            // P4 to P9 opening
            InstantiatePoolArch(p4, 5.0f * GRID, 3.0f * GRID, 0, pArchGo);

            // P7 to P8 opening
            InstantiatePoolArch(p7, 2.0f * GRID, 0.5f * GRID, 90, pArchGo);
            // P8 to P9 opening
            InstantiatePoolArch(p8, 4.0f * GRID, 0.5f * GRID, 90, pArchGo);

            // P4 to F1 Exit Gate opening at gx=6.0, gz=4.0
            InstantiatePoolArch(p4, 6.0f * GRID, 4.0f * GRID, 90, pArchGo);

            // Position Player at B1 START
            var fpc = GameObject.Find("FirstPersonController");
            if (fpc != null)
            {
                // B1 center is around gx = -3, gz = 0.5
                fpc.transform.position = new Vector3(-3.0f * GRID, 0.2f, 0.5f * GRID);
                fpc.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            // Mark all static
            SetStaticRecursively(rootEnv);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            return $"Built environment successfully. Total modules placed under {rootEnv.name}.";
        }

        private static void BuildBackroomsRoom(GameObject parent, int minX, int maxX, int minZ, int maxZ,
            GameObject tilePrefab, GameObject wallPrefab, GameObject doorPrefab, GameObject pillarPrefab)
        {
            // Floor & Ceilings
            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    Vector3 pos = new Vector3((x + 0.5f) * GRID, 0, (z + 0.5f) * GRID);
                    var tile = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, parent.transform);
                    tile.transform.position = pos;
                }
            }

            // Exterior boundary walls
            // North walls (z = maxZ + 1)
            for (int x = minX; x <= maxX; x++)
            {
                InstantiateBackroomsWall(parent, (x + 0.5f) * GRID, (maxZ + 1) * GRID, 0, wallPrefab);
            }
            // South walls (z = minZ)
            for (int x = minX; x <= maxX; x++)
            {
                InstantiateBackroomsWall(parent, (x + 0.5f) * GRID, minZ * GRID, 180, wallPrefab);
            }
            // West walls (x = minX)
            for (int z = minZ; z <= maxZ; z++)
            {
                InstantiateBackroomsWall(parent, minX * GRID, (z + 0.5f) * GRID, -90, wallPrefab);
            }
            // East walls (x = maxX + 1)
            for (int z = minZ; z <= maxZ; z++)
            {
                InstantiateBackroomsWall(parent, (maxX + 1) * GRID, (z + 0.5f) * GRID, 90, wallPrefab);
            }
        }

        private static void InstantiateBackroomsWall(GameObject parent, float x, float z, float rotY, GameObject wallPrefab)
        {
            var wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent.transform);
            wall.transform.position = new Vector3(x, 0, z);
            wall.transform.rotation = Quaternion.Euler(0, rotY, 0);
            EnsureCollider(wall);
        }

        private static void InstantiateBackroomsDoor(GameObject parent, float x, float z, float rotY, GameObject doorPrefab)
        {
            var door = (GameObject)PrefabUtility.InstantiatePrefab(doorPrefab, parent.transform);
            door.transform.position = new Vector3(x, 0, z);
            door.transform.rotation = Quaternion.Euler(0, rotY, 0);
            EnsureCollider(door);
        }

        private static void InstantiateBackroomsPillar(GameObject parent, float x, float z, GameObject pillarPrefab)
        {
            var pillar = (GameObject)PrefabUtility.InstantiatePrefab(pillarPrefab, parent.transform);
            pillar.transform.position = new Vector3(x, 0, z);
            EnsureCollider(pillar);
        }

        private static void BuildPoolroomsRoom(GameObject parent, int minX, int maxX, int minZ, int maxZ, bool isFlooded,
            GameObject floorDry, GameObject floorDeep, GameObject ceiling, GameObject wall, GameObject door, GameObject arch, GameObject waterRoot)
        {
            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    // For Poolrooms FBX, module spans [x*GRID..(x+1)*GRID]
                    Vector3 cornerPos = new Vector3((x + 1) * GRID, 0, (z + 1) * GRID);

                    GameObject floorPrefab = isFlooded ? floorDeep : floorDry;
                    var fl = (GameObject)PrefabUtility.InstantiatePrefab(floorPrefab, parent.transform);
                    fl.transform.position = cornerPos;
                    EnsureCollider(fl);

                    var cl = (GameObject)PrefabUtility.InstantiatePrefab(ceiling, parent.transform);
                    cl.transform.position = cornerPos;
                    EnsureCollider(cl);

                    if (isFlooded && waterRoot != null)
                    {
                        var waterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(P_WATER);
                        var wtr = (GameObject)PrefabUtility.InstantiatePrefab(waterPrefab, waterRoot.transform);
                        // Submerged water surface sits slightly below ground level (-0.2m)
                        wtr.transform.position = new Vector3(cornerPos.x, -0.2f, cornerPos.z);
                    }
                }
            }

            // Outer walls
            // North walls (z = maxZ)
            for (int x = minX; x <= maxX; x++)
            {
                InstantiatePoolWall(parent, (x + 1) * GRID, (maxZ + 1) * GRID, 0, wall);
            }
            // South walls (z = minZ)
            for (int x = minX; x <= maxX; x++)
            {
                InstantiatePoolWall(parent, x * GRID, minZ * GRID, 180, wall);
            }
            // West walls (x = minX)
            for (int z = minZ; z <= maxZ; z++)
            {
                InstantiatePoolWall(parent, minX * GRID, (z + 1) * GRID, -90, wall);
            }
            // East walls (x = maxX)
            for (int z = minZ; z <= maxZ; z++)
            {
                InstantiatePoolWall(parent, (maxX + 1) * GRID, z * GRID, 90, wall);
            }
        }

        private static void InstantiatePoolWall(GameObject parent, float x, float z, float rotY, GameObject wallPrefab)
        {
            var w = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent.transform);
            w.transform.position = new Vector3(x, 0, z);
            w.transform.rotation = Quaternion.Euler(0, rotY, 0);
            EnsureCollider(w);
        }

        private static void InstantiatePoolArch(GameObject parent, float x, float z, float rotY, GameObject archPrefab)
        {
            var arch = (GameObject)PrefabUtility.InstantiatePrefab(archPrefab, parent.transform);
            arch.transform.position = new Vector3(x, 0, z);
            arch.transform.rotation = Quaternion.Euler(0, rotY, 0);
            EnsureCollider(arch);
        }

        private static void InstantiatePoolColumn(GameObject parent, float x, float z, GameObject colPrefab)
        {
            var col = (GameObject)PrefabUtility.InstantiatePrefab(colPrefab, parent.transform);
            col.transform.position = new Vector3(x, 0, z);
            EnsureCollider(col);
        }

        private static void BuildF1ExitRoom(GameObject parent, int minX, int maxX, int minZ, int maxZ,
            GameObject floor, GameObject ceiling, GameObject wall, GameObject door, Material darkMat, Material exitSignMat)
        {
            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    Vector3 cornerPos = new Vector3((x + 1) * GRID, 0, (z + 1) * GRID);
                    var fl = (GameObject)PrefabUtility.InstantiatePrefab(floor, parent.transform);
                    fl.transform.position = cornerPos;
                    ApplyMaterial(fl, darkMat);
                    EnsureCollider(fl);

                    var cl = (GameObject)PrefabUtility.InstantiatePrefab(ceiling, parent.transform);
                    cl.transform.position = cornerPos;
                    ApplyMaterial(cl, darkMat);
                    EnsureCollider(cl);
                }
            }

            // Walls around F1 with dark pool material
            for (int x = minX; x <= maxX; x++)
            {
                var wN = (GameObject)PrefabUtility.InstantiatePrefab(wall, parent.transform);
                wN.transform.position = new Vector3((x + 1) * GRID, 0, (maxZ + 1) * GRID);
                wN.transform.rotation = Quaternion.Euler(0, 0, 0);
                ApplyMaterial(wN, darkMat);
                EnsureCollider(wN);

                var wS = (GameObject)PrefabUtility.InstantiatePrefab(wall, parent.transform);
                wS.transform.position = new Vector3(x * GRID, 0, minZ * GRID);
                wS.transform.rotation = Quaternion.Euler(0, 180, 0);
                ApplyMaterial(wS, darkMat);
                EnsureCollider(wS);
            }

            for (int z = minZ; z <= maxZ; z++)
            {
                var wE = (GameObject)PrefabUtility.InstantiatePrefab(wall, parent.transform);
                wE.transform.position = new Vector3((maxX + 1) * GRID, 0, z * GRID);
                wE.transform.rotation = Quaternion.Euler(0, 90, 0);
                ApplyMaterial(wE, darkMat);
                EnsureCollider(wE);

                // West wall only for non-door tiles
                if (z != 3) // leave doorway opening at z=3/4
                {
                    var wW = (GameObject)PrefabUtility.InstantiatePrefab(wall, parent.transform);
                    wW.transform.position = new Vector3(minX * GRID, 0, (z + 1) * GRID);
                    wW.transform.rotation = Quaternion.Euler(0, -90, 0);
                    ApplyMaterial(wW, darkMat);
                    EnsureCollider(wW);
                }
            }

            // Add Red Illuminated EXIT Sign on the Far East Wall of F1
            GameObject exitSign = GameObject.CreatePrimitive(PrimitiveType.Quad);
            exitSign.name = "Illuminated_EXIT_Sign";
            exitSign.transform.SetParent(parent.transform);
            exitSign.transform.position = new Vector3((maxX + 1) * GRID - 0.05f, 2.2f, (minZ + 0.5f) * GRID);
            exitSign.transform.rotation = Quaternion.Euler(0, -90, 0);
            exitSign.transform.localScale = new Vector3(1.2f, 0.5f, 1f);
            var mr = exitSign.GetComponent<MeshRenderer>();
            if (mr != null && exitSignMat != null)
            {
                mr.sharedMaterial = exitSignMat;
            }
            // Add a soft red point light for the exit glow
            var lightGo = new GameObject("Exit_RedLight");
            lightGo.transform.SetParent(exitSign.transform);
            lightGo.transform.localPosition = new Vector3(0, 0, -0.3f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1.0f, 0.1f, 0.1f);
            light.range = 8.0f;
            light.intensity = 2.5f;
        }

        private static void ApplyMaterial(GameObject go, Material mat)
        {
            if (mat == null) return;
            foreach (var r in go.GetComponentsInChildren<MeshRenderer>())
            {
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = mat;
                }
                r.sharedMaterials = mats;
            }
        }

        private static void EnsureCollider(GameObject go)
        {
            var col = go.GetComponentInChildren<Collider>();
            if (col == null)
            {
                var mf = go.GetComponentInChildren<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    var mc = go.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                }
            }
        }

        private static void SetStaticRecursively(GameObject go)
        {
            go.isStatic = true;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                SetStaticRecursively(go.transform.GetChild(i).gameObject);
            }
        }
    }
}
