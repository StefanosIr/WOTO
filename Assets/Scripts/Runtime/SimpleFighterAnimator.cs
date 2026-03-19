using UnityEngine;

public class SimpleFighterAnimator : MonoBehaviour
{
    private FighterGameplay gameplay;
    private Transform visualRoot;
    private Transform hips;
    private Transform chest;
    private Transform head;
    private Transform leftUpperArm;
    private Transform rightUpperArm;
    private Transform leftLowerArm;
    private Transform rightLowerArm;
    private Transform leftUpperLeg;
    private Transform rightUpperLeg;
    private Transform leftLowerLeg;
    private Transform rightLowerLeg;

    private Vector3 hipsBasePosition;
    private Quaternion chestBaseRotation;
    private Quaternion headBaseRotation;
    private Quaternion leftUpperArmBaseRotation;
    private Quaternion rightUpperArmBaseRotation;
    private Quaternion leftLowerArmBaseRotation;
    private Quaternion rightLowerArmBaseRotation;
    private Quaternion leftUpperLegBaseRotation;
    private Quaternion rightUpperLegBaseRotation;
    private Quaternion leftLowerLegBaseRotation;
    private Quaternion rightLowerLegBaseRotation;
    private float cycle;

    public void Initialize(FighterGameplay owner, Transform visual)
    {
        gameplay = owner;
        visualRoot = visual;

        Animator animator = visual.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        hips = FindBone(animator, HumanBodyBones.Hips, "Rig_Hips", "mixamorig:Hips", "Hips");
        chest = FindBone(animator, HumanBodyBones.Chest, "Rig_Chest", "mixamorig:Spine2", "Spine2");
        head = FindBone(animator, HumanBodyBones.Head, "Rig_Head", "mixamorig:Head", "Head");
        leftUpperArm = FindBone(animator, HumanBodyBones.LeftUpperArm, "Rig_LeftUpperArm", "mixamorig:LeftArm", "LeftArm");
        rightUpperArm = FindBone(animator, HumanBodyBones.RightUpperArm, "Rig_RightUpperArm", "mixamorig:RightArm", "RightArm");
        leftLowerArm = FindBone(animator, HumanBodyBones.LeftLowerArm, "Rig_LeftLowerArm", "mixamorig:LeftForeArm", "LeftForeArm");
        rightLowerArm = FindBone(animator, HumanBodyBones.RightLowerArm, "Rig_RightLowerArm", "mixamorig:RightForeArm", "RightForeArm");
        leftUpperLeg = FindBone(animator, HumanBodyBones.LeftUpperLeg, "Rig_LeftUpperLeg", "mixamorig:LeftUpLeg", "LeftUpLeg");
        rightUpperLeg = FindBone(animator, HumanBodyBones.RightUpperLeg, "Rig_RightUpperLeg", "mixamorig:RightUpLeg", "RightUpLeg");
        leftLowerLeg = FindBone(animator, HumanBodyBones.LeftLowerLeg, "Rig_LeftLowerLeg", "mixamorig:LeftLeg", "LeftLeg");
        rightLowerLeg = FindBone(animator, HumanBodyBones.RightLowerLeg, "Rig_RightLowerLeg", "mixamorig:RightLeg", "RightLeg");

        hipsBasePosition = hips != null ? hips.localPosition : Vector3.zero;
        chestBaseRotation = chest != null ? chest.localRotation : Quaternion.identity;
        headBaseRotation = head != null ? head.localRotation : Quaternion.identity;
        leftUpperArmBaseRotation = leftUpperArm != null ? leftUpperArm.localRotation : Quaternion.identity;
        rightUpperArmBaseRotation = rightUpperArm != null ? rightUpperArm.localRotation : Quaternion.identity;
        leftLowerArmBaseRotation = leftLowerArm != null ? leftLowerArm.localRotation : Quaternion.identity;
        rightLowerArmBaseRotation = rightLowerArm != null ? rightLowerArm.localRotation : Quaternion.identity;
        leftUpperLegBaseRotation = leftUpperLeg != null ? leftUpperLeg.localRotation : Quaternion.identity;
        rightUpperLegBaseRotation = rightUpperLeg != null ? rightUpperLeg.localRotation : Quaternion.identity;
        leftLowerLegBaseRotation = leftLowerLeg != null ? leftLowerLeg.localRotation : Quaternion.identity;
        rightLowerLegBaseRotation = rightLowerLeg != null ? rightLowerLeg.localRotation : Quaternion.identity;
    }

