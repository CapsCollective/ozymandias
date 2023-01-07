using System;
using UnityEngine;

namespace UI
{
    public class Notifications : MonoBehaviour
    {
        [SerializeField] private GameObject notificationPrefab;
        
        public void Display(string text, Sprite icon = null, float delay = 0, Action onClick = null)
        {
            Instantiate(notificationPrefab, transform).GetComponent<Notification>().Display(text,icon,delay,onClick);
        }
    }
}
