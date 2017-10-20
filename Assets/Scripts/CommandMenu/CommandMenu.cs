using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMenu : MonoBehaviour {

    private GameObject moveButton;
    private GameObject endButton;

    public void Awake(){
        moveButton = transform.Find("Move").gameObject;
        endButton = transform.Find("End").gameObject;
        moveButton.GetComponent<MeshRenderer>().sortingLayerName = "CommandMenu";
        endButton.GetComponent<MeshRenderer>().sortingLayerName = "CommandMenu";
    }

    public void setVisibility(bool visible){
        setMoveVisibility(visible);
        setEndVisibility(visible);
    }

    public void setMoveVisibility(bool visible){
        moveButton.SetActive(visible);
    }

    public void setEndVisibility(bool visible){
        endButton.SetActive(visible);
    }
}
