﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMenu : MonoBehaviour {

    private GameObject moveButton;
    private GameObject attackButton;
    private GameObject endButton;

    public void Awake(){
        moveButton = transform.Find("Move").gameObject;
        attackButton = transform.Find("Attack").gameObject;
        endButton = transform.Find("End").gameObject;
    }

    public void setVisibility(bool visible){
        setMoveVisibility(visible);
        setAttackVisibility(visible);
        setEndVisibility(visible);
    }

    public void setMoveVisibility(bool visible){
        moveButton.SetActive(visible);
    }

    public void setAttackVisibility(bool visible){
        attackButton.SetActive(visible);
    }

    public void setEndVisibility(bool visible){
        endButton.SetActive(visible);
    }
}
