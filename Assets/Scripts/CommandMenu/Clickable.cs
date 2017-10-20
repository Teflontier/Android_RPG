using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerDownHandler {

    protected GameManager gameManager;

    public virtual void Awake() {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        gameManager.objectWasClicked(gameObject);
    }
}
