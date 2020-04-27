using UnityEngine;
using UnityEngine.UI;

public class NoticeboardController : MonoBehaviour
{
    [SerializeField] private Text textList;

    public void Display()
    {
        textList.text = string.Join("\n", GetDummyEvents());
    }

    private static string[] GetDummyEvents()
    {
        return new [] {"Real Fake Event 1", "Real Fake Event 2"};
    }
}
