using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccumulatedHatred : Skill {

    public int range = 3;
    public float damage = 2f;
    private float snapDist = 0.1f;
    private float moveSpeed = 3f;
    private GameObject particles;
    private LayerMask layerMask;

    public new void Awake() {
        base.Awake();
        layerMask = LayerMask.GetMask("Tile");
    }

    public override void initialize(Tile target) {
        particles = GameObject.Instantiate(ddol.teleportationParticles, user.gameObject.transform);
    }

    public override bool activate(Tile target) {
        particles.transform.position = Vector2.MoveTowards(particles.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(particles.transform.position, target.transform.position) > snapDist)
            return false;
        Destroy(particles);
        levelManager.getEntityOnTile(target).increaseHp(-damage);
        return true;
    }

    public override bool canBeUsed(Tile target) {
        Tile startingTile = levelManager.getTileForPosition(user.transform.position);
        if (levelManager.getEntityOnTile(target) == null)
            return false;
        float distance = (target.transform.position - user.transform.position).magnitude;
        RaycastHit2D[] hits = Physics2D.LinecastAll(user.transform.position, target.transform.position, layerMask);
        foreach (RaycastHit2D hit in hits) {
            Tile hitTile = hit.collider.gameObject.GetComponent<Tile>();
            if (hitTile == null)
                continue;
            Entity hitEntity = levelManager.getEntityOnTile(hitTile);
            if (hitEntity == null || hitEntity == user)
                continue;
            float hitDist = Vector2.Distance(user.transform.position, hit.collider.gameObject.transform.position);
            if (hitDist < distance)
                return false;
        }
        Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> floodFilledTiles = TileUtilities.floodFill<int>(startingTile, targetTile: target);
        return TileUtilities.getShortestWayFromFloodFilledTiles(floodFilledTiles, startingTile, target).Count <= range;
    }
}
