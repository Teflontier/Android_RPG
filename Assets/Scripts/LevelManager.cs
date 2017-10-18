using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static int width = 8;
    public static int height = 8;

    public int minBlockers = 5;
    public int maxBlockers = 9;

    private DDOL ddol;
    private Tile[,] tiles = new Tile[width, height];
    private Blocker[,] blockers = new Blocker[width, height];
    private Entity[,] entities = new Entity[width, height];

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
        for (int i = 0; i < width; i++) {
            for (int k = 0; k < height; k++) {
                tiles[i, k] = null;
                blockers[i, k] = null;
                entities[i, k] = null;
            }
        }
    }

    private void createTiles() {
        for (int i = 0; i < width; i++) {
            for (int k = 0; k < height; k++) {
                Tile tile = GameObject.Instantiate(ddol.tiles[Random.Range(0, ddol.tiles.Count)], new Vector2(i, k), Quaternion.identity, tileObjectHolder) as Tile;
                tiles[i, k] = tile;
            }
        }
    }

    private void createBlockers() {
        for (int i = 0; i < Random.Range(minBlockers, maxBlockers); i++) {
            Vector2 coordinates = getRandomCoordinatesWithoutBlocker();
            Blocker blocker = GameObject.Instantiate(ddol.blockers[Random.Range(0, ddol.blockers.Count)], coordinates, Quaternion.identity, blockerObjectHolder) as Blocker;
            blockers[(int)coordinates.x, (int)coordinates.y] = blocker;
        }
    }

    private void createPlayers(List<Player> playersToPlace) {
        foreach (Player player in playersToPlace) {
            player.transform.SetParent(playerObjectHolder);
            Vector2 coordinates = getRandomCoordinatesWithoutBlocker();
            teleportEntity(player, (int)coordinates.x, (int)coordinates.y);
        }
    }

    private Vector2 getRandomCoordinates() {
        return new Vector2(Random.Range(0, width), Random.Range(0, height));
    }

    private Vector2 getRandomCoordinatesWithoutBlocker() {
        Vector2 coordinates = getRandomCoordinates();
        while (blockers[(int)coordinates.x, (int)coordinates.y] != null)
            coordinates = getRandomCoordinates();
        return coordinates;
    }

    public bool moveEntity(Entity entity, int x, int y) {
        if (x < 0 || x > width - 1 || y < 0 || y > height - 1)
            return false;
        if (blockers[x, y] != null || entities[x, y] != null)
            return false;
        entities[entity.x, entity.y] = null;
        entities[x, y] = entity;
        entity.x = x;
        entity.y = y;
        return true;
    }

    public bool teleportEntity(Entity entity, int x, int y) {
        if (x < 0 || x > width - 1 || y < 0 || y > height - 1)
            return false;
        if (blockers[x, y] != null || entities[x, y] != null)
            return false;
        entities[entity.x, entity.y] = null;
        entities[x, y] = entity;
        entity.x = x;
        entity.y = y;
        entity.transform.position = new Vector2(entity.x, entity.y);
        return true;
    }

    public void createOverlay(GameObject overlay, Vector2 position){
        GameObject.Instantiate(overlay, position, Quaternion.identity, overlayObjectHolder);
    }
}
