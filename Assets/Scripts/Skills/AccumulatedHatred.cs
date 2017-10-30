using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccumulatedHatred : Skill {

    public int range = 3;
    public float damage = 2f;
    private float snapDist = 0.3f;
    private float moveSpeed = 3f;
    private GameObject particles;

    public override void initialize(GameObject target) {
        particles = GameObject.Instantiate(ddol.teleportationParticles, owner.gameObject.transform);
    }

    public override bool activate(GameObject target) {
        if (Vector2.Distance(particles.transform.position, target.transform.position) <= snapDist) {
            Destroy(particles);
            levelManager.getGameObjectOnTile(target.GetComponent<Tile>()).GetComponent<Mob>().increaseHp(-damage);
            return true;
        }
        particles.transform.position = Vector2.MoveTowards(particles.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        return false;
    }

    public override bool canBeUsed(GameObject target) {
        Tile startingTile = levelManager.getTileForPosition(owner.transform.position);
        Tile targetTile = target.GetComponent<Tile>();
        if (targetTile == null)
            return false;
        GameObject objectOnTile = levelManager.getGameObjectOnTile(targetTile);
        if (objectOnTile == null)
            return false;
        Mob targetMob = objectOnTile.GetComponent<Mob>();
        if (targetMob == null)
            return false;
        Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> floodFilledTiles = TileUtilities.floodFill<int>(startingTile, targetTile: targetTile);
        return TileUtilities.getShortestWayFromFloodFilledTiles(floodFilledTiles, startingTile, targetTile).Count <= range;
    }
}
