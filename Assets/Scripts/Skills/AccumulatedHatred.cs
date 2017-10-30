using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccumulatedHatred : Skill {

    public int range = 3;
    public float damage = 2f;
    private float snapDist = 0.3f;
    private float moveSpeed = 3f;
    private GameObject particles;
    private LayerMask layerMask;

    public override void initialize(Tile target) {
        particles = GameObject.Instantiate(ddol.teleportationParticles, owner.gameObject.transform);
        layerMask = LayerMask.GetMask("Tile");
    }

    public override bool activate(Tile target) {
        particles.transform.position = Vector2.MoveTowards(particles.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(particles.transform.position, snapDist, layerMask);
        foreach (Collider2D  collider in objectsInRange) {
            Tile tile = collider.gameObject.GetComponent<Tile>();
            GameObject goOnTile = levelManager.getGameObjectOnTile(tile);
            if (goOnTile == null)
                continue;
            Entity entityOnTile = goOnTile.GetComponent<Entity>();
            if (entityOnTile != null && entityOnTile != owner) {
                Destroy(particles);
                entityOnTile.increaseHp(-damage);
                return true;
            }
        }
        return false;
    }

    public override bool canBeUsed(Tile target) {
        Tile startingTile = levelManager.getTileForPosition(owner.transform.position);
        if (getMobOnTile(target) == null)
            return false;
        Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> floodFilledTiles = TileUtilities.floodFill<int>(startingTile, targetTile: target);
        return TileUtilities.getShortestWayFromFloodFilledTiles(floodFilledTiles, startingTile, target).Count <= range;
    }
}