    private void LateUpdate()
    {
        if (gameplay == null || visualRoot == null)
        {
            return;
        }

        cycle += Time.deltaTime * Mathf.Lerp(1.2f, 6.5f, gameplay.HorizontalSpeedNormalized);
        ApplyBasePose();
        ApplyLocomotion();
        ApplyStatePose();
    }

    private void ApplyBasePose()
    {
        if (hips != null)
        {
            hips.localPosition = hipsBasePosition + new Vector3(0f, Mathf.Sin(cycle * 0.7f) * 0.02f, 0f);
        }

        SetLocalRotation(chest, chestBaseRotation * Quaternion.Euler(-4f, 0f, 0f));
        SetLocalRotation(head, headBaseRotation * Quaternion.Euler(4f, 0f, 0f));
        SetLocalRotation(leftUpperArm, leftUpperArmBaseRotation * Quaternion.Euler(18f, 0f, 18f));
        SetLocalRotation(rightUpperArm, rightUpperArmBaseRotation * Quaternion.Euler(18f, 0f, -18f));
        SetLocalRotation(leftLowerArm, leftLowerArmBaseRotation * Quaternion.Euler(-26f, 0f, 0f));
        SetLocalRotation(rightLowerArm, rightLowerArmBaseRotation * Quaternion.Euler(-26f, 0f, 0f));
        SetLocalRotation(leftUpperLeg, leftUpperLegBaseRotation);
        SetLocalRotation(rightUpperLeg, rightUpperLegBaseRotation);
        SetLocalRotation(leftLowerLeg, leftLowerLegBaseRotation);
        SetLocalRotation(rightLowerLeg, rightLowerLegBaseRotation);
    }

    private void ApplyLocomotion()
    {
        float move = gameplay.MoveInput;
        float speed = gameplay.HorizontalSpeedNormalized;
        if (speed < 0.02f)
        {
            return;
        }

        float swing = Mathf.Sin(cycle * 2.2f) * (24f + 12f * speed);
        float armSwing = swing * Mathf.Sign(move == 0f ? 1f : move);

        SetLocalRotation(leftUpperLeg, leftUpperLegBaseRotation * Quaternion.Euler(swing, 0f, 0f));
        SetLocalRotation(rightUpperLeg, rightUpperLegBaseRotation * Quaternion.Euler(-swing, 0f, 0f));
        SetLocalRotation(leftLowerLeg, leftLowerLegBaseRotation * Quaternion.Euler(Mathf.Max(0f, -swing) * 0.9f, 0f, 0f));
        SetLocalRotation(rightLowerLeg, rightLowerLegBaseRotation * Quaternion.Euler(Mathf.Max(0f, swing) * 0.9f, 0f, 0f));
        SetLocalRotation(leftUpperArm, leftUpperArm.localRotation * Quaternion.Euler(-armSwing * 0.55f, 0f, 0f));
        SetLocalRotation(rightUpperArm, rightUpperArm.localRotation * Quaternion.Euler(armSwing * 0.55f, 0f, 0f));
        SetLocalRotation(chest, chest.localRotation * Quaternion.Euler(0f, armSwing * 0.08f, 0f));
    }

