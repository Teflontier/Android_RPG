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
        for (int i = 0; i < width; i++) {
            for (int k = 0; k < height; k++) {
                Vector2 pos = new Vector2(i, k);
                if(k % 2 != 0){
                    pos.x += 0.5f;
                }
                pos.y -= 0.25f * k;
                Tile tile = GameObject.Instantiate(ddol.tiles[Random.Range(0, ddol.tiles.Count)], pos, Quaternion.identity, tileObjectHolder) as Tile;
                tiles.Add(tile);
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
        return new Vector2(Random.Range(0, width), Random.Range(0, height));
    }

    private Vector2 getRandomCoordinatesWithoutBlocker() {
        Vector2 coordinates = getRandomCoordinates();
        while (blockers.Find(blocker => blocker.transform.position.Equals(coordinates)) != null)
            coordinates = getRandomCoordinates();
        return coordinates;
    }

    public Tile getTileAtPosition(int x, int y) {
        return tiles.Find(tile => ((Vector2)tile.transform.position).Equals(new Vector2(x, y)));
    }

    public void createOverlay(GameObject overlay, Vector2 position) {
        GameObject.Instantiate(overlay, position, Quaternion.identity, overlayObjectHolder);
    }

    public void destroyOverlays() {
        foreach (Transform child in overlayObjectHolder)
            Destroy(child.gameObject);
    }
}
