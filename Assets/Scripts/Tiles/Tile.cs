using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : Clickable {

    public Tile topLeft;
    public Tile topRight;
    public Tile left;
    public Tile right;
    public Tile bottomLeft;
    public Tile bottomRight;

    public override void Awake() {
        base.Awake();
        PolygonCollider2D poly = gameObject.AddComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        poly.points = new Vector2[]{ new Vector2(0, 0.5f), new Vector2(-0.5f, 0.25f), new Vector2(-0.5f, -0.25f), new Vector2(0, -0.5f), new Vector2(0.5f, -0.25f), new Vector2(0.5f, 0.25f) };
    }

    public List<Tile> getSurroundingTiles() {
        List<Tile> surroundingTiles = new List<Tile>();
        if (topLeft != null)
            surroundingTiles.Add(topLeft);
        if (topRight != null)
            surroundingTiles.Add(topRight);
        if (left != null)
            surroundingTiles.Add(left);
        if (right != null)
            surroundingTiles.Add(right);
        if (bottomLeft != null)
            surroundingTiles.Add(bottomLeft);
        if (bottomRight != null)
            surroundingTiles.Add(bottomRight);
        return surroundingTiles;
    }

}
