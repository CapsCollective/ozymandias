using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DepthOfFieldController : MonoBehaviour
{
    [SerializeField] private PostProcessProfile profile;
    private DepthOfField depthOfField;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        profile.TryGetSettings<DepthOfField>(out depthOfField);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            depthOfField.focusDistance.value = hit.distance;
        }
    }
}
