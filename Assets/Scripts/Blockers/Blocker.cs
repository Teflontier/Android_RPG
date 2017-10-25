using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Blocker : Clickable {

    private SpriteRenderer sRenderer;

    public override void Awake() {
        base.Awake();
        sRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void setMaterial(Material material){
        sRenderer.material = material;
    }
}
