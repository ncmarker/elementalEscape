using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{    
    public bool pressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        Debug.Log(gameObject.name + " is pressed.");
    }

    // Called when the user releases the button
    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        Debug.Log(gameObject.name + " is released.");
    }
}
