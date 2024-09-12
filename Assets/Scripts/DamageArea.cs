using UnityEngine;
using UnityEngine.Events;

public class DamageArea : MonoBehaviour
{
    public UnityEvent<IHealthAdjustable, GameObject> OnEntityEnterDamage;
    public UnityEvent<IHealthAdjustable, GameObject> OnEntityExitDamage;


    private void OnTriggerEnter(Collider other)
    {
        IHealthAdjustable damagedUnit = other.GetComponent<IHealthAdjustable>();

        OnEntityEnterDamage.Invoke(damagedUnit,other.gameObject);
    }


    private void OnTriggerExit(Collider other)
    {
        IHealthAdjustable damagedUnit = GetComponent<IHealthAdjustable>();

        OnEntityExitDamage.Invoke(damagedUnit, other.gameObject);
    }



}
