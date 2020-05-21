using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cancel : MonoBehaviour
{
    private EventSystem eventSystem;
    private Button button;
    private Image image;
    private Place place;
    private GameObject previousSelect;
    private Click previousClick;
    // Start is called before the first frame update
    void Start()
    {
        place = FindObjectOfType<Place>();
        eventSystem = EventSystem.current;
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.enabled = false;
        image.enabled = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (place.selectedObject)
        {
            button.enabled = true;
            image.enabled = true;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        else
        {
            button.enabled = false;
            image.enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void CancelSelection()
    {
        place.Deselect();
    }
}
