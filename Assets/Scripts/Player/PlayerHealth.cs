using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Player Health", menuName = "SO/Health Object")]
public class PlayerHealth : ScriptableObject
{
    public event UnityAction OnPlayerDeath;
    public event UnityAction OnPlayerHealthUpdated;


    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;


    public float CurrentHealth
    {
        get { return currentHealth; }
        private set 
        { 
            currentHealth = value;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnPlayerHealthUpdated?.Invoke();

            if ( currentHealth <= 0 ) OnPlayerDeath?.Invoke();

        }
    }

    public float MaxHealth { get { return maxHealth; } }


    public void Damage(float damage)
    {
        CurrentHealth -= damage;
    }

    public void Heal(float healAmount)
    {
        CurrentHealth += healAmount;
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void OnDisable()
    {
        OnPlayerDeath = null;
    }

}
