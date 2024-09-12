using UnityEngine;

public class PlayerHandler : MonoBehaviour, IHealthAdjustable
{

    [SerializeField] private PlayerHealth playerHealth;

    public void TakeDamage(float damage)
    {
        playerHealth.Damage(damage);
    }

    public void TakeHealing(float healAmount)
    {
        playerHealth.Heal(healAmount);
    }


}
