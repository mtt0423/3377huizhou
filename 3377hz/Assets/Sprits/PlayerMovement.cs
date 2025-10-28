using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private player inputActions;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new player();
        inputActions.Player.Enable();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        rb.velocity = moveInput * moveSpeed;

        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);

        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("lastHorizontal", moveInput.x);
            animator.SetFloat("lastVertical", moveInput.y);
        }
    }
}