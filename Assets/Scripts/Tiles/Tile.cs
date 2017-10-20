using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : Clickable {
    public override void Awake() {
        base.Awake();
        PolygonCollider2D poly = gameObject.AddComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        poly.points = new Vector2[]{ new Vector2(0, 0.5f), new Vector2(-0.5f, 0.25f), new Vector2(-0.5f, -0.25f), new Vector2(0, -0.5f), new Vector2(0.5f, -0.25f), new Vector2(0.5f, 0.25f) };
    }
}
