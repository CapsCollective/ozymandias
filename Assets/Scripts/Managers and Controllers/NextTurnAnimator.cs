using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class NextTurnAnimator : MonoBehaviour
{
    public Light sun;
    public float sunSetTime = 2f;
    public ParticleSystem glowflyPS;

    private float orig_angle;
    private float t = 0f;
    private float x = 0f;

    void Awake()
    {
        GameManager.OnNextTurn += OnNextTurn;
        orig_angle = sun.transform.eulerAngles.x;
    }

    private void OnDestroy()
    {
        GameManager.OnNextTurn -= OnNextTurn;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnNextTurn()
    {
        StartCoroutine(AnimateSun());

    }

    public IEnumerator AnimateSun()
    {
        while (t < sunSetTime)
        {
            t += Time.deltaTime;
            x = Mathf.Lerp(orig_angle, orig_angle + 360f, t / sunSetTime);
            if(!glowflyPS.isEmitting&& (x>170 || x < 10))
            {
                glowflyPS.Play();
            }
            sun.transform.eulerAngles = new Vector3(x, sun.transform.rotation.y, sun.transform.rotation.z);
            yield return null;
        }
        t = 0f;
        Manager.NewTurn();
    }
}
