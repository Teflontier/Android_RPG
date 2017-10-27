using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TileUtilities : MonoBehaviour {

    public static Dictionary<Tile, KeyValuePair<Tile, int>> floodFill(Tile startingTile, int searchDepth, Predicate<Tile> filter) {
        Dictionary<Tile, KeyValuePair<Tile, int>> floodFilledTiles = new Dictionary<Tile, KeyValuePair<Tile, int>>();
        List<Tile> tilesToCheck = new List<Tile>();
        tilesToCheck.Add(startingTile);

        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            tilesToCheck.RemoveAt(0);
            if (tileToCheck != startingTile)
                searchDepth = floodFilledTiles[tileToCheck].Value - 1;
            if (searchDepth > 0)
                tilesToCheck.AddRange(getAdjacentTilesFiltered(searchDepth, tileToCheck, filter, floodFilledTiles));
        }
        return floodFilledTiles;
    }

    public static List<Tile> getAdjacentTilesFiltered(int searchDepth, Tile tile, Predicate<Tile> filter, Dictionary<Tile, KeyValuePair<Tile, int>> floodFilledTiles) {
        List<Tile> surroundingTiles = new List<Tile>();
        tile.getSurroundingTiles().FindAll(t => (!floodFilledTiles.ContainsKey(t) && filter(t))).ForEach(adjacentTile => {
                surroundingTiles.Add(adjacentTile);
                floodFilledTiles.Add(adjacentTile, new KeyValuePair<Tile, int>(tile, searchDepth));
            });
        return surroundingTiles;
    }


}
