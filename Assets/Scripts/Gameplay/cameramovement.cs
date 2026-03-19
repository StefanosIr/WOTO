using UnityEngine;

public class cameramovement : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("cameramovement has no player assigned.", this);
            enabled = false;
            return;
        }

        offset = transform.position - player.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
