using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Blocker : MonoBehaviour {

    protected GameManager gameManager;

    public void Awake() {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public void OnMouseDown(){
        gameManager.objectWasClicked(gameObject);
    }
}
