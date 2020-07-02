using System.Collections;
using System.Collections.Generic;
using Managers_and_Controllers;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DepthOfFieldController : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private PostProcessProfile profile;
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private LayerMask layerMask;
    private DepthOfField depthOfField;
    private Camera cam;
    private CameraController cc;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cc = GetComponent<CameraController>();
        profile.TryGetSettings<DepthOfField>(out depthOfField);
    }

    // Update is called once per frame
    void Update()
    {
        volume.weight = Mathf.InverseLerp(cc.maxHeight, cc.minHeight, transform.position.y);
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            depthOfField.focusDistance.value = hit.distance;
        }
    }
}
