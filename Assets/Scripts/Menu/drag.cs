using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Map map;
    private Image image;
    public GameObject thing;
    private GameObject thingInstantiated;

    public Transform parent = null;
    public Transform parentPlaceholder = null;
    GameObject placeHolder = null;

    private Vector2 mousePos;
    private Vector3 worldPoint;
    RaycastHit hit;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Set placeholder so that a card can return to it's original position
        placeHolder = new GameObject();
        placeHolder.transform.SetParent(transform.parent);
        LayoutElement le = placeHolder.AddComponent<LayoutElement>();
        le.preferredWidth = GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        //By making the transform a child of the canvas, it is not longer locked to a board
        parent = transform.parent;
        parentPlaceholder = parent;
        transform.SetParent(transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //////////////////////////////////////////////////////Intantiate object on board/////////////////////////////////////////////////////////
        //If card is outside panel,
        if (!eventData.pointerEnter)
        {
            //fire a ray to see if it is being hovered over an object.
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            Physics.Raycast(ray, out hit);
            //If it has not instantiated an object
            if (!thingInstantiated)
            {
                if (hit.collider) 
                {
                    thingInstantiated = Instantiate(thing, hit.point, transform.rotation);
                    image.enabled = false;
                } 
            }
            //If it has not instantiated an object
            else
            {
                thingInstantiated.transform.position = hit.point;
            }
        }
        //If card is in panel
        else if (eventData.pointerEnter)                                                                                                                                {
            image.enabled = true                                                                                                                                      ;
            Destroy(thingInstantiated)                                                                                                                                ;
                                                                                                                                                                        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Placeholders
        transform.position = eventData.position;
        int newSiblingIndex = parentPlaceholder.childCount;

        //The default position for a card is the right most of the list. Otherwise if position is more left than a card then put it there.
        for (int i = 0; i < parentPlaceholder.childCount; i++)
        {
            if (transform.position.x < parentPlaceholder.GetChild(i).position.x)
            {
                newSiblingIndex = i;

                if (placeHolder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }
                break;
            }
        }
        placeHolder.transform.SetSiblingIndex(newSiblingIndex);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parent);
        transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(placeHolder);
        map.Occupy(thingInstantiated.transform.position);
        Destroy(thingInstantiated);
    }
}
