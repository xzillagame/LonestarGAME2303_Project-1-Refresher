using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 PlayerMovementInput { get; private set; }

    [field:SerializeField] public Rigidbody PlayerRigidbody { get; private set; }
    
    [SerializeField] private float unitSpeed, unitJumpForce;
    [SerializeField] private float customGravityValue;

    [SerializeField, Range(0f,1f)] private float rotationSpeed = 0.25f;

    [SerializeField] private float groundRayCheckDistance;
    
    private bool isGrounded = true;

    private PlayerInputMap playerInput;

    private Vector3 rotationVectorFromInput = Vector2.zero;
    private float waitTime;

    #region Homing Attack
    [Header("Homing Attack")]
    [SerializeField] private float homingSpeed;
    [SerializeField] private float homingSphereRadius;
    [SerializeField] private LayerMask attackLayer;
    Coroutine homingAttackCoroutine;
    #endregion

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
        float prevYVelocity = PlayerRigidbody.velocity.y;

        Vector3 newVelocity = new Vector3(PlayerMovementInput.x, 0f, PlayerMovementInput.y) * unitSpeed;
        newVelocity.y = prevYVelocity;

        newVelocity.y = isGrounded ? 0f : (newVelocity.y - (customGravityValue * Time.fixedDeltaTime));

        PlayerRigidbody.velocity = newVelocity;
        
    }

    private void RotateTowardsInputDirection()
    {
        if(PlayerMovementInput.magnitude == 0f) { return; }

        rotationVectorFromInput.x = PlayerMovementInput.x;
        rotationVectorFromInput.z = PlayerMovementInput.y;

        Quaternion lookDirection = Quaternion.LookRotation(rotationVectorFromInput.normalized, Vector3.up);
        PlayerRigidbody.rotation = Quaternion.Lerp(PlayerRigidbody.rotation, lookDirection, rotationSpeed);
    }


    private void OnPerformJump(InputAction.CallbackContext ctx)
    {

        if(ctx.action.WasPressedThisFrame() && !isGrounded)
        {
            Gizmos.color = Color.red;
            Collider[] list = Physics.OverlapSphere(transform.position + (transform.forward * homingSphereRadius),
                                homingSphereRadius, attackLayer, QueryTriggerInteraction.Collide);
            


            if (list.Length > 0) homingAttackCoroutine = StartCoroutine(HomingAttack(list[0].transform));
        }



        if (ctx.action.WasPressedThisFrame() && isGrounded)
        {
            PlayerRigidbody.AddForce(Vector3.up * unitJumpForce, ForceMode.Impulse);
        }
        else if (ctx.action.WasReleasedThisFrame())
        {
            float currentYVelocity = PlayerRigidbody.velocity.y;

            if (currentYVelocity > 0f)
            {
                Vector3 currentVelocity = PlayerRigidbody.velocity;
                currentVelocity.y = currentYVelocity / 2f;

                PlayerRigidbody.velocity = currentVelocity;

            }

        }

    }

    private IEnumerator HomingAttack(Transform target)
    {
        this.enabled = false;
        PlayerRigidbody.velocity = Vector3.zero;

        float totalDuration = 2f;
        float counter = 0f;

        while(this.transform != target.transform && counter < totalDuration)
        {
            counter += Time.deltaTime;

            PlayerRigidbody.transform.position = 
                Vector3.Lerp(PlayerRigidbody.transform.position, target.position,
                                homingSpeed * Time.deltaTime);

            yield return null;
        }

        this.enabled = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("C: " + Convert.ToString(1 << collision.gameObject.layer, 2).PadLeft(32, '0'));
        //Debug.Log("A: " + Convert.ToString(attackLayer, 2).PadLeft(32, '0'));

        //Debug.Log("T: " + Convert.ToString(1 << collision.gameObject.layer & attackLayer, 2).PadLeft(32, '0'));

        //Debug.Log( ( (1 << collision.gameObject.layer) & attackLayer) == attackLayer);

        if (((1 << other.gameObject.layer) & attackLayer) != 0)
        {
            if (homingAttackCoroutine != null) StopCoroutine(homingAttackCoroutine);
        }
    }

    public void DisableInputAndMove(Vector3 direction, float force, float waitTime)
    {
        this.enabled = false;
        playerInput.Movement.Disable();
        this.waitTime = waitTime;
        StartCoroutine(DisableControl());
        PlayerRigidbody.velocity = direction.normalized * force;
    }

    private IEnumerator DisableControl()
    {
        this.enabled = false;
        playerInput.Movement.Disable();

        yield return new WaitForSeconds(waitTime);

        playerInput.Movement.Enable();
        this.enabled = true;
    }


#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * homingSphereRadius), homingSphereRadius);
    }

#endif

}
