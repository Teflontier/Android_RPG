using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {

    public Sprite image;
    protected Entity owner;
    protected LevelManager levelManager;
    protected DDOL ddol;

    public void Awake() {
        owner = gameObject.GetComponent<Entity>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        ddol = GameObject.FindObjectOfType<DDOL>();
    }

    public abstract void initialize(GameObject target);

    public abstract bool activate(GameObject target);

    public abstract bool canBeUsed(GameObject target);

}
