using UnityEngine;
using UnityEngine.Events;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] private UnityEvent OnPlayerEnter;
    [SerializeField] private UnityEvent OnPlayerExit;


    private void OnTriggerEnter(Collider other) => OnPlayerEnter.Invoke();

    private void OnTriggerExit(Collider other) => OnPlayerExit.Invoke();


}
