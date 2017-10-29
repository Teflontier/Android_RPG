using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : Skill {

    public int range = 7;

    public override void activate(GameObject target) {
        owner.transform.position = target.transform.position;
    }

    public override bool canBeUsed(GameObject target) {
        Tile startingTile = levelManager.getTileForPosition(owner.transform.position);
        Tile targetTile = target.GetComponent<Tile>();
        if (targetTile == null)
            return false;
        Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> floodFilledTiles = TileUtilities.floodFill<int>(startingTile, targetTile: targetTile);
        return levelManager.isMovable(targetTile) && TileUtilities.getShortestWayFromFloodFilledTiles(floodFilledTiles, startingTile, targetTile).Count <= range;
    }
}
