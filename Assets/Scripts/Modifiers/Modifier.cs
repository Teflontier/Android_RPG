using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier : MonoBehaviour {

    public int runTime = -1;
    public int attacks = -1;
    protected Entity owner;

    public void Awake() {
        owner = gameObject.GetComponent<Entity>();
    }

    public void Start() {
        owner.recalculateModifiers();
    }

    public virtual bool checkAliveConditions() {
        return runTime != 0 && attacks != 0;
    }

    public abstract void modify();

}
