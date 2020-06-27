using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTrail : MonoBehaviour
{
    public GameObject UIvfx;
    public GameObject origin;
    public GameObject destination;
    public iTween.EaseType easeType;
    public float time;
    public float rate;
    public Vector3 offset;

    public void FromToCall()
    {
        StartCoroutine(FromTo());
        StartCoroutine(Scale());
    }

    IEnumerator FromTo()
    {
        var vfx = Instantiate(UIvfx, origin.transform.position, Quaternion.identity) as GameObject;
        vfx.transform.SetParent(origin.transform);
           
        iTween.MoveTo(vfx, iTween.Hash("position", destination.transform.position + offset,
                                        "easetype", easeType, 
                                        "ignoretimescale", true, 
                                        "time", time));
        yield return new WaitForSeconds(rate);
    }

    IEnumerator Scale()
    {
        yield return new WaitForSeconds(0.35f);
        iTween.ScaleAdd(destination, new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        iTween.ScaleAdd(destination, new Vector3(-0.1f, -0.1f, -0.1f), 0.1f);
        yield return new WaitForSeconds(0.1f);
        iTween.ScaleAdd(destination, new Vector3(0.1f, 0.1f, 0.1f), 0.1f);
        yield return new WaitForSeconds(0.05f);
        iTween.ScaleAdd(destination, new Vector3(-0.1f, -0.1f, -0.1f), 0.1f);
    }
}
