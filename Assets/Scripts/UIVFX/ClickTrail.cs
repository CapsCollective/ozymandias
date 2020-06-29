using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrail : MonoBehaviour
{
    //object to instantiate
    public GameObject effect;
    //tracking instantiated object
    private GameObject trail;
    //directional information
    public Transform start;
    public GameObject end;
    private Transform[] directions = new Transform[3];

    public void StartEffect()
    {
        //Instantiate trail object
        trail = Instantiate(effect, transform.position, Quaternion.identity);
        trail.transform.SetParent(transform);

        //change particle color depending on location
        var particleMain = trail.GetComponent<ParticleSystem>().main;
        if (end.name == "Target1")
        {
            particleMain.startColor = Color.yellow;
        }
        if (end.name == "Target2")
        {
            particleMain.startColor = Color.cyan;
        }

        //create directions for the trail object [start, curve, end]
        directions[0] = start;
        directions[2] = end.transform;
        Vector3 curve = new Vector3();
        curve = start.position + end.transform.position;
        GameObject placeholder = new GameObject();
        placeholder.transform.position = new Vector3(curve.x * 1 / 3, curve.y * 2 / 3, 0);
        directions[1] = placeholder.transform;

        //pass directions to trail object and begin coroutine to vary target shape
        //trail.GetComponent<Trail>().waypointArray = directions;
        StartCoroutine(Scale(placeholder));
    }

    //scale the object the particles are flying to
    IEnumerator Scale(GameObject placeholder)
    {
        yield return new WaitForSeconds(0.7f);
        iTween.ScaleAdd(end, new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        iTween.ScaleAdd(end, new Vector3(-0.1f, -0.1f, -0.1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        iTween.ScaleAdd(end, new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
        yield return new WaitForSeconds(0.05f);
        iTween.ScaleAdd(end, new Vector3(-0.1f, -0.1f, -0.1f), 0.1f);
        Destroy(placeholder);
    }
}

