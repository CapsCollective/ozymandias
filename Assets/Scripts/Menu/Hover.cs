using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/*
 * At the moments works with the place script to handle hover actions (because at the moment, it requires a right click to open up a help
 * This doesn't translate that well when it's scaled to more than just building information (having to right click everything, even knowing to right click)
 * As a result, making the helper object fade in after a certain amount of time of the cursor being hovered over an object
 * In addition, making enumerators for this script so it adapts to the need of each ui object as opposed to having to make a new script each time
 */ 

public enum UIType {building, threat, quest, destroy, money, sidebar  };

public class Hover : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private GameObject[] helperPrefab = new GameObject[5];
    //building[0], threat[1], quest[2], destroy[3], money[4], sidebar[5]
    public UIType uiType;
    private GameObject helper;
    //private bool isHovered;
    private bool faded = true;
    private float fadeDuration = 0.25f;
    /*
    private void Awake()
    {
        ClickManager.OnRightClick += RightClick;
    }
    
    public void RightClick()
    {
        if (!isHovered) return;
        if (helper) Destroy(helper);
        else InfoBox();
    }
    */

    public void OnPointerEnter(PointerEventData eventData)
    {
        //isHovered = true;
        UIGet(uiType);
        faded = !faded;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(helper);
        //isHovered = false;
        faded = !faded;
}

    public IEnumerator Fade(CanvasGroup canvasGroup, float start, float end)
    {
        yield return new WaitForSeconds(0.5f);
        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            if (canvasGroup) canvasGroup.alpha = Mathf.Lerp(start, end, counter / fadeDuration);

            yield return null;
        }
    }
    
    /*
    public void InfoBox()
    {
        if (!helperPrefab) return;
        helper = Instantiate(helperPrefab, transform, false);
        helper.transform.localPosition = new Vector3(0, 150, 0);
    }
    */


    public void InfoInstantiate(GameObject info, Vector3 offset)
    {
        if (!info) return;
        helper = Instantiate(info, transform, false);
        helper.transform.localPosition = offset;
        CanvasGroup canvasGroup = helper.GetComponent<CanvasGroup>();
        StartCoroutine(Fade(canvasGroup, canvasGroup.alpha, faded ? 1 : 0 ));
    }

    private void UIGet(UIType ui)
    {
        switch (ui)
        {
            case UIType.building:
                InfoInstantiate(helperPrefab[0], new Vector3(0, 100, 0));
                if (helper) helper.GetComponent<BuildingHelper>().FillText(gameObject);
                break;
            case UIType.threat:
                InfoInstantiate(helperPrefab[1], new Vector3(0, -80, 0));
                break;
            case UIType.quest:
                InfoInstantiate(helperPrefab[2], new Vector3(0, 100, 0));
                break;
            case UIType.destroy:
                InfoInstantiate(helperPrefab[3], new Vector3(0, 100, 0));
                break;
            case UIType.money:
                InfoInstantiate(helperPrefab[4], new Vector3(30, 150, 0));
                break;
            case UIType.sidebar:
                InfoInstantiate(helperPrefab[5], new Vector3(-100, 0, 0));
                break;
        }
    }
}
