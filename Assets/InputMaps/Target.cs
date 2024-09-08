using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float pushValue = 10f;
    [SerializeField] private float waitTime = 1f;

    [SerializeField] bool destoryOnTrigger;

    private void OnTriggerEnter(Collider other)
    {

        //Rigidbody body = other.GetComponent<Rigidbody>();
        //body?.AddForce(Vector3.up * pushValue, ForceMode.Impulse);
        //if(body != null) body.velocity = Vector3.up * pushValue;

        PlayerMovement player = other.GetComponent<PlayerMovement>();
        player?.DisableInputAndMove(Vector3.up, pushValue, waitTime);


        if (destoryOnTrigger) Destroy(gameObject);
    }
}
