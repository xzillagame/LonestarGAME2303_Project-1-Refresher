using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private Animator playerAnimator;

    [SerializeField] private string speedAnimationParamaterName = "UnitSpeed";
    [SerializeField] private string horizontalVelocityParamName = "HorizontalVelocity";

    void Update()
    {
        playerAnimator.SetFloat(speedAnimationParamaterName, playerMovement.PlayerMovementInput.magnitude);
        playerAnimator.SetFloat(horizontalVelocityParamName, playerMovement.PlayerRigidbody.velocity.y);
    }
}
