using UnityEngine;

public class FighterPresentation : MonoBehaviour
{
    private Transform visualRoot;
    private Transform shadowQuad;
    private Vector3 visualBaseLocalPosition;
    private float breatheOffset;

    public void Configure(Transform visual, bool firstPlayer)
    {
        visualRoot = visual;
        visualBaseLocalPosition = visualRoot.localPosition;

        HideImportedMesh();
        BuildCustomRig(firstPlayer);
        BuildShadow();
    }

    private void Update()
    {
        if (visualRoot == null)
        {
            return;
        }

        breatheOffset += Time.deltaTime * 2f;
        visualRoot.localPosition = visualBaseLocalPosition + new Vector3(0f, Mathf.Sin(breatheOffset) * 0.012f, 0f);
    }

    private void LateUpdate()
    {
        if (shadowQuad != null)
        {
            shadowQuad.position = new Vector3(transform.position.x, 0.02f, transform.position.z);
            shadowQuad.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    private void HideImportedMesh()
    {
        foreach (Renderer renderer in visualRoot.GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = false;
        }
    }

    private void BuildCustomRig(bool firstPlayer)
    {
        visualRoot.localScale = Vector3.one * 1.55f;

        Transform rigRoot = CreateJoint(visualRoot, "Rig_Hips", new Vector3(0f, 1.05f, 0f));
        Transform spine = CreateJoint(rigRoot, "Rig_Spine", new Vector3(0f, 0.28f, 0f));
        Transform chest = CreateJoint(spine, "Rig_Chest", new Vector3(0f, 0.34f, 0f));
        Transform neck = CreateJoint(chest, "Rig_Neck", new Vector3(0f, 0.22f, 0f));
        Transform head = CreateJoint(neck, "Rig_Head", new Vector3(0f, 0.12f, 0f));

        Transform leftUpperArm = CreateJoint(chest, "Rig_LeftUpperArm", new Vector3(-0.32f, 0.18f, 0f));
        Transform leftLowerArm = CreateJoint(leftUpperArm, "Rig_LeftLowerArm", new Vector3(0f, -0.28f, 0f));
        Transform leftHand = CreateJoint(leftLowerArm, "Rig_LeftHand", new Vector3(0f, -0.24f, 0f));
        Transform rightUpperArm = CreateJoint(chest, "Rig_RightUpperArm", new Vector3(0.32f, 0.18f, 0f));
        Transform rightLowerArm = CreateJoint(rightUpperArm, "Rig_RightLowerArm", new Vector3(0f, -0.28f, 0f));
        Transform rightHand = CreateJoint(rightLowerArm, "Rig_RightHand", new Vector3(0f, -0.24f, 0f));

        Transform leftUpperLeg = CreateJoint(rigRoot, "Rig_LeftUpperLeg", new Vector3(-0.13f, -0.08f, 0f));
        Transform leftLowerLeg = CreateJoint(leftUpperLeg, "Rig_LeftLowerLeg", new Vector3(0f, -0.42f, 0f));
        Transform leftFoot = CreateJoint(leftLowerLeg, "Rig_LeftFoot", new Vector3(0f, -0.42f, 0.03f));
        Transform rightUpperLeg = CreateJoint(rigRoot, "Rig_RightUpperLeg", new Vector3(0.13f, -0.08f, 0f));
        Transform rightLowerLeg = CreateJoint(rightUpperLeg, "Rig_RightLowerLeg", new Vector3(0f, -0.42f, 0f));
        Transform rightFoot = CreateJoint(rightLowerLeg, "Rig_RightFoot", new Vector3(0f, -0.42f, 0.03f));

        Material skin = ProceduralVisualFactory.GetColorMaterial(
            firstPlayer ? "ZeusSkin" : "AresSkin",
            firstPlayer ? new Color(0.9f, 0.82f, 0.73f) : new Color(0.72f, 0.61f, 0.55f),
            0.26f,
            0.02f);
        Material cloth = ProceduralVisualFactory.GetColorMaterial(
            firstPlayer ? "ZeusCloth" : "AresCloth",
            firstPlayer ? new Color(0.96f, 0.97f, 1f) : new Color(0.28f, 0.12f, 0.12f),
            0.16f,
            0.02f);
        Material armor = ProceduralVisualFactory.GetColorMaterial(
            firstPlayer ? "ZeusArmor" : "AresArmor",
            firstPlayer ? new Color(0.84f, 0.74f, 0.34f) : new Color(0.25f, 0.26f, 0.32f),
            0.78f,
            0.92f);
        Material accent = ProceduralVisualFactory.GetColorMaterial(
            firstPlayer ? "ZeusAccent" : "AresAccent",
            firstPlayer ? new Color(0.66f, 0.86f, 1f) : new Color(0.98f, 0.44f, 0.18f),
            0.32f,
            0.05f,
            firstPlayer ? new Color(0.35f, 0.66f, 1f) : new Color(1f, 0.3f, 0.1f));
        Material hair = ProceduralVisualFactory.GetColorMaterial(
            firstPlayer ? "ZeusHair" : "AresHair",
            firstPlayer ? new Color(0.92f, 0.92f, 0.96f) : new Color(0.16f, 0.09f, 0.08f),
            0.22f,
            0.02f);

        CreatePiece(rigRoot, "Pelvis", PrimitiveType.Cube, new Vector3(0f, 0.03f, 0f), new Vector3(0.5f, 0.26f, 0.22f), cloth);
        CreatePiece(spine, "Abdomen", PrimitiveType.Cube, new Vector3(0f, 0.05f, 0f), new Vector3(0.46f, 0.3f, 0.2f), skin);
        CreatePiece(chest, "Torso", PrimitiveType.Cube, new Vector3(0f, 0f, 0f), new Vector3(0.68f, 0.54f, 0.24f), skin);
        CreatePiece(chest, "Cuirass", PrimitiveType.Cube, new Vector3(0f, 0f, 0.11f), new Vector3(0.58f, 0.46f, 0.07f), armor);
        CreatePiece(chest, "LeftPauldron", PrimitiveType.Sphere, new Vector3(-0.3f, 0.12f, 0f), new Vector3(0.24f, 0.18f, 0.22f), armor);
        CreatePiece(chest, "RightPauldron", PrimitiveType.Sphere, new Vector3(0.3f, 0.12f, 0f), new Vector3(0.24f, 0.18f, 0.22f), armor);
        CreatePiece(chest, "Cape", PrimitiveType.Cube, new Vector3(0f, -0.02f, -0.1f), new Vector3(0.54f, 0.9f, 0.03f), firstPlayer ? accent : cloth);
        CreatePiece(chest, "Sigil", PrimitiveType.Sphere, new Vector3(0f, 0.02f, 0.15f), new Vector3(0.11f, 0.11f, 0.06f), accent);

        CreatePiece(head, "Head", PrimitiveType.Sphere, new Vector3(0f, 0.02f, 0f), new Vector3(0.34f, 0.38f, 0.32f), skin);
        CreatePiece(head, "HelmBand", PrimitiveType.Cube, new Vector3(0f, 0.12f, 0f), new Vector3(0.34f, 0.07f, 0.26f), armor);
        CreatePiece(head, "HelmCrest", PrimitiveType.Cube, new Vector3(0f, 0.28f, 0f), new Vector3(0.1f, 0.24f, firstPlayer ? 0.48f : 0.34f), firstPlayer ? hair : accent);
        CreatePiece(head, "FacePlate", PrimitiveType.Cube, new Vector3(0f, 0.03f, 0.18f), new Vector3(0.22f, 0.2f, 0.05f), armor);
        CreatePiece(head, "EyeLeft", PrimitiveType.Sphere, new Vector3(-0.07f, 0.06f, 0.18f), new Vector3(0.034f, 0.034f, 0.034f), accent);
        CreatePiece(head, "EyeRight", PrimitiveType.Sphere, new Vector3(0.07f, 0.06f, 0.18f), new Vector3(0.034f, 0.034f, 0.034f), accent);
        if (firstPlayer)
        {
            CreatePiece(head, "Beard", PrimitiveType.Cube, new Vector3(0f, -0.1f, 0.1f), new Vector3(0.18f, 0.18f, 0.1f), hair);
        }
        else
        {
            CreatePiece(head, "WarWingLeft", PrimitiveType.Cube, new Vector3(-0.18f, 0.16f, 0f), new Vector3(0.06f, 0.18f, 0.1f), accent);
            CreatePiece(head, "WarWingRight", PrimitiveType.Cube, new Vector3(0.18f, 0.16f, 0f), new Vector3(0.06f, 0.18f, 0.1f), accent);
        }

        BuildArm(leftUpperArm, leftLowerArm, leftHand, skin, armor, accent, true);
        BuildArm(rightUpperArm, rightLowerArm, rightHand, skin, armor, accent, false);
        BuildLeg(leftUpperLeg, leftLowerLeg, leftFoot, skin, armor, cloth, accent, true);
        BuildLeg(rightUpperLeg, rightLowerLeg, rightFoot, skin, armor, cloth, accent, false);
    }

    private static void BuildArm(Transform upper, Transform lower, Transform hand, Material skin, Material armor, Material accent, bool left)
    {
        CreatePiece(upper, (left ? "L" : "R") + "_UpperArm", PrimitiveType.Capsule, new Vector3(0f, -0.14f, 0f), new Vector3(0.1f, 0.18f, 0.1f), skin);
        CreatePiece(lower, (left ? "L" : "R") + "_LowerArm", PrimitiveType.Capsule, new Vector3(0f, -0.14f, 0f), new Vector3(0.09f, 0.18f, 0.09f), skin);
        CreatePiece(lower, (left ? "L" : "R") + "_Bracer", PrimitiveType.Cube, new Vector3(0f, -0.13f, 0.02f), new Vector3(0.12f, 0.18f, 0.1f), armor);
        CreatePiece(hand, (left ? "L" : "R") + "_Hand", PrimitiveType.Sphere, new Vector3(0f, 0f, 0f), new Vector3(0.1f, 0.1f, 0.08f), skin);
        CreatePiece(hand, (left ? "L" : "R") + "_Aura", PrimitiveType.Sphere, new Vector3(0f, 0f, 0.04f), new Vector3(0.05f, 0.05f, 0.05f), accent);
    }

    private static void BuildLeg(Transform upper, Transform lower, Transform foot, Material skin, Material armor, Material cloth, Material accent, bool left)
    {
        CreatePiece(upper, (left ? "L" : "R") + "_Thigh", PrimitiveType.Capsule, new Vector3(0f, -0.22f, 0f), new Vector3(0.12f, 0.24f, 0.12f), skin);
        CreatePiece(upper, (left ? "L" : "R") + "_Skirt", PrimitiveType.Cube, new Vector3(0f, -0.08f, 0.08f), new Vector3(0.14f, 0.28f, 0.03f), cloth);
        CreatePiece(lower, (left ? "L" : "R") + "_Shin", PrimitiveType.Capsule, new Vector3(0f, -0.2f, 0f), new Vector3(0.1f, 0.22f, 0.1f), skin);
        CreatePiece(lower, (left ? "L" : "R") + "_Greave", PrimitiveType.Cube, new Vector3(0f, -0.18f, 0.04f), new Vector3(0.13f, 0.22f, 0.08f), armor);
        CreatePiece(foot, (left ? "L" : "R") + "_Sandal", PrimitiveType.Cube, new Vector3(0f, 0.02f, 0.1f), new Vector3(0.14f, 0.05f, 0.22f), accent);
    }

    private void BuildShadow()
    {
        Transform shadow = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
        shadow.name = "ShadowBlob";
        shadow.SetParent(transform, false);
        shadow.localScale = new Vector3(1.8f, 1.8f, 1f);
        shadow.GetComponent<Renderer>().sharedMaterial = ProceduralVisualFactory.GetTransparentMaterial(name + "_Shadow", new Color(0f, 0f, 0f, 0.24f));
        Object.Destroy(shadow.GetComponent<Collider>());
        shadowQuad = shadow;
    }

    private static Transform CreateJoint(Transform parent, string name, Vector3 localPosition)
    {
        GameObject joint = new GameObject(name);
        joint.transform.SetParent(parent, false);
        joint.transform.localPosition = localPosition;
        joint.transform.localRotation = Quaternion.identity;
        joint.transform.localScale = Vector3.one;
        return joint.transform;
    }

    private static void CreatePiece(Transform parent, string name, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Material material)
    {
        if (parent == null)
        {
            return;
        }

        GameObject piece = GameObject.CreatePrimitive(type);
        piece.name = name;
        piece.transform.SetParent(parent, false);
        piece.transform.localPosition = localPosition;
        piece.transform.localRotation = Quaternion.identity;
        piece.transform.localScale = localScale;
        piece.GetComponent<Renderer>().sharedMaterial = material;
        Object.Destroy(piece.GetComponent<Collider>());
    }
}
