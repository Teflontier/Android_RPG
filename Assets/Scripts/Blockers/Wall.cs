using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Blocker {
    public override void Awake() {
        base.Awake();
//        PolygonCollider2D poly = transform.Find("poly").gameObject.GetComponent<PolygonCollider2D>();
//        poly.points = new Vector2[]{ new Vector2(0, 0.5f), new Vector2(-0.5f, 0.25f), new Vector2(-0.5f, -0.25f), new Vector2(0, -0.5f), new Vector2(0.5f, -0.25f), new Vector2(0.5f, 0.25f) };
    }
}
