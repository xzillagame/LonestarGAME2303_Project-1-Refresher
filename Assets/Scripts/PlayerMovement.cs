using System;
using System.Collections;
using TreeEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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

    private Vector3 rotationVectorFromInput = Vector2.zero;
    private void RotateTowardsInputDirection()
    {
        if(PlayerMovementInput.magnitude == 0f) { return; }

        rotationVectorFromInput.x = PlayerMovementInput.x;
        rotationVectorFromInput.z = PlayerMovementInput.y;

        Quaternion lookDirection = Quaternion.LookRotation(rotationVectorFromInput.normalized, Vector3.up);
        playerRigidbody.rotation = Quaternion.Lerp(playerRigidbody.rotation, lookDirection, rotationSpeed);
    }


    [SerializeField] private float homingSphereRadius;
    [SerializeField] private LayerMask attackLayer;
    Coroutine homingAttackCoroutine;
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
            playerRigidbody.AddForce(Vector3.up * unitJumpForce, ForceMode.Impulse);
        }
        else if (ctx.action.WasReleasedThisFrame())
        {
            float currentYVelocity = playerRigidbody.velocity.y;

            if (currentYVelocity > 0f)
            {
                Vector3 currentVelocity = playerRigidbody.velocity;
                currentVelocity.y = currentYVelocity / 2f;

                playerRigidbody.velocity = currentVelocity;

            }

        }

    }

    [SerializeField] private float homingSpeed;
    private IEnumerator HomingAttack(Transform target)
    {
        this.enabled = false;
        playerRigidbody.velocity = Vector3.zero;

        float totalDuration = 2f;
        float counter = 0f;

        while(this.transform != target.transform && counter < totalDuration)
        {
            counter += Time.deltaTime;

            playerRigidbody.transform.position = 
                Vector3.Lerp(playerRigidbody.transform.position, target.position,
                                homingSpeed * Time.deltaTime);

            yield return null;
        }

        this.enabled = true;

    }

    //[SerializeField] private float pushValue = 10f;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("C: " + Convert.ToString(1 << collision.gameObject.layer, 2).PadLeft(32, '0'));
        //Debug.Log("A: " + Convert.ToString(attackLayer, 2).PadLeft(32, '0'));

        //Debug.Log("T: " + Convert.ToString(1 << collision.gameObject.layer & attackLayer, 2).PadLeft(32, '0'));

        //Debug.Log( ( (1 << collision.gameObject.layer) & attackLayer) == attackLayer);

        if (((1 << other.gameObject.layer) & attackLayer) != 0)
        {
            //Destroy(other.gameObject);
            //playerRigidbody.velocity  = Vector3.up * pushValue;
            if (homingAttackCoroutine != null) StopCoroutine(homingAttackCoroutine);
            //StartCoroutine(DisableControl());
            //this.enabled = true;
        }
    }

    public void DisableInputAndMove(Vector3 direction, float force, float waitTime)
    {
        this.enabled = false;
        playerInput.Movement.Disable();
        this.waitTime = waitTime;
        StartCoroutine(DisableControl());
        playerRigidbody.velocity = direction.normalized * force;
    }



    private float waitTime;
    private IEnumerator DisableControl()
    {
        //this.enabled = false;
        //playerInput.Movement.Disable();

        yield return new WaitForSeconds(waitTime);

        playerInput.Movement.Enable();
        this.enabled = true;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (transform.forward * homingSphereRadius), homingSphereRadius);
    }

}
