using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float unitSpeed, unitJumpForce;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private float customGravityValue;

    [SerializeField, Range(0f,1f)] private float rotationSpeed = 0.25f;

    [SerializeField] private float groundRayCheckDistance;

    private PlayerInputMap playerInput;
    public Vector2 PlayerMovementInput { get; private set; }

    private bool isGrounded = true;

    private void Awake()
    {
        playerInput = new PlayerInputMap();
        playerInput.Movement.Jump.performed += OnPerformJump;
    }

    private void OnDestroy()
    {
        playerInput.Movement.Jump.performed -= OnPerformJump;
    }

    private void Start()
    {
        playerInput.Enable();
    }

    private void Update()
    {
        PlayerMovementInput = playerInput.Movement.Move.ReadValue<Vector2>();

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundRayCheckDistance);
    }

    private void FixedUpdate()
    {
        MoveTowardsInputDirection();
        RotateTowardsInputDirection();
    }

    private void MoveTowardsInputDirection()
    {
        float prevYVelocity = playerRigidbody.velocity.y;

        Vector3 newVelocity = new Vector3(PlayerMovementInput.x, 0f, PlayerMovementInput.y) * unitSpeed;
        newVelocity.y = prevYVelocity;

        newVelocity.y = isGrounded ? 0f : (newVelocity.y - (customGravityValue * Time.fixedDeltaTime));

        playerRigidbody.velocity = newVelocity;
    }

    private void RotateTowardsInputDirection()
    {
        if(PlayerMovementInput.magnitude == 0f) { return; }

        Quaternion lookDirection = Quaternion.LookRotation(new Vector3(PlayerMovementInput.x,0f,PlayerMovementInput.y), Vector3.up);
        playerRigidbody.rotation = Quaternion.Lerp(playerRigidbody.rotation, lookDirection, rotationSpeed);
    }


    private void OnPerformJump(InputAction.CallbackContext ctx)
    {

        if(ctx.action.WasPressedThisFrame() && isGrounded)
        {
            playerRigidbody.AddForce(Vector3.up * unitJumpForce, ForceMode.Impulse);
        }
        else if(ctx.action.WasReleasedThisFrame()) 
        {
            float currentYVelocity = playerRigidbody.velocity.y;

            if(currentYVelocity > 0f) 
            {
                Vector3 currentVelocity = playerRigidbody.velocity;
                currentVelocity.y = currentYVelocity / 2f;

                playerRigidbody.velocity = currentVelocity;

            }

        }


    }

}
