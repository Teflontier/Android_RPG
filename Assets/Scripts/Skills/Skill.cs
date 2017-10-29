using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {

    public Sprite image;
    protected Entity owner;
    protected LevelManager levelManager;

    public void Awake() {
        owner = gameObject.GetComponent<Entity>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    public abstract void activate(GameObject target);

    public abstract bool canBeUsed(GameObject target);

}
