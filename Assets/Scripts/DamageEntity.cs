using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEntity : MonoBehaviour
{
    [SerializeField] private float damageValue;
    [SerializeField] private float tickRate;

    IEnumerator damageRoutine;

    private Dictionary<int, IHealthAdjustable> trackedDamagedUnits = new Dictionary<int, IHealthAdjustable>();


    public void AddDamagableUnit(IHealthAdjustable unitHealthAdjustable, GameObject unitToGetID)
    {
        int objectInstance = unitToGetID.GetInstanceID();

        if (!trackedDamagedUnits.ContainsKey(objectInstance)) trackedDamagedUnits.Add(objectInstance, unitHealthAdjustable);
    }


    public void RemoveDamageableUnit(IHealthAdjustable unitHealthAdjustable, GameObject unitToGetID)
    {
        int instanceID = unitToGetID.GetInstanceID();

        if( trackedDamagedUnits.ContainsKey(instanceID) ) trackedDamagedUnits.Remove(instanceID);
    }


    public void StartTickDamage()
    {
        StartCoroutine(damageRoutine);
    }

    public void StopTickDamage()
    {
        StopCoroutine(damageRoutine);
    }

    private void Start()
    {
        damageRoutine = PerformDamage();
    }

    private IEnumerator PerformDamage()
    {
        WaitForSeconds tickRateTimer = new WaitForSeconds(tickRate);

        while(true)
        {

            foreach (var unit in trackedDamagedUnits)
            {
                unit.Value.TakeDamage(damageValue);
            }

            yield return tickRateTimer;

        }

    }


}
