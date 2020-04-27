using UnityEngine;
using UnityEngine.UI;

public class FlyerManager : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text descriptionText;

    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }
}
