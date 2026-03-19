using System;
using System.Collections;
using UnityEngine;

public class FighterGameplay : MonoBehaviour
{
    private static readonly int VSpeed = Animator.StringToHash("VSpeed");
    private static readonly int HSpeed = Animator.StringToHash("HSpeed");
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Punching = Animator.StringToHash("Punching");
    private static readonly int Kicking = Animator.StringToHash("Kicking");
    private static readonly int CurrentAction = Animator.StringToHash("CurrentAction");

    [SerializeField] private string fighterName = "Champion";
    [SerializeField] private bool firstPlayer = true;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveAcceleration = 14f;
    [SerializeField] private float airControl = 0.62f;
    [SerializeField] private float jumpHeight = 2.9f;
    [SerializeField] private float gravity = -28f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float arenaHalfWidth = 18.5f;
    [SerializeField] private float groundY = 0.5f;
    [SerializeField] private float comboResetTime = 0.6f;
    [SerializeField] private float ultraCooldown = 5f;

    private CharacterController characterController;
    private Animator animator;
    private FighterGameplay opponent;
    private Vector3 velocity;
    private float currentHealth;
    private float horizontalVelocity;
    private float attackLockTimer;
    private float hitStunTimer;
    private float comboTimer;
    private float ultraCooldownTimer;
    private int comboStep;
    private bool acceptingInput = true;
    private bool roundActive;
    private bool hasLegacyAnimator;
    private VisualState currentVisualState;
    private float visualStateTimer;
    private float currentMoveInput;
    private bool isGroundedState;

    public event Action<FighterGameplay> HealthChanged;
    public event Action<FighterGameplay> Defeated;

    public string FighterName => fighterName;
    public float HealthNormalized => maxHealth <= 0f ? 0f : currentHealth / maxHealth;
    public float CurrentHealth => currentHealth;
    public float UltraCooldownNormalized => ultraCooldown <= 0f ? 0f : Mathf.Clamp01(ultraCooldownTimer / ultraCooldown);
    public bool IsDefeated => currentHealth <= 0f;
    public bool AcceptingInput => acceptingInput && roundActive && !IsDefeated;
    public float MoveInput => currentMoveInput;
    public float HorizontalSpeedNormalized => Mathf.Clamp01(Mathf.Abs(horizontalVelocity) / Mathf.Max(0.01f, moveSpeed));
    public bool IsGroundedState => isGroundedState;
    public bool IsInHitStun => hitStunTimer > 0f;
    public VisualState CurrentVisualState => currentVisualState;

    public void Initialize(string displayName, bool isFirstPlayer)
    {
        fighterName = displayName;
        firstPlayer = isFirstPlayer;
    }

    public void SetOpponent(FighterGameplay other)
    {
        opponent = other;
    }

    public void SetRoundActive(bool value)
    {
        roundActive = value;
        acceptingInput = value && !IsDefeated;

        if (!value)
        {
            horizontalVelocity = 0f;
            attackLockTimer = 0f;
            if (hasLegacyAnimator)
            {
                animator.SetBool(Punching, false);
                animator.SetBool(Kicking, false);
                animator.SetInteger(CurrentAction, 0);
            }
        }
    }

    public void ResetForRound(Vector3 spawnPosition)
    {
        StopAllCoroutines();
        currentHealth = maxHealth;
        attackLockTimer = 0f;
        hitStunTimer = 0f;
        comboTimer = 0f;
        comboStep = 0;
        ultraCooldownTimer = 0f;
        horizontalVelocity = 0f;
        velocity = Vector3.zero;
        currentMoveInput = 0f;
        currentVisualState = VisualState.Idle;
        visualStateTimer = 0f;
        roundActive = false;
        acceptingInput = false;

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = new Vector3(spawnPosition.x, groundY, 0f);
        transform.rotation = Quaternion.identity;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        if (hasLegacyAnimator)
        {
            animator.SetFloat(VSpeed, 0f);
            animator.SetFloat(HSpeed, 0f);
            animator.SetBool(Jumping, false);
            animator.SetBool(Punching, false);
            animator.SetBool(Kicking, false);
            animator.SetInteger(CurrentAction, 0);
            animator.Rebind();
            animator.Update(0f);
        }

        HealthChanged?.Invoke(this);
    }

