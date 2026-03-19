using UnityEngine;

[CreateAssetMenu(menuName = "WOTO/Stage Runtime Config", fileName = "StageRuntimeConfig")]
public class StageRuntimeConfig : ScriptableObject
{
    public GameObject fighterPrefab;
    public RuntimeAnimatorController fighterController;
    public Material arenaFloorMaterial;
    public Material fallbackFighterMaterial;
    public Material skyboxMaterial;
}
