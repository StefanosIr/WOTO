using UnityEngine;

public class Controls2 : MonoBehaviour
{
    private static readonly int VSpeed = Animator.StringToHash("VSpeed");
    private static readonly int HSpeed = Animator.StringToHash("HSpeed");
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Punching = Animator.StringToHash("Punching");
    private static readonly int Kicking = Animator.StringToHash("Kicking");
    private static readonly int TurningLeft = Animator.StringToHash("TurningLeft");
    private static readonly int TurningRight = Animator.StringToHash("TurningRight");
    private static readonly int CurrentAction = Animator.StringToHash("CurrentAction");

    private Animator myAnimator;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 100f;
    private CharacterController characterController;

    private void Start()
    {
        myAnimator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
        if (myAnimator == null)
        {
            Debug.LogError("Controls2 requires an Animator component.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        float vertical = GetAxisOrKeys("Vertical2", KeyCode.I, KeyCode.K);
        float horizontal = GetAxisOrKeys("Horizontal2", KeyCode.L, KeyCode.J);

        myAnimator.SetFloat(VSpeed, vertical);
        myAnimator.SetFloat(HSpeed, horizontal);
        MoveCharacter(vertical, horizontal);

        if (GetButtonDownOrKey("Jump2", KeyCode.O))
        {
            myAnimator.SetBool(Jumping, true);
            Invoke(nameof(StopJumping), 0.1f);
        }

        if (GetButtonDownOrKey("Fire1p", KeyCode.U))
        {
            myAnimator.SetBool(Punching, true);
            Invoke(nameof(StopPunching), 0.5f);
        }

        if (GetButtonDownOrKey("Fire2p", KeyCode.P))
        {
            myAnimator.SetBool(Kicking, true);
            Invoke(nameof(StopKicking), 0.5f);
        }

        if (Input.GetKey(KeyCode.LeftBracket))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * turnSpeed);
            if (Mathf.Approximately(vertical, 0f) && Mathf.Approximately(horizontal, 0f))
            {
                myAnimator.SetBool(TurningLeft, true);
            }
        }
        else
        {
            myAnimator.SetBool(TurningLeft, false);
        }

        if (Input.GetKey(KeyCode.RightBracket))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * -turnSpeed);
            if (Mathf.Approximately(vertical, 0f) && Mathf.Approximately(horizontal, 0f))
            {
                myAnimator.SetBool(TurningRight, true);
            }
        }
        else
        {
            myAnimator.SetBool(TurningRight, false);
        }

        if (Input.GetKeyDown(KeyCode.Quote))
        {
            myAnimator.SetInteger(CurrentAction, myAnimator.GetInteger(CurrentAction) == 1 ? 0 : 1);
        }

        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            myAnimator.SetInteger(CurrentAction, myAnimator.GetInteger(CurrentAction) == 2 ? 0 : 2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            myAnimator.SetLayerWeight(1, 1f);
            myAnimator.SetInteger(CurrentAction, 3);
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            myAnimator.SetInteger(CurrentAction, 0);
        }
    }

    private void StopJumping()
    {
        myAnimator.SetBool(Jumping, false);
    }

    private void StopPunching()
    {
        myAnimator.SetBool(Punching, false);
    }

    private void StopKicking()
    {
        myAnimator.SetBool(Kicking, false);
    }

    private void MoveCharacter(float vertical, float horizontal)
    {
        Vector3 move = transform.forward * vertical + transform.right * horizontal;
        move = Vector3.ClampMagnitude(move, 1f) * moveSpeed;

        if (characterController != null)
        {
            characterController.Move(move * Time.deltaTime + Physics.gravity * Time.deltaTime);
            return;
        }

        transform.position += move * Time.deltaTime;
    }

    private static float GetAxisOrKeys(string axisName, KeyCode positive, KeyCode negative)
    {
        try
        {
            return Input.GetAxis(axisName);
        }
        catch
        {
            float value = 0f;
            if (Input.GetKey(positive))
            {
                value += 1f;
            }

            if (Input.GetKey(negative))
            {
                value -= 1f;
            }

            return value;
        }
    }

    private static bool GetButtonDownOrKey(string buttonName, KeyCode fallbackKey)
    {
        try
        {
            return Input.GetButtonDown(buttonName);
        }
        catch
        {
            return Input.GetKeyDown(fallbackKey);
        }
    }
}
