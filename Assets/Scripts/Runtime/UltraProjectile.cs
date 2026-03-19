using UnityEngine;

public class UltraProjectile : MonoBehaviour
{
    private FighterGameplay owner;
    private FighterGameplay target;
    private float speed;
    private float damage;
    private float lifetime;
    private Transform core;
    private Transform trail;
    private Transform ringA;
    private Transform ringB;

    public void Initialize(FighterGameplay projectileOwner, FighterGameplay projectileTarget, float projectileSpeed, float projectileDamage, float projectileLifetime)
    {
        owner = projectileOwner;
        target = projectileTarget;
        speed = projectileSpeed;
        damage = projectileDamage;
        lifetime = projectileLifetime;
        BuildVisuals();
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        UpdateVisuals();

        Vector3 direction;
        if (target == null)
        {
            direction = transform.forward;
        }
        else
        {
            Vector3 aimPoint = target.transform.position + Vector3.up * 1.1f;
            direction = (aimPoint - transform.position).normalized;

            if (Vector3.Distance(transform.position, aimPoint) < 1.1f)
            {
                target.TakeDamage(damage, new Vector3(Mathf.Sign(direction.x) * 6.5f, 0f, 0f));
                CombatEffects.SpawnImpact(aimPoint, new Color(0.98f, 0.86f, 0.45f), 1.5f);
                Destroy(gameObject);
                return;
            }
        }

        direction.y = 0f;
        direction.z = 0f;
        direction.Normalize();

        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void BuildVisuals()
    {
        Renderer rootRenderer = GetComponent<Renderer>();
        if (rootRenderer != null)
        {
            rootRenderer.enabled = false;
        }

        if (TryGetComponent<Collider>(out Collider collider))
        {
            Destroy(collider);
        }

        Color beamColor = owner != null && owner.name.Contains("Zeus")
            ? new Color(0.68f, 0.9f, 1f, 0.92f)
            : new Color(1f, 0.55f, 0.28f, 0.92f);

        Material glowMaterial = ProceduralVisualFactory.GetTransparentMaterial("UltraGlow_" + ColorUtility.ToHtmlStringRGBA(beamColor), beamColor);
        Material coreMaterial = ProceduralVisualFactory.GetColorMaterial("UltraCore_" + ColorUtility.ToHtmlStringRGBA(beamColor), new Color(beamColor.r, beamColor.g, beamColor.b, 1f), 0.72f, 0.04f, beamColor * 1.3f);

        core = CreateChildPrimitive("Core", PrimitiveType.Sphere, glowMaterial, new Vector3(0.6f, 0.6f, 0.6f), Vector3.zero);
        trail = CreateChildPrimitive("Trail", PrimitiveType.Cylinder, glowMaterial, new Vector3(0.12f, 0.7f, 0.12f), new Vector3(0f, 0f, -0.7f));
        trail.localRotation = Quaternion.Euler(90f, 0f, 0f);
        ringA = CreateChildPrimitive("RingA", PrimitiveType.Cylinder, coreMaterial, new Vector3(0.42f, 0.02f, 0.42f), new Vector3(0f, 0f, 0f));
        ringA.localRotation = Quaternion.Euler(90f, 0f, 0f);
        ringB = CreateChildPrimitive("RingB", PrimitiveType.Cylinder, glowMaterial, new Vector3(0.28f, 0.015f, 0.28f), new Vector3(0f, 0f, 0f));
        ringB.localRotation = Quaternion.Euler(0f, 0f, 90f);

        Light light = gameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 11f;
        light.intensity = 4.8f;
        light.color = new Color(beamColor.r, beamColor.g, beamColor.b, 1f);
    }

    private void UpdateVisuals()
    {
        if (trail != null)
        {
            trail.localScale = new Vector3(0.14f, 0.9f + Mathf.PingPong(Time.time * 2f, 0.3f), 0.14f);
        }

        if (core != null)
        {
            float pulse = 0.58f + Mathf.PingPong(Time.time * 1.6f, 0.12f);
            core.localScale = new Vector3(pulse, pulse, pulse);
        }

        if (ringA != null)
        {
            ringA.Rotate(0f, 0f, 480f * Time.deltaTime, Space.Self);
        }

        if (ringB != null)
        {
            ringB.Rotate(480f * Time.deltaTime, 0f, 0f, Space.Self);
        }
    }

    private Transform CreateChildPrimitive(string pieceName, PrimitiveType type, Material material, Vector3 localScale, Vector3 localPosition)
    {
        GameObject piece = GameObject.CreatePrimitive(type);
        piece.name = pieceName;
        piece.transform.SetParent(transform, false);
        piece.transform.localPosition = localPosition;
        piece.transform.localScale = localScale;
        piece.GetComponent<Renderer>().sharedMaterial = material;
        Destroy(piece.GetComponent<Collider>());
        return piece.transform;
    }
}
