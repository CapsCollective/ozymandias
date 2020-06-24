using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public Transform[] waypointArray = new Transform[3];
    float percentsPerSecond = 0.4f; 
    float currentPathPercent = 0.0f;

    private void Start()
    {
        /*
        Vector3 curve = new Vector3();
        curve = waypointArray[0].position + waypointArray[2].position;
        GameObject placeholder = new GameObject();
        placeholder.transform.position = new Vector3(curve.x * 1/3, curve.y * 2/3, 0);
        waypointArray[1] = placeholder.transform;
        */
    }

    void Update()
    {
        if (currentPathPercent < 1)
        {
            currentPathPercent += percentsPerSecond * Time.deltaTime;
        iTween.PutOnPath(gameObject, waypointArray, currentPathPercent);
        percentsPerSecond += 0.018f;
        }
        else
        {
            StartCoroutine(Decay());
        }
    }

    IEnumerator Decay()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
