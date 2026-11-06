using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Keyboard helper that works with the old Input Manager and the new Input System.
public static class KeyInput
{
    public static bool Up
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.upArrowKey.isPressed;
#else
            return Input.GetKey(KeyCode.UpArrow);
#endif
        }
    }

    public static bool Down
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.downArrowKey.isPressed;
#else
            return Input.GetKey(KeyCode.DownArrow);
#endif
        }
    }

    public static bool Left
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.leftArrowKey.isPressed;
#else
            return Input.GetKey(KeyCode.LeftArrow);
#endif
        }
    }

    public static bool Right
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.rightArrowKey.isPressed;
#else
            return Input.GetKey(KeyCode.RightArrow);
#endif
        }
    }

    public static bool JumpPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.spaceKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Space);
#endif
        }
    }

    public static bool RestartPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.rKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.R);
#endif
        }
    }

    public static bool MenuPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.escapeKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }
    }

    public static bool ConfirmPressed
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current; return k != null && k.enterKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Return);
#endif
        }
    }
}

[RequireComponent(typeof(CharacterController))]
public class DinoController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float backSpeed = 4f;
    public float turnSpeed = 110f;
    public float gravity = -25f;
    public float jumpHeight = 2.0f;
    public float coyoteTime = 0.12f;
    public float jumpBuffer = 0.15f;

    public float HorizontalSpeed { get; private set; }
    public bool Grounded { get; private set; }

    CharacterController cc;
    float verticalVelocity;
    float coyoteTimer;
    float jumpBufferTimer;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Left / Right arrows turn the dinosaur.
        float turn = (KeyInput.Right ? 1f : 0f) - (KeyInput.Left ? 1f : 0f);
        transform.Rotate(0f, turn * turnSpeed * dt, 0f);

        // Up / Down arrows walk forwards / backwards.
        float forward = (KeyInput.Up ? 1f : 0f) - (KeyInput.Down ? 1f : 0f);
        float speed = forward > 0f ? moveSpeed : backSpeed;
        Vector3 horizontal = transform.forward * forward * speed;
        HorizontalSpeed = Mathf.Abs(forward) * speed;

        // Jumping with coyote time and a small input buffer.
        if (Grounded) coyoteTimer = coyoteTime; else coyoteTimer -= dt;
        if (KeyInput.JumpPressed) jumpBufferTimer = jumpBuffer; else jumpBufferTimer -= dt;

        if (Grounded && verticalVelocity < 0f) verticalVelocity = -2f;

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        verticalVelocity += gravity * dt;

        CollisionFlags flags = cc.Move((horizontal + Vector3.up * verticalVelocity) * dt);
        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f) verticalVelocity = 0f;
        Grounded = cc.isGrounded;
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        cc.enabled = false;
        transform.position = position;
        transform.rotation = rotation;
        verticalVelocity = 0f;
        Grounded = false;
        cc.enabled = true;
    }
}
