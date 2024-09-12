using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image healthImage;


    private void UpdatePlayerHealthUI()
    {
        healthImage.fillAmount = playerHealth.CurrentHealth / playerHealth.MaxHealth;
    }

    private void OnEnable()
    {
        playerHealth.OnPlayerHealthUpdated += UpdatePlayerHealthUI;
    }

    private void OnDisable()
    {
        playerHealth.OnPlayerHealthUpdated -= UpdatePlayerHealthUI;
    }

}
