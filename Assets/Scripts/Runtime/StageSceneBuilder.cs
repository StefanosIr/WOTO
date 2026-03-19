using UnityEngine;
using UnityEngine.SceneManagement;

public static class StageSceneBuilder
{
    private const string StageSceneName = "stage 1";
    private const string ArenaRootName = "_RecoveredArena";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RebuildStageIfNeeded()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != StageSceneName)
        {
            return;
        }

        GameObject existingArena = GameObject.Find(ArenaRootName);
        if (existingArena != null)
        {
            Object.Destroy(existingArena);
        }

        BuildStage();
    }

    private static void BuildStage()
    {
        StageRuntimeConfig config = Resources.Load<StageRuntimeConfig>("StageRuntimeConfig");

        GameObject arenaRoot = new GameObject(ArenaRootName);
        BuildEnvironment(arenaRoot.transform, config);

        GameObject fighterOne = CreateFighter("Zeus Champion", new Vector3(-3.6f, 0.5f, 0f), Quaternion.Euler(0f, 90f, 0f), true, config);
        GameObject fighterTwo = CreateFighter("Ares Champion", new Vector3(3.6f, 0.5f, 0f), Quaternion.Euler(0f, -90f, 0f), false, config);
        fighterOne.transform.SetParent(arenaRoot.transform);
        fighterTwo.transform.SetParent(arenaRoot.transform);

        FighterGameplay gameplayOne = fighterOne.GetComponent<FighterGameplay>();
        FighterGameplay gameplayTwo = fighterTwo.GetComponent<FighterGameplay>();
        gameplayOne.SetOpponent(gameplayTwo);
        gameplayTwo.SetOpponent(gameplayOne);

        GameObject matchObject = new GameObject("_MatchController");
        matchObject.transform.SetParent(arenaRoot.transform);
        MatchController match = matchObject.AddComponent<MatchController>();
        match.Initialize(gameplayOne, gameplayTwo);

        ConfigureCamera(fighterOne.transform, fighterTwo.transform);
        StageHudBuilder.Build(gameplayOne, gameplayTwo, match);
    }

    private static void BuildEnvironment(Transform parent, StageRuntimeConfig config)
    {
        RenderSettings.ambientLight = new Color(0.78f, 0.79f, 0.84f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.64f, 0.67f, 0.76f, 1f);
        RenderSettings.fogDensity = 0.0018f;

        if (config != null && config.skyboxMaterial != null)
        {
            RenderSettings.skybox = config.skyboxMaterial;
        }

        Material marble = ProceduralVisualFactory.GetMarbleMaterial("OlympusMarble", new Color(0.91f, 0.92f, 0.95f), new Color(0.7f, 0.74f, 0.82f), 0.58f, 0.04f);
        Material darkStone = ProceduralVisualFactory.GetMarbleMaterial("BasaltStone", new Color(0.16f, 0.16f, 0.2f), new Color(0.27f, 0.24f, 0.22f), 0.24f, 0.03f);
        Material gold = ProceduralVisualFactory.GetColorMaterial("OlympusGold", new Color(0.84f, 0.69f, 0.24f), 0.82f, 0.95f);
        Material bronze = ProceduralVisualFactory.GetColorMaterial("OlympusBronze", new Color(0.61f, 0.39f, 0.19f), 0.63f, 0.82f);
        Material inlaidFloor = config != null && config.arenaFloorMaterial != null
            ? config.arenaFloorMaterial
            : ProceduralVisualFactory.GetMarbleMaterial("ArenaInlay", new Color(0.34f, 0.34f, 0.42f), new Color(0.18f, 0.18f, 0.24f), 0.3f, 0.02f);

        CreateBlock(parent, "LowerPodium", new Vector3(0f, -3.5f, 0f), new Vector3(66f, 7f, 42f), darkStone);
        CreateBlock(parent, "ArenaPlatform", new Vector3(0f, -0.5f, 0f), new Vector3(46f, 1f, 28f), marble);
        CreateBlock(parent, "CombatFloor", new Vector3(0f, 0.05f, 0f), new Vector3(40f, 0.1f, 18f), inlaidFloor);
        BuildFloorInlays(parent, gold, darkStone);

        BuildCenterEmblem(parent, gold, darkStone);
        CreateStairRun(parent, new Vector3(0f, -0.1f, 10f), 6, 20f, 1.2f, 0.45f, marble, true);
        CreateStairRun(parent, new Vector3(0f, -0.1f, -10f), 6, 20f, 1.2f, 0.45f, marble, false);

        CreateParapet(parent, "NorthParapet", new Vector3(0f, 1.3f, 13.6f), new Vector3(43f, 2.4f, 1f), darkStone);
        CreateParapet(parent, "SouthParapet", new Vector3(0f, 1.3f, -13.6f), new Vector3(43f, 2.4f, 1f), darkStone);
        CreateParapet(parent, "EastParapet", new Vector3(21.4f, 1.3f, 0f), new Vector3(1f, 2.4f, 27f), darkStone);
        CreateParapet(parent, "WestParapet", new Vector3(-21.4f, 1.3f, 0f), new Vector3(1f, 2.4f, 27f), darkStone);

        BuildTempleBackdrop(parent, marble, darkStone, gold);
        BuildColumns(parent, marble, gold);
        BuildBraziers(parent, bronze);
        BuildStatues(parent, marble, gold);
        BuildCliffWalls(parent, darkStone);
        BuildFloatingRocks(parent, darkStone);

        Light directionalLight = Object.FindAnyObjectByType<Light>();
        if (directionalLight == null)
        {
            GameObject lightObject = new GameObject("Directional Light");
            directionalLight = lightObject.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
        }

        directionalLight.transform.rotation = Quaternion.Euler(34f, -20f, 0f);
        directionalLight.intensity = 1.55f;
        directionalLight.shadows = LightShadows.Soft;

        CreateFillLight(parent, new Vector3(0f, 9f, -8f), new Color(0.62f, 0.66f, 0.78f), 1.1f);
        CreateFillLight(parent, new Vector3(-12f, 4f, -6f), new Color(0.95f, 0.78f, 0.62f), 0.65f);
        CreateFillLight(parent, new Vector3(12f, 4f, -6f), new Color(0.95f, 0.78f, 0.62f), 0.65f);
    }

    private static void BuildTempleBackdrop(Transform parent, Material marble, Material darkStone, Material gold)
    {
        CreateBlock(parent, "TempleBase", new Vector3(0f, 1.3f, 13.8f), new Vector3(20f, 1.2f, 4f), marble);
        CreateBlock(parent, "TempleRoof", new Vector3(0f, 8.2f, 13.8f), new Vector3(22f, 1f, 5f), darkStone);
        CreateBlock(parent, "TemplePediment", new Vector3(0f, 10.2f, 13.8f), new Vector3(18f, 0.7f, 2.5f), gold);

        for (int i = -4; i <= 4; i += 2)
        {
            if (i == 0)
            {
                continue;
            }

            CreateColumn(parent, $"TempleColumn_{i}", new Vector3(i * 1.7f, 4.2f, 13.3f), 0.65f, 7.4f, marble, gold);
        }
    }

    private static void BuildColumns(Transform parent, Material marble, Material gold)
    {
        float[] xPositions = { -19f, -13f, 13f, 19f };
        foreach (float x in xPositions)
        {
            CreateColumn(parent, "NorthColumn_" + x, new Vector3(x, 3.2f, -12.2f), 0.55f, 6.4f, marble, gold);
            CreateColumn(parent, "SouthColumn_" + x, new Vector3(x, 3.2f, 12.2f), 0.55f, 6.4f, marble, gold);
        }
    }

    private static void BuildFloorInlays(Transform parent, Material gold, Material darkStone)
    {
        for (int i = -4; i <= 4; i++)
        {
            CreateBlock(parent, "FloorStripeX_" + i, new Vector3(i * 4f, 0.09f, 0f), new Vector3(0.16f, 0.02f, 18f), i == 0 ? gold : darkStone);
        }

        for (int i = -2; i <= 2; i++)
        {
            CreateBlock(parent, "FloorStripeZ_" + i, new Vector3(0f, 0.091f, i * 4f), new Vector3(40f, 0.02f, 0.16f), i == 0 ? gold : darkStone);
        }
    }

    private static void BuildCenterEmblem(Transform parent, Material gold, Material darkStone)
    {
        CreateBlock(parent, "CenterBarX", new Vector3(0f, 0.095f, 0f), new Vector3(3.4f, 0.02f, 0.22f), gold);
        CreateBlock(parent, "CenterBarZ", new Vector3(0f, 0.095f, 0f), new Vector3(0.22f, 0.02f, 3.4f), gold);
        CreateBlock(parent, "CenterCore", new Vector3(0f, 0.097f, 0f), new Vector3(0.6f, 0.03f, 0.6f), darkStone);
        CreateBlock(parent, "CenterWingNE", new Vector3(1.2f, 0.096f, 1.2f), new Vector3(0.6f, 0.02f, 0.18f), gold);
        CreateBlock(parent, "CenterWingNW", new Vector3(-1.2f, 0.096f, 1.2f), new Vector3(0.6f, 0.02f, 0.18f), gold);
        CreateBlock(parent, "CenterWingSE", new Vector3(1.2f, 0.096f, -1.2f), new Vector3(0.6f, 0.02f, 0.18f), gold);
        CreateBlock(parent, "CenterWingSW", new Vector3(-1.2f, 0.096f, -1.2f), new Vector3(0.6f, 0.02f, 0.18f), gold);
    }

    private static void BuildBraziers(Transform parent, Material bronze)
    {
        CreateBrazier(parent, new Vector3(-12f, 1f, -7.5f), bronze);
        CreateBrazier(parent, new Vector3(12f, 1f, -7.5f), bronze);
        CreateBrazier(parent, new Vector3(-12f, 1f, 7.5f), bronze);
        CreateBrazier(parent, new Vector3(12f, 1f, 7.5f), bronze);
    }

    private static void BuildStatues(Transform parent, Material marble, Material gold)
    {
        CreateStatue(parent, "StatueLeft", new Vector3(-18f, 0f, 0f), marble, gold);
        CreateStatue(parent, "StatueRight", new Vector3(18f, 0f, 0f), marble, gold);
    }

    private static void BuildFloatingRocks(Transform parent, Material darkStone)
    {
        Random.InitState(11);
        for (int i = 0; i < 18; i++)
        {
            Vector3 position = new Vector3(Random.Range(-48f, 48f), Random.Range(9f, 24f), Random.Range(-38f, 38f));
            Vector3 scale = Vector3.one * Random.Range(1.2f, 4.8f);
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "FloatingRock_" + i;
            rock.transform.SetParent(parent);
            rock.transform.position = position;
            rock.transform.localScale = scale;
            rock.GetComponent<Renderer>().sharedMaterial = darkStone;
            Object.Destroy(rock.GetComponent<Collider>());
        }
    }

    private static void BuildCliffWalls(Transform parent, Material darkStone)
    {
        CreateBlock(parent, "BackCliffLeft", new Vector3(-34f, 8f, 0f), new Vector3(12f, 20f, 54f), darkStone);
        CreateBlock(parent, "BackCliffRight", new Vector3(34f, 8f, 0f), new Vector3(12f, 20f, 54f), darkStone);
        CreateBlock(parent, "RearBridge", new Vector3(0f, 11.8f, 24.5f), new Vector3(22f, 1.8f, 5f), darkStone);
    }

    private static void CreateRing(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = name;
        ring.transform.SetParent(parent);
        ring.transform.position = position;
        ring.transform.localScale = scale;
        ring.GetComponent<Renderer>().sharedMaterial = material;
        Object.Destroy(ring.GetComponent<Collider>());
    }

    private static void CreateStairRun(Transform parent, Vector3 startPosition, int steps, float width, float depth, float height, Material material, bool facePositiveZ)
    {
        for (int i = 0; i < steps; i++)
        {
            float zOffset = facePositiveZ ? i * depth : -i * depth;
            float yOffset = -i * height;
            CreateBlock(
                parent,
                (facePositiveZ ? "NorthStep_" : "SouthStep_") + i,
                startPosition + new Vector3(0f, yOffset, zOffset),
                new Vector3(width + i * 1.4f, height, depth + 0.1f),
                material);
        }
    }

    private static void CreateParapet(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        CreateBlock(parent, name, position, scale, material);
    }

    private static void CreateColumn(Transform parent, string name, Vector3 position, float radius, float height, Material shaftMaterial, Material capMaterial)
    {
        CreateBlock(parent, name + "_Base", position + new Vector3(0f, -height * 0.5f, 0f), new Vector3(radius * 2.4f, 0.45f, radius * 2.4f), capMaterial);
        CreateBlock(parent, name + "_Cap", position + new Vector3(0f, height * 0.5f, 0f), new Vector3(radius * 2.8f, 0.42f, radius * 2.8f), capMaterial);
        for (int i = 0; i < 4; i++)
        {
            float segmentHeight = height / 4f;
            float segmentY = position.y - height * 0.375f + i * segmentHeight;
            GameObject shaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shaft.name = name + "_Segment_" + i;
            shaft.transform.SetParent(parent);
            shaft.transform.position = new Vector3(position.x, segmentY, position.z);
            shaft.transform.localScale = new Vector3(radius * (i % 2 == 0 ? 0.96f : 0.88f), segmentHeight * 0.5f, radius * (i % 2 == 0 ? 0.96f : 0.88f));
            shaft.GetComponent<Renderer>().sharedMaterial = shaftMaterial;
            Object.Destroy(shaft.GetComponent<Collider>());
        }
    }

    private static void CreateBrazier(Transform parent, Vector3 position, Material bronze)
    {
        CreateBlock(parent, "BrazierPedestal", position + new Vector3(0f, -0.2f, 0f), new Vector3(1.1f, 0.8f, 1.1f), bronze);

        GameObject bowl = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bowl.name = "BrazierBowl";
        bowl.transform.SetParent(parent);
        bowl.transform.position = position + new Vector3(0f, 0.55f, 0f);
        bowl.transform.localScale = new Vector3(0.95f, 0.35f, 0.95f);
        bowl.GetComponent<Renderer>().sharedMaterial = bronze;
        Object.Destroy(bowl.GetComponent<Collider>());

        GameObject flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        flame.name = "BrazierFlame";
        flame.transform.SetParent(parent);
        flame.transform.position = position + new Vector3(0f, 1.05f, 0f);
        flame.transform.localScale = new Vector3(0.55f, 0.9f, 0.55f);
        flame.GetComponent<Renderer>().sharedMaterial = ProceduralVisualFactory.GetColorMaterial(
            "BrazierFire",
            new Color(1f, 0.46f, 0.12f),
            0f,
            0f,
            new Color(1.2f, 0.45f, 0.08f));
        Object.Destroy(flame.GetComponent<Collider>());

        GameObject lightObject = new GameObject("BrazierLight");
        lightObject.transform.SetParent(parent);
        lightObject.transform.position = position + new Vector3(0f, 1.2f, 0f);
        Light pointLight = lightObject.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.range = 12f;
        pointLight.intensity = 3.5f;
        pointLight.color = new Color(1f, 0.58f, 0.22f);
    }

    private static void CreateStatue(Transform parent, string name, Vector3 position, Material marble, Material gold)
    {
        Transform statueRoot = new GameObject(name).transform;
        statueRoot.SetParent(parent);
        statueRoot.position = position;

        CreateBlock(statueRoot, "Pedestal", new Vector3(0f, 1f, 0f), new Vector3(3f, 2f, 3f), gold);
        CreateColumn(statueRoot, "Body", new Vector3(0f, 5f, 0f), 0.85f, 6f, marble, gold);
        CreateBlock(statueRoot, "Arms", new Vector3(0f, 6.2f, 0f), new Vector3(2.8f, 0.35f, 0.35f), marble);
        CreateBlock(statueRoot, "Head", new Vector3(0f, 8.5f, 0f), new Vector3(1.1f, 1.1f, 1.1f), marble);
        CreateBlock(statueRoot, "Crown", new Vector3(0f, 9.25f, 0f), new Vector3(1.6f, 0.2f, 1.2f), gold);
    }

    private static GameObject CreateBlock(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = name;
        block.transform.SetParent(parent);
        block.transform.localPosition = position;
        block.transform.localScale = scale;
        block.GetComponent<Renderer>().sharedMaterial = material;
        return block;
    }

    private static void CreateFillLight(Transform parent, Vector3 position, Color color, float intensity)
    {
        GameObject fill = new GameObject("Fill Light");
        fill.transform.SetParent(parent);
        fill.transform.localPosition = position;
        Light light = fill.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = 60f;
    }

    private static GameObject CreateFighter(string name, Vector3 position, Quaternion rotation, bool firstPlayer, StageRuntimeConfig config)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        root.transform.rotation = rotation;

        CharacterController controller = root.AddComponent<CharacterController>();
        controller.center = new Vector3(0f, 1.1f, 0f);
        controller.height = 2.25f;
        controller.radius = 0.42f;
        controller.stepOffset = 0.3f;

        GameObject visual = CreateVisualRoot(root.transform, config);
        Animator animator = visual.GetComponentInChildren<Animator>();
        if (animator == null)
        {
            animator = root.AddComponent<Animator>();
        }
        animator.applyRootMotion = false;
        animator.enabled = false;

        FighterPresentation presentation = root.AddComponent<FighterPresentation>();
        presentation.Configure(visual.transform, firstPlayer);

        FighterGameplay gameplay = root.AddComponent<FighterGameplay>();
        gameplay.Initialize(name, firstPlayer);

        SimpleFighterAnimator simpleAnimator = root.AddComponent<SimpleFighterAnimator>();
        simpleAnimator.Initialize(gameplay, visual.transform);

        return root;
    }

    private static GameObject CreateVisualRoot(Transform parent, StageRuntimeConfig config)
    {
        GameObject visual = new GameObject("VisualRig");
        visual.transform.SetParent(parent);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = Vector3.one;

        GameObject modelRoot;
        if (config != null && config.fighterPrefab != null)
        {
            modelRoot = Object.Instantiate(config.fighterPrefab, visual.transform);
            modelRoot.name = "AnimatedModel";
            modelRoot.transform.localPosition = Vector3.zero;
            modelRoot.transform.localRotation = Quaternion.identity;
            modelRoot.transform.localScale = Vector3.one * 1.55f;
        }
        else
        {
            modelRoot = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            modelRoot.name = "AnimatedModel";
            modelRoot.transform.SetParent(visual.transform);
            modelRoot.transform.localPosition = Vector3.up;
            modelRoot.transform.localRotation = Quaternion.identity;
        }

        if (config != null && config.fallbackFighterMaterial != null)
        {
            foreach (Renderer renderer in visual.GetComponentsInChildren<Renderer>(true))
            {
                renderer.sharedMaterial = config.fallbackFighterMaterial;
            }
        }

        return visual;
    }

    private static void ConfigureCamera(Transform fighterOne, Transform fighterTwo)
    {
        Camera camera = Camera.main;
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.AddComponent<AudioListener>();
        }

        ArenaCameraRig rig = camera.GetComponent<ArenaCameraRig>();
        if (rig == null)
        {
            rig = camera.gameObject.AddComponent<ArenaCameraRig>();
        }

        rig.SetTargets(fighterOne, fighterTwo);
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.fieldOfView = 36f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 250f;
    }
}
