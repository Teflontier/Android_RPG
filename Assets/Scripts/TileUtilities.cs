using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TileUtilities : MonoBehaviour {

    public static List<Tile> getShortestWayFromFloodFilledTiles<T>(Dictionary<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>> floodFilledTiles, Tile targetTile) {
        List<Tile> shortestWay = new List<Tile>();
        Tile tile = targetTile;
        shortestWay.Add(tile);
        WrappedTile<T> temp = null;
        while ((temp = getWrappedTileInFloodFilledTiles(floodFilledTiles, tile)) != null)
            shortestWay.Insert(0, tile = floodFilledTiles[temp].Key.tile);
        return shortestWay;
    }

    public static WrappedTile<T> getWrappedTileInFloodFilledTiles<T>(Dictionary<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>> floodFilledTiles, Tile tile) {
        KeyValuePair<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>> temp = floodFilledTiles.FirstOrDefault(wrappedTile => wrappedTile.Key.tile == tile);
        return temp.Equals(default(KeyValuePair<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>>)) ? null : temp.Key;
    }

    public static Dictionary<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>> floodFill<T>(Tile startingTile, int searchDepth = -1, Predicate<Tile> adjacentTilesFilter = null, Tile targetTile = null, Func<Tile, T> heuristicForShortestPath = null, Comparer<T> insertionComparer = null) {
        if (searchDepth < 0 && targetTile == null)
            throw new Exception("Search depth is infinity and target tile is null -> endless loop");

        Dictionary<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>> floodFilledTiles = new Dictionary<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>>();
        List<WrappedTile<T>> tilesToCheck = new List<WrappedTile<T>>();
       
        bool doHeuristicInsertion = heuristicForShortestPath != null && insertionComparer != null;
        tilesToCheck.Add(new WrappedTile<T>(startingTile, doHeuristicInsertion ? heuristicForShortestPath(startingTile) : default(T)));

        int currentSeachDepth = searchDepth;
        while (tilesToCheck.Count > 0) {
            WrappedTile<T> tileToCheck = tilesToCheck[0];
            tilesToCheck.RemoveAt(0);
            if (tileToCheck.tile != startingTile)
                currentSeachDepth = floodFilledTiles[tileToCheck].Value - 1;
            if (currentSeachDepth == 0)
                continue;
            List<WrappedTile<T>> adjacentTiles = getAdjacentTilesFiltered(tileToCheck, adjacentTilesFilter, currentSeachDepth, floodFilledTiles);
            if (doHeuristicInsertion)
                insertTilesOrdered(insertionComparer, adjacentTiles, tilesToCheck);
            else
                tilesToCheck.AddRange(adjacentTiles);
        }
        return floodFilledTiles;
    }

    public static List<WrappedTile<T>> getAdjacentTilesFiltered<T>(WrappedTile<T> tile, Predicate<Tile> adjacentTilesFilter = null, int searchDepth = 1, Dictionary<WrappedTile<T>, KeyValuePair<WrappedTile<T>, int>> floodFilledTiles = null, Func<Tile, T> heuristicForShortestPath = null) {
        List<WrappedTile<T>> surroundingTiles = new List<WrappedTile<T>>();
        bool floodFilledTilesGiven = floodFilledTiles != null;
        bool adjacentTilesFilterNonNull = adjacentTilesFilter != null;
        tile.tile.getSurroundingTiles()
            .FindAll(t => {
                if (adjacentTilesFilterNonNull && !adjacentTilesFilter(t))
                    return false;
                if (!floodFilledTilesGiven)
                    return true;
                return floodFilledTiles.Where(pair => pair.Key.tile == t).Select(pair => pair.Key.tile).FirstOrDefault() == null;
            }).ForEach(adjacentTile => {
                WrappedTile<T> wrappedAdjacentTile = new WrappedTile<T>(adjacentTile, heuristicForShortestPath != null ? heuristicForShortestPath(adjacentTile) : default(T));
                surroundingTiles.Add(wrappedAdjacentTile);
                if (floodFilledTilesGiven)
                    floodFilledTiles.Add(wrappedAdjacentTile, new KeyValuePair<WrappedTile<T>, int>(tile, searchDepth));
            });
        return surroundingTiles;
    }

    private static void insertTilesOrdered<T>(Comparer<T> insertionComparer, List<WrappedTile<T>> tilesToBeInserted, List<WrappedTile<T>> tilesToInsertIn) {
        tilesToBeInserted.ForEach(adTile => {
                for (int i = 0; i < tilesToInsertIn.Count; i++) {
                    if (insertionComparer.Compare(adTile.metadata, tilesToInsertIn[i].metadata) < 0) {
                        tilesToInsertIn.Insert(i, adTile);
                        return;
                    }
                }
            });
    }
}
