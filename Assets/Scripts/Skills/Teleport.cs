using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : Skill {

    public int range = 7;
    private float snapDist = 0.3f;
    private float moveSpeed = 15f;
    private float colorAlpha = 0.5f;
    private GameObject particles;

    public override void initialize(Tile target) {
        Color color = owner.spriteRenderer.color;
        color.a = colorAlpha;
        owner.spriteRenderer.color = color;
        particles = GameObject.Instantiate(ddol.teleportationParticles, owner.gameObject.transform);
    }

    public override bool activate(Tile target) {
        if (Vector2.Distance(owner.gameObject.transform.position, target.transform.position) <= snapDist) {
            owner.transform.position = target.transform.position;
            Color color = owner.spriteRenderer.color;
            color.a = 1f;
            owner.spriteRenderer.color = color;
            Destroy(particles);
            return true;
        }
        owner.transform.position = Vector2.MoveTowards(owner.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        return false;
    }

    public override bool canBeUsed(Tile target) {
        Tile startingTile = levelManager.getTileForPosition(owner.transform.position);
        Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> floodFilledTiles = TileUtilities.floodFill<int>(startingTile, targetTile: target);
        return levelManager.isMovable(target) && TileUtilities.getShortestWayFromFloodFilledTiles(floodFilledTiles, startingTile, target).Count <= range;
    }
}
