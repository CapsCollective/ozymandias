using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public GameObject helper;
    public GameObject instantiatedHelper;
    public bool isHovered = false;
    private Place place;

    private void Start()
    {
        place = FindObjectOfType<Place>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        place.hoverObject = this;
    }
    

    public void InfoBox()
    {
        instantiatedHelper = Instantiate(helper, transform, false);
        instantiatedHelper.transform.localPosition = new Vector3(0, 150, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(instantiatedHelper);
        isHovered = false;
    }
}
