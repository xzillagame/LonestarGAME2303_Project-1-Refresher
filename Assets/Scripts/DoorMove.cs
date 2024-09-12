using System.Collections;
using UnityEngine;

public class DoorMove : MonoBehaviour
{

    [SerializeField] private Vector3 LocalMoveToLocation;
    [SerializeField] private MeshFilter meshFilter;
    
    private Vector3 orginalPosition;

    IEnumerator routineCall;
   
    public void OpenDoor()
    {
        if (routineCall != null) StopCoroutine(routineCall);

        routineCall = MoveDoor(orginalPosition + LocalMoveToLocation);

        StartCoroutine(routineCall);
        
    }

    public void CloseDoor()
    {
        if (routineCall != null) StopCoroutine(routineCall);

        routineCall = MoveDoor(orginalPosition);

        StartCoroutine(routineCall);
    }


    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        orginalPosition = transform.position;
    }

    [SerializeField] private float MoveSpeed;
    private IEnumerator MoveDoor(Vector3 position)
    {
        while (transform.position.magnitude != position.magnitude)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, MoveSpeed * Time.deltaTime);

            yield return null;
        }
    }


#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireMesh(meshFilter.sharedMesh, transform.position + LocalMoveToLocation, this.transform.rotation, this.transform.localScale);

    }

#endif



}
