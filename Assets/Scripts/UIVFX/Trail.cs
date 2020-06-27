using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public Transform[] waypointArray = new Transform[3];
    float percentsPerSecond = 0.7f; 
    float currentPathPercent = 0.0f;

    void Update()
    {
        if (currentPathPercent < 1)
        {
            currentPathPercent += percentsPerSecond * Time.deltaTime;
        iTween.PutOnPath(gameObject, waypointArray, currentPathPercent);
        percentsPerSecond += 0.035f;
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