    public void TakeDamage(float damage, Vector3 knockback)
    {
        if (IsDefeated)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        hitStunTimer = currentHealth <= 0f ? 0.8f : 0.28f;
        attackLockTimer = 0f;
        comboTimer = 0f;
        comboStep = 0;
        horizontalVelocity = knockback.x;
        velocity.y = Mathf.Max(velocity.y, currentHealth <= 0f ? 4.8f : 3.6f);
        SetVisualState(currentHealth <= 0f ? VisualState.Defeated : VisualState.Hit, currentHealth <= 0f ? 1f : 0.28f);
        HealthChanged?.Invoke(this);

        StartCoroutine(CombatEffects.HitPause(currentHealth <= 0f ? 0.09f : 0.055f));
        ArenaCameraRig.TriggerShake(currentHealth <= 0f ? 0.18f : 0.09f, currentHealth <= 0f ? 0.24f : 0.14f);
        CombatEffects.SpawnImpact(
            transform.position + Vector3.up * 1.15f,
            firstPlayer ? new Color(1f, 0.42f, 0.22f) : new Color(0.52f, 0.84f, 1f),
            currentHealth <= 0f ? 1.35f : 1f);

        if (currentHealth <= 0f)
        {
            acceptingInput = false;
            roundActive = false;
            if (hasLegacyAnimator)
            {
                animator.SetInteger(CurrentAction, 4);
                animator.SetBool(Punching, false);
                animator.SetBool(Kicking, false);
            }
            Defeated?.Invoke(this);
            return;
        }

        if (hasLegacyAnimator)
        {
            animator.SetInteger(CurrentAction, 2);
        }
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        hasLegacyAnimator = animator != null && animator.runtimeAnimatorController != null && animator.enabled;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (characterController == null)
        {
            return;
        }

        FaceOpponent();
        TickTimers();
        HandleStates();
    }

