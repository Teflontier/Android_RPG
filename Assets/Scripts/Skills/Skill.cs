﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {

    public Sprite image;
    protected Entity user;
    protected LevelManager levelManager;
    protected DDOL ddol;

    public void Awake() {
        user = gameObject.GetComponent<Entity>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        ddol = GameObject.FindObjectOfType<DDOL>();
    }

    public abstract void initialize(Tile target);

    public abstract bool activate(Tile target);

    public abstract bool canBeUsed(Tile target);

}
