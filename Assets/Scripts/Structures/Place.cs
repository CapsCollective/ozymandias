using System.Collections.Generic;
using Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Utilities;
using static Managers.GameManager;

namespace Structures
{
    public class Place : MonoBehaviour
    {



        #region Controller Inputs

        /*private void RotateBuilding(InputAction.CallbackContext obj)
        {
            if (!Manager.Cards.SelectedCard) return;

            int dir = (int)Mathf.Sign(obj.ReadValue<float>());
            _rotation += dir;
            if (_rotation < 0) _rotation = 3;
            _rotation %= 4;
        }

        /*public void ImitateHover(int cardNum)
        {

            if (cardNum >= 0)
            {
                if (cards[cardNum].isReplacing) return;
                cards[cardNum].SelectCard();
                if (cards[cardNum].toggle.interactable)
                {
                    cards[cardNum].toggle.isOn = true;
                }
            }
        
            for (int i = 0; i < cards.Length; i++)
            {
                if (i == cardNum) continue;
        
                cards[i].DeselectCard();
                if (cards[i].toggle.isOn) cards[i].toggle.isOn = false;
            }
        }*/
        #endregion

        

        /*public int NavigateCards(int newCardNum)
        {
            if (newCardNum > cards.Length - 1)
                newCardNum = 0;
            else if (newCardNum < 0)
                newCardNum = cards.Length - 1;

            return newCardNum;
        }


        
        private void UICancel(InputAction.CallbackContext obj)
        {
            if (Manager.State.InIntro) return;

            if (isSelectingCard)
            {
                _place.ImitateHover(-1);
                isSelectingCard = false;
            }
        }

        private void Navigate(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame || ChangingCard(lastSelectedCard)) return;

            int dir = (int)obj.ReadValue<Vector2>().x;
            if (isSelectingCard)
            {
                lastSelectedCard = NavigateCards(lastSelectedCard + dir);
                ImitateHover(lastSelectedCard);
            }
        }

        private void SelectCards(InputAction.CallbackContext obj)
        {
            if (!Manager.State.InGame) return;

            if (!isSelectingCard)
            {
                _place.ImitateHover(lastSelectedCard);
                isSelectingCard = true;
            }
        }
    }*/
    }
}
