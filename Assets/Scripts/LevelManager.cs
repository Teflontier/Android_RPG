using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public int width = 2;
    public int height = 2;

    public int minBlockers = 0;
    public int maxBlockers = 0;

    [HideInInspector] public int minTileX;
    [HideInInspector] public int maxTileX;
    [HideInInspector] public int minTileY;
    [HideInInspector] public int maxTileY;

    [HideInInspector] public List<Tile> tiles = new List<Tile>();
    [HideInInspector] public List<Blocker> blockers = new List<Blocker>();
    [HideInInspector] public List<Entity> entities = new List<Entity>();
    [HideInInspector] public Tile[,] tileMatrix;

    private DDOL ddol;
    private Transform playerObjectHolder;
    private Transform mobObjectHolder;
    private Transform tileObjectHolder;
    private Transform blockerObjectHolder;
    private Transform overlayObjectHolder;

    public void Awake() {
        ddol = GameObject.FindObjectOfType<DDOL>();
        playerObjectHolder = transform.Find("Players");
        mobObjectHolder = transform.Find("Mobs");
        tileObjectHolder = transform.Find("Tiles");
        blockerObjectHolder = transform.Find("Blockers");
        overlayObjectHolder = transform.Find("Overlays");
        tileMatrix = new Tile[width, height];
    }

    public void createLevel(List<Player> playersToPlace) {
        destroyLevel();
        createTiles();
        createBlockers();
        entities.Clear();
        playersToPlace.ForEach(player => entities.Add(player));
        createPlayers(playersToPlace);
        createMobs();
    }

    private void destroyLevel() {
        foreach (Transform child in playerObjectHolder)
            Destroy(child.gameObject);
        foreach (Transform child in mobObjectHolder)
            Destroy(child.gameObject);
        foreach (Transform child in tileObjectHolder)
            Destroy(child.gameObject);
        foreach (Transform child in blockerObjectHolder)
            Destroy(child.gameObject);
        foreach (Transform child in overlayObjectHolder)
            Destroy(child.gameObject);
        tiles.Clear();
        blockers.Clear();
        entities.Clear();
    }

    private void createTiles() {
        minTileX = 0;
        maxTileX = width - 1;
        minTileY = 0;
        maxTileY = height - 1;
        tiles.Clear();
        for (int i = 0; i < width; i++) {
            for (int k = 0; k < height; k++) {
                Vector2 pos = getCoordinatesFor(i, k);
                Tile tile = GameObject.Instantiate(ddol.tiles[Random.Range(0, ddol.tiles.Count)], pos, Quaternion.identity, tileObjectHolder) as Tile;
                tileMatrix[i, k] = tile;
                tiles.Add(tile);
            }
        }
        for (int i = 0; i < width; i++) {
            for (int k = 0; k < height; k++) {
                Tile tile = tileMatrix[i, k];
                if (i - 1 >= 0)
                    tile.left = tileMatrix[i - 1, k];
                if (i + 1 < width)
                    tile.right = tileMatrix[i + 1, k];

                if (k % 2 == 0) {
                    if (k + 1 < height) {
                        tile.topRight = tileMatrix[i, k + 1];
                        if (i - 1 >= 0)
                            tile.topLeft = tileMatrix[i - 1, k + 1];
                    }
                    if (k - 1 > 0) {
                        tile.bottomRight = tileMatrix[i, k - 1];
                        if (i - 1 >= 0)
                            tile.bottomLeft = tileMatrix[i - 1, k - 1];
                    }
                }
                else {
                    if (k + 1 < height) {
                        tile.topLeft = tileMatrix[i, k + 1];
                        if (i + 1 < width)
                            tile.topRight = tileMatrix[i + 1, k + 1];
                    }
                    if (k - 1 >= 0) {
                        tile.bottomLeft = tileMatrix[i, k - 1];
                        if (i + 1 < width)
                            tile.bottomRight = tileMatrix[i + 1, k - 1];
                    }
                }
            }
        }
    }

    private void createBlockers() {
        for (int i = 0; i < Random.Range(minBlockers, maxBlockers); i++) {
            Vector2 coordinates = getRandomFreeTileCoordinates();
            blockers.Add(GameObject.Instantiate(ddol.blockers[Random.Range(0, ddol.blockers.Count)], coordinates, Quaternion.identity, blockerObjectHolder) as Blocker);
        }
    }

    private void createPlayers(List<Player> playersToPlace) {
        foreach (Player player in playersToPlace) {
            player.transform.SetParent(playerObjectHolder);
            player.transform.position = getRandomFreeTileCoordinates();
        }
    }

    private void createMobs() {
        Vector2 coordinates = getRandomFreeTileCoordinates();
        entities.Add(GameObject.Instantiate(ddol.mobs[Random.Range(0, ddol.mobs.Count)], coordinates, Quaternion.identity, mobObjectHolder) as Mob);
    }

    private Vector2 getRandomCoordinates() {
        return getCoordinatesFor(Random.Range(0, width), Random.Range(0, height));
    }

    private Vector2 getRandomFreeTileCoordinates() {
        Vector2 coordinates;
        int stop = 0;
        while (true) {
            coordinates = getRandomCoordinates();
            Blocker b = blockers.Find(blocker => ((Vector2)blocker.transform.position).Equals(coordinates));
            Entity e = entities.Find(entity => ((Vector2)entity.transform.position).Equals(coordinates));
            bool isEmpty = b == null && e == null;
            if (isEmpty)
                return coordinates;
            stop++;
            if (stop >= 1000)
                break;
        }
        return new Vector2(0, 0);
    }

    public void createOverlay(GameObject overlay, Vector2 position) {
        GameObject.Instantiate(overlay, position, Quaternion.identity, overlayObjectHolder);
    }

    public void destroyOverlays() {
        foreach (Transform child in overlayObjectHolder)
            Destroy(child.gameObject);
    }

    public static Vector2 getCoordinatesFor(float indexX, float indexY) {
        Vector2 pos = new Vector2(indexX, indexY);
        if (indexY % 2 != 0)
            pos.x += 0.5f;
        pos.y -= 0.25f * indexY;
        return pos;
    }

    public static Vector2 getCoordinatesFor(Vector2 indices) {
        return getCoordinatesFor(indices.x, indices.y);
    }

    public static Vector2 getIndicesFor(float posX, float posY) {
        posY = posY / 0.75f;
        Vector2 pos = new Vector2(posX, posY);
        if (posY % 2 != 0)
            pos.x -= 0.5f;
        return pos;
    }

    public static Vector2 getIndicesFor(Vector2 pos) {
        return getIndicesFor(pos.x, pos.y);
    }
}
