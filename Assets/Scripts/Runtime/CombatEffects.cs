using System.Collections;
using UnityEngine;

public static class CombatEffects
{
    public static IEnumerator HitPause(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }

    public static void SpawnImpact(Vector3 position, Color color, float scale = 0.8f)
    {
        GameObject impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impact.name = "ImpactFlash";
        impact.transform.position = position;
        impact.transform.localScale = Vector3.one * scale;
        impact.GetComponent<Renderer>().sharedMaterial = ProceduralVisualFactory.GetTransparentMaterial(
            "Impact_" + ColorUtility.ToHtmlStringRGBA(color),
            new Color(color.r, color.g, color.b, 0.8f));
        Object.Destroy(impact.GetComponent<Collider>());
        impact.AddComponent<AutoDestroy>().Initialize(0.18f);

        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "ImpactRing";
        ring.transform.position = position;
        ring.transform.localScale = new Vector3(scale * 1.2f, 0.02f, scale * 1.2f);
        ring.GetComponent<Renderer>().sharedMaterial = ProceduralVisualFactory.GetTransparentMaterial(
            "ImpactRing_" + ColorUtility.ToHtmlStringRGBA(color),
            new Color(color.r, color.g, color.b, 0.55f));
        Object.Destroy(ring.GetComponent<Collider>());
        ring.AddComponent<AutoDestroy>().Initialize(0.14f);
    }

    public static void SpawnStrikeArc(Vector3 position, Color color, Vector3 direction)
    {
        GameObject slash = GameObject.CreatePrimitive(PrimitiveType.Quad);
        slash.name = "StrikeArc";
        slash.transform.position = position;
        slash.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        slash.transform.Rotate(0f, 90f, 0f);
        slash.transform.localScale = new Vector3(1.2f, 0.46f, 1f);
        slash.GetComponent<Renderer>().sharedMaterial = ProceduralVisualFactory.GetTransparentMaterial(
            "StrikeArc_" + ColorUtility.ToHtmlStringRGBA(color),
            color);
        Object.Destroy(slash.GetComponent<Collider>());
        slash.AddComponent<AutoDestroy>().Initialize(0.15f);
    }

    public static void SpawnDustBurst(Vector3 position)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject puff = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            puff.name = "DustPuff";
            puff.transform.position = position + Vector3.up * 0.12f;
            puff.transform.localScale = Vector3.one * Random.Range(0.18f, 0.34f);
            puff.GetComponent<Renderer>().sharedMaterial = ProceduralVisualFactory.GetTransparentMaterial(
                "DustMat",
                new Color(0.82f, 0.8f, 0.76f, 0.33f));
            Object.Destroy(puff.GetComponent<Collider>());
            Rigidbody rigidbody = puff.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.linearDamping = 2.5f;
            rigidbody.linearVelocity = new Vector3(Random.Range(-1.2f, 1.2f), Random.Range(0.5f, 1.8f), Random.Range(-0.2f, 0.2f));
            puff.AddComponent<AutoDestroy>().Initialize(0.5f);
        }
    }

    public sealed class AutoDestroy : MonoBehaviour
    {
        private float lifetime;

        public AutoDestroy Initialize(float value)
        {
            lifetime = value;
            return this;
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
