using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorPlatform : MonoBehaviour
{
    public GameObject worldObject;
    public GameObject rotHolder;
    public Vector3 worldUp;

    public float rotDuration = .5f;

    ObjectLock objLock;

    private void Awake()
    {
        objLock = GameObject.FindGameObjectWithTag("Player").GetComponent<ObjectLock>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            //Debug.Log("E");
            RaycastHit hit;
            if (Physics.Raycast(other.transform.position, -Vector3.up, out hit, Mathf.Infinity, LayerMask.GetMask("World")) && !objLock.rotationLock)
            {
                Debug.Log("F");
                rotHolder.transform.position = other.transform.position;
                worldObject.transform.SetParent(rotHolder.transform);
                //rotHolder.transform.rotation = Quaternion.Euler(worldUp);
                objLock.rotationLock = true;
                StartCoroutine(SlowRotate());
            }
        }
    }

    IEnumerator SlowRotate()
    {
        float elapsed = 0f;
        while (elapsed < rotDuration)
        {
            rotHolder.transform.rotation = Quaternion.Lerp(rotHolder.transform.rotation, Quaternion.Euler(worldUp), elapsed/rotDuration);
            elapsed += Time.deltaTime;

            yield return null;
        }

        rotHolder.transform.rotation = Quaternion.Euler(worldUp);
        worldObject.transform.SetParent(null);
        objLock.rotationLock = false;
        yield return null;
    }
}