    private void TickTimers()
    {
        if (ultraCooldownTimer > 0f)
        {
            ultraCooldownTimer = Mathf.Max(0f, ultraCooldownTimer - Time.deltaTime);
        }

        if (visualStateTimer > 0f)
        {
            visualStateTimer -= Time.deltaTime;
            if (visualStateTimer <= 0f && !IsDefeated)
            {
                currentVisualState = VisualState.Idle;
            }
        }

        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                comboStep = 0;
            }
        }
    }

    private void HandleStates()
    {
        if (hitStunTimer > 0f)
        {
            hitStunTimer -= Time.deltaTime;
            if (hitStunTimer <= 0f)
            {
                if (hasLegacyAnimator)
                {
                    animator.SetInteger(CurrentAction, 0);
                }
            }

            UpdateMovement(0f, false);
            return;
        }

        if (!roundActive)
        {
            UpdateMovement(0f, true);
            if (hasLegacyAnimator)
            {
                animator.SetFloat(VSpeed, 0f);
                animator.SetFloat(HSpeed, 0f);
            }
            return;
        }

        float movementInput = AcceptingInput
            ? (firstPlayer ? GetAxisOrKeys("Horizontal", KeyCode.D, KeyCode.A) : GetAxisOrKeys("Horizontal2", KeyCode.L, KeyCode.J))
            : 0f;
        currentMoveInput = movementInput;

        bool grounded = characterController.isGrounded;
        isGroundedState = grounded;
        UpdateMovement(movementInput, grounded);
        if (hasLegacyAnimator)
        {
            animator.SetFloat(VSpeed, Mathf.Abs(horizontalVelocity) / Mathf.Max(0.01f, moveSpeed));
            animator.SetFloat(HSpeed, velocity.y);
            animator.SetBool(Jumping, !grounded);
        }

        if (!AcceptingInput)
        {
            return;
        }

        if (grounded && GetJumpPressed())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            SetVisualState(VisualState.Jump, 0.32f);
            CombatEffects.SpawnDustBurst(transform.position + Vector3.down * 0.3f);
        }

        if (attackLockTimer > 0f)
        {
            attackLockTimer -= Time.deltaTime;
            return;
        }

        if (GetPunchPressed())
        {
            StartCoroutine(PerformMeleeAttack(AttackType.Punch));
        }
        else if (GetKickPressed())
        {
            StartCoroutine(PerformMeleeAttack(AttackType.Kick));
        }
        else if (GetUltraPressed() && ultraCooldownTimer <= 0f)
        {
            StartCoroutine(PerformUltraAttack());
        }
    }

    private IEnumerator PerformMeleeAttack(AttackType type)
    {
        comboStep = Mathf.Clamp(comboStep + 1, 1, 3);
        comboTimer = comboResetTime;

        float windup = type == AttackType.Punch ? 0.08f : 0.12f;
        float recovery = type == AttackType.Punch ? 0.17f : 0.24f;
        float range = type == AttackType.Punch ? 2.2f : 2.7f;
        float damage = type == AttackType.Punch ? 8f + comboStep * 1.5f : 13f + comboStep * 2f;
        float knockbackForce = type == AttackType.Punch ? 4.4f + comboStep * 0.45f : 6.7f + comboStep * 0.6f;

        attackLockTimer = windup + recovery;
        SetVisualState(type == AttackType.Punch ? VisualState.Punch : VisualState.Kick, windup + recovery);
        if (hasLegacyAnimator)
        {
            animator.SetBool(Punching, type == AttackType.Punch);
            animator.SetBool(Kicking, type == AttackType.Kick);
            animator.SetInteger(CurrentAction, type == AttackType.Punch ? 1 : 2);
        }

        yield return new WaitForSeconds(windup);

        Vector3 knockback = new Vector3(Mathf.Sign(opponent.transform.position.x - transform.position.x) * knockbackForce, 0f, 0f);
        Vector3 hitPosition = transform.position + transform.forward * (range * 0.55f) + Vector3.up * 1.15f;
        CombatEffects.SpawnStrikeArc(hitPosition, type == AttackType.Punch ? new Color(0.96f, 0.9f, 0.75f, 0.82f) : new Color(1f, 0.65f, 0.38f, 0.82f), transform.forward);
        TryDamageOpponent(range, damage, knockback);

        yield return new WaitForSeconds(recovery);
        if (hasLegacyAnimator)
        {
            animator.SetBool(Punching, false);
            animator.SetBool(Kicking, false);
            animator.SetInteger(CurrentAction, 0);
        }
    }

    private IEnumerator PerformUltraAttack()
    {
        ultraCooldownTimer = ultraCooldown;
        attackLockTimer = 1f;
        comboStep = 0;
        comboTimer = 0f;
        SetVisualState(VisualState.Ultra, 0.8f);
        if (hasLegacyAnimator)
        {
            animator.SetInteger(CurrentAction, 3);
        }
        yield return new WaitForSeconds(0.18f);
        SpawnUltraProjectile();
        ArenaCameraRig.TriggerShake(0.12f, 0.2f);
        CombatEffects.SpawnImpact(transform.position + transform.forward * 0.9f + Vector3.up * 1.1f, new Color(0.95f, 0.88f, 0.45f), 1.3f);
        yield return new WaitForSeconds(0.55f);
        if (hasLegacyAnimator)
        {
            animator.SetInteger(CurrentAction, 0);
        }
    }

    private void SpawnUltraProjectile()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = fighterName + "_Ultra";
        projectile.transform.position = transform.position + transform.forward * 1.4f + Vector3.up * 1.3f;
        projectile.transform.localScale = Vector3.one * 0.95f;
        projectile.GetComponent<Renderer>().sharedMaterial = ProceduralVisualFactory.GetTransparentMaterial(
            fighterName + "_UltraMat",
            firstPlayer ? new Color(0.55f, 0.82f, 1f, 0.92f) : new Color(1f, 0.42f, 0.25f, 0.92f));
        UnityEngine.Object.Destroy(projectile.GetComponent<Collider>());

        Light light = projectile.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 12f;
        light.intensity = 5f;
        light.color = firstPlayer ? new Color(0.55f, 0.82f, 1f) : new Color(1f, 0.42f, 0.25f);

        UltraProjectile mover = projectile.AddComponent<UltraProjectile>();
        mover.Initialize(this, opponent, 18f, 22f, 2.8f);
    }

    private void TryDamageOpponent(float range, float damage, Vector3 knockback)
    {
        if (opponent == null || opponent.IsDefeated)
        {
            return;
        }

        Vector3 toOpponent = opponent.transform.position - transform.position;
        float forwardDot = Vector3.Dot(transform.forward, toOpponent.normalized);
        if (Mathf.Abs(toOpponent.z) < 2f && toOpponent.magnitude <= range && forwardDot > 0.55f)
        {
            opponent.TakeDamage(damage, knockback);
        }
    }

    private void UpdateMovement(float movementInput, bool grounded)
    {
        float targetSpeed = movementInput * moveSpeed;
        float acceleration = grounded ? moveAcceleration : moveAcceleration * airControl;
        horizontalVelocity = Mathf.MoveTowards(horizontalVelocity, targetSpeed, acceleration * Time.deltaTime);

        if (grounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 totalMove = new Vector3(horizontalVelocity, velocity.y, 0f) * Time.deltaTime;
        characterController.Move(totalMove);

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -arenaHalfWidth, arenaHalfWidth);
        position.z = 0f;
        if (position.y < groundY)
        {
            position.y = groundY;
        }

        transform.position = position;
    }

    private void FaceOpponent()
    {
        if (opponent == null)
        {
            return;
        }

        float deltaX = opponent.transform.position.x - transform.position.x;
        if (Mathf.Abs(deltaX) < 0.01f)
        {
            return;
        }

        float targetY = deltaX >= 0f ? 90f : -90f;
        float currentY = transform.eulerAngles.y;
        float nextY = Mathf.MoveTowardsAngle(currentY, targetY, 720f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, nextY, 0f);
    }

    private void SetVisualState(VisualState state, float duration)
    {
        currentVisualState = state;
        visualStateTimer = duration;
    }

    private bool GetJumpPressed()
    {
        return firstPlayer ? GetButtonDownOrKey("Jump", KeyCode.Space) : GetButtonDownOrKey("Jump2", KeyCode.O);
    }

    private bool GetPunchPressed()
    {
        return firstPlayer ? GetButtonDownOrKey("Fire1", KeyCode.Z) : GetButtonDownOrKey("Fire1p", KeyCode.U);
    }

    private bool GetKickPressed()
    {
        return firstPlayer ? GetButtonDownOrKey("Fire2", KeyCode.X) : GetButtonDownOrKey("Fire2p", KeyCode.P);
    }

    private bool GetUltraPressed()
    {
        return firstPlayer ? Input.GetKeyDown(KeyCode.C) : Input.GetKeyDown(KeyCode.M);
    }

    private static float GetAxisOrKeys(string axisName, KeyCode positive, KeyCode negative)
    {
        float keyValue = 0f;
        if (Input.GetKey(positive))
        {
            keyValue += 1f;
        }

        if (Input.GetKey(negative))
        {
            keyValue -= 1f;
        }

        if (Mathf.Abs(keyValue) > 0.001f)
        {
            return keyValue;
        }

        try
        {
            return Input.GetAxisRaw(axisName);
        }
        catch
        {
            return 0f;
        }
    }

    private static bool GetButtonDownOrKey(string buttonName, KeyCode fallbackKey)
    {
        if (Input.GetKeyDown(fallbackKey))
        {
            return true;
        }

        try
        {
            return Input.GetButtonDown(buttonName);
        }
        catch
        {
            return Input.GetKeyDown(fallbackKey);
        }
    }

    private enum AttackType
    {
        Punch,
        Kick
    }

    public enum VisualState
    {
        Idle,
        Jump,
        Punch,
        Kick,
        Ultra,
        Hit,
        Defeated
    }
}
