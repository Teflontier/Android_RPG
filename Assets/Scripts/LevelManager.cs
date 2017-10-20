using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static int width = 8;
    public static int height = 8;

    public int minBlockers = 5;
    public int maxBlockers = 9;

    public int minTileX;
    public int maxTileX;
    public int minTileY;
    public int maxTileY;

    [HideInInspector] public List<Tile> tiles = new List<Tile>();
    [HideInInspector] public List<Blocker> blockers = new List<Blocker>();
    [HideInInspector] public List<Entity> entities = new List<Entity>();
    [HideInInspector] public Tile[,] tileMatrix = new Tile[width, height];

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
    }

    public void createLevel(List<Player> playersToPlace) {
        destroyLevel();
        createTiles();
        createBlockers();
        entities.Clear();
        playersToPlace.ForEach(player => entities.Add(player));
        createPlayers(playersToPlace);
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
        }

    }

    private void createBlockers() {
        for (int i = 0; i < Random.Range(minBlockers, maxBlockers); i++) {
            Vector2 coordinates = getRandomCoordinatesWithoutBlocker();
            blockers.Add(GameObject.Instantiate(ddol.blockers[Random.Range(0, ddol.blockers.Count)], coordinates, Quaternion.identity, blockerObjectHolder) as Blocker);
        }
    }

    private void createPlayers(List<Player> playersToPlace) {
        foreach (Player player in playersToPlace) {
            player.transform.SetParent(playerObjectHolder);
            player.transform.position = getRandomCoordinatesWithoutBlocker();
        }
    }

    private Vector2 getRandomCoordinates() {
        return getCoordinatesFor(Random.Range(0, width), Random.Range(0, height));
    }

    private Vector2 getRandomCoordinatesWithoutBlocker() {
        Vector2 coordinates = getRandomCoordinates();
        while (blockers.Find(blocker => blocker.transform.position.Equals(coordinates)) != null)
            coordinates = getRandomCoordinates();
        return coordinates;
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

    public static Vector2 getIndicesFor(float posX, float posY){
        Vector2 pos = new Vector2(posX, posY);
        if (posY % 2 != 0)
            pos.x -= 0.5f;
        pos.y = posY / 0.75f;
        return pos;
    }

    public static Vector2 getIndicesFor(Vector2 pos){
        return getIndicesFor(pos.x, pos.y);
    }
}
