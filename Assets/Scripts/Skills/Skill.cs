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

    protected Entity getEntityOnTile(Tile target) {
        GameObject objectOnTile = levelManager.getGameObjectOnTile(target);
        if (objectOnTile == null)
            return null;
        return objectOnTile.GetComponent<Entity>();
    }

    public abstract void initialize(Tile target);

    public abstract bool activate(Tile target);

    public abstract bool canBeUsed(Tile target);

}