    private void ApplyStatePose()
    {
        switch (gameplay.CurrentVisualState)
        {
            case FighterGameplay.VisualState.Jump:
                SetLocalRotation(leftUpperArm, leftUpperArmBaseRotation * Quaternion.Euler(-52f, 0f, 18f));
                SetLocalRotation(rightUpperArm, rightUpperArmBaseRotation * Quaternion.Euler(-52f, 0f, -18f));
                SetLocalRotation(leftUpperLeg, leftUpperLegBaseRotation * Quaternion.Euler(28f, 0f, 0f));
                SetLocalRotation(rightUpperLeg, rightUpperLegBaseRotation * Quaternion.Euler(18f, 0f, 0f));
                SetLocalRotation(chest, chestBaseRotation * Quaternion.Euler(-14f, 0f, 0f));
                break;
            case FighterGameplay.VisualState.Punch:
                SetLocalRotation(rightUpperArm, rightUpperArmBaseRotation * Quaternion.Euler(-96f, 12f, -24f));
                SetLocalRotation(rightLowerArm, rightLowerArmBaseRotation * Quaternion.Euler(-18f, 0f, 0f));
                SetLocalRotation(leftUpperArm, leftUpperArmBaseRotation * Quaternion.Euler(36f, 0f, 20f));
                SetLocalRotation(leftLowerArm, leftLowerArmBaseRotation * Quaternion.Euler(-36f, 0f, 0f));
                SetLocalRotation(chest, chestBaseRotation * Quaternion.Euler(-18f, 14f, 0f));
                SetLocalRotation(head, headBaseRotation * Quaternion.Euler(-4f, 10f, 0f));
                break;
            case FighterGameplay.VisualState.Kick:
                SetLocalRotation(rightUpperLeg, rightUpperLegBaseRotation * Quaternion.Euler(-72f, 0f, 0f));
                SetLocalRotation(rightLowerLeg, rightLowerLegBaseRotation * Quaternion.Euler(38f, 0f, 0f));
                SetLocalRotation(leftUpperLeg, leftUpperLegBaseRotation * Quaternion.Euler(12f, 0f, 0f));
                SetLocalRotation(leftUpperArm, leftUpperArmBaseRotation * Quaternion.Euler(-18f, 0f, 16f));
                SetLocalRotation(rightUpperArm, rightUpperArmBaseRotation * Quaternion.Euler(32f, 0f, -16f));
                SetLocalRotation(chest, chestBaseRotation * Quaternion.Euler(-12f, 16f, 0f));
                SetLocalRotation(head, headBaseRotation * Quaternion.Euler(-2f, 12f, 0f));
                break;
            case FighterGameplay.VisualState.Ultra:
                SetLocalRotation(leftUpperArm, leftUpperArmBaseRotation * Quaternion.Euler(-84f, 0f, 34f));
                SetLocalRotation(rightUpperArm, rightUpperArmBaseRotation * Quaternion.Euler(-84f, 0f, -34f));
                SetLocalRotation(leftLowerArm, leftLowerArmBaseRotation * Quaternion.Euler(-44f, 0f, 0f));
                SetLocalRotation(rightLowerArm, rightLowerArmBaseRotation * Quaternion.Euler(-44f, 0f, 0f));
                SetLocalRotation(chest, chestBaseRotation * Quaternion.Euler(-20f, 0f, 0f));
                SetLocalRotation(head, headBaseRotation * Quaternion.Euler(-8f, 0f, 0f));
                break;
            case FighterGameplay.VisualState.Hit:
                SetLocalRotation(chest, chestBaseRotation * Quaternion.Euler(22f, -14f, 0f));
                SetLocalRotation(head, headBaseRotation * Quaternion.Euler(18f, -10f, 0f));
                SetLocalRotation(leftUpperArm, leftUpperArmBaseRotation * Quaternion.Euler(42f, 0f, 22f));
                SetLocalRotation(rightUpperArm, rightUpperArmBaseRotation * Quaternion.Euler(42f, 0f, -22f));
                break;
            case FighterGameplay.VisualState.Defeated:
                if (hips != null)
                {
                    hips.localPosition = hipsBasePosition + new Vector3(0f, -0.28f, 0f);
                }

                SetLocalRotation(chest, chestBaseRotation * Quaternion.Euler(40f, 0f, 0f));
                SetLocalRotation(head, headBaseRotation * Quaternion.Euler(28f, 0f, 0f));
                SetLocalRotation(leftUpperLeg, leftUpperLegBaseRotation * Quaternion.Euler(30f, 0f, 0f));
                SetLocalRotation(rightUpperLeg, rightUpperLegBaseRotation * Quaternion.Euler(18f, 0f, 0f));
                break;
        }
    }

    private Transform FindBone(Animator animator, HumanBodyBones humanBone, params string[] fallbackNames)
    {
        if (animator != null && animator.avatar != null && animator.avatar.isHuman)
        {
            Transform bone = animator.GetBoneTransform(humanBone);
            if (bone != null)
            {
                return bone;
            }
        }

        foreach (string fallbackName in fallbackNames)
        {
            foreach (Transform child in visualRoot.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == fallbackName)
                {
                    return child;
                }
            }
        }

        return null;
    }

    private static void SetLocalRotation(Transform bone, Quaternion rotation)
    {
        if (bone != null)
        {
            bone.localRotation = rotation;
        }
    }
}
