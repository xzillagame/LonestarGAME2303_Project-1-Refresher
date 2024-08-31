using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private string speedAnimationParamaterName = "UnitSpeed";


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerAnimator.SetFloat(speedAnimationParamaterName, playerMovement.PlayerMovementInput.magnitude);
    }
}
