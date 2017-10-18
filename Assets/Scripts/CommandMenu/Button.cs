using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    protected GameManager gameManager;

    public void Awake() {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        GetComponent<MeshRenderer>().sortingLayerName = "CommandMenu";
    }

    public void OnMouseDown() {
        gameManager.objectWasClicked(gameObject);
    }
}
