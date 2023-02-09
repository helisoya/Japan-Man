using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Title_Button : MonoBehaviour
{
    public void Event_Enter(BaseEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
