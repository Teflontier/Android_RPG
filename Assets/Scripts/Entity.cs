using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

    public enum EntityState {
        INITIALIZE,
        SHOW_POSSIBLE_MOVES,
        CHOOSE_ACTION,
        MOVEMENT,
        END_TURN
    }

    public int maxHp;
    public int maxMoves = 3;
    public int x;
    public int y;

    public EntityState state = EntityState.INITIALIZE;
    public GameObject clickedObject;

    protected int hp;
    protected int moves;

    protected DDOL ddol;
    protected LevelManager levelManager;
    protected GameManager gameManager;

    public void Awake() {
        ddol = GameObject.FindObjectOfType<DDOL>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    public abstract bool act();

    public void initialize() {
        moves = maxMoves;
    }

    public void showPossibleMoves() {
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(transform.position, moves + 0.5f);
        List<Blocker> blockersInRange = new List<Blocker>();
        List<Player> playersInRange = new List<Player>();
        List<Mob> mobsInRange = new List<Mob>();
        List<Tile> tilesInRange = new List<Tile>();
        foreach (Collider2D collider in collidersInRange) {
            Blocker blocker = collider.GetComponent<Blocker>();
            if (blocker != null)
                blockersInRange.Add(blocker);
            Player player = collider.GetComponent<Player>();
            if (player != null)
                playersInRange.Add(player);
            Mob mob = collider.GetComponent<Mob>();
            if (mob != null)
                mobsInRange.Add(mob);
            Tile tile = collider.GetComponent<Tile>();
            if (tile != null)
                tilesInRange.Add(tile);
        }

        List<Vector3> nonWalkablePositions = new List<Vector3>();
        nonWalkablePositions.AddRange(blockersInRange.ConvertAll(blocker => blocker.transform.position));
        nonWalkablePositions.AddRange(playersInRange.ConvertAll(player => player.transform.position));
        nonWalkablePositions.AddRange(mobsInRange.ConvertAll(mob => mob.transform.position));

        List<Tile> filteredTiles = new List<Tile>();
        tilesInRange.ForEach(tile => {
                if (!nonWalkablePositions.Contains(tile.transform.position))
                    filteredTiles.Add(tile);
            });

        List<Vector2> validPositions = new List<Vector2>();
        List<Tile> possibleMoves = new List<Tile>();
        validPositions.Add(transform.position);
        createOverlaysRecursive(validPositions, filteredTiles, possibleMoves, moves);
    }

    private void createOverlaysRecursive(List<Vector2> validPositions, List<Tile> filteredTiles, List<Tile> possibleMoves, int moves) {
        if (moves <= 0)
            return;
        List<Vector2> newPositions = new List<Vector2>();
        foreach (Vector2 position in validPositions) {
            float x = position.x;
            float y = position.y;
            createOverlayIfNeededAndFillLists(new Vector2(x - 1, y), filteredTiles, possibleMoves, newPositions);
            createOverlayIfNeededAndFillLists(new Vector2(x + 1, y), filteredTiles, possibleMoves, newPositions);
            createOverlayIfNeededAndFillLists(new Vector2(x, y - 1), filteredTiles, possibleMoves, newPositions);
            createOverlayIfNeededAndFillLists(new Vector2(x, y + 1), filteredTiles, possibleMoves, newPositions);
        }
        createOverlaysRecursive(newPositions, filteredTiles, possibleMoves, moves - 1);
    }

    private void createOverlayIfNeededAndFillLists(Vector2 position, List<Tile> filteredTiles, List<Tile> possibleMoves, List<Vector2> newPositions) {
        Tile tile = filteredTiles.Find(filteredTile => ((Vector2)filteredTile.transform.position).Equals(position) && !possibleMoves.Contains(filteredTile));
        if (tile == null)
            return;
        levelManager.createOverlay(ddol.greenOverlay, tile.transform.position);
        possibleMoves.Add(tile);
        newPositions.Add(tile.transform.position);
    }

    public void checkIfSomethingClicked() {

    }

    protected bool tileWasClicked() {
        return false;
    }

    protected bool endTurnWasClicked() {
        return false;
    }

    public bool animate() {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = new Vector2(x, y);
        if (currentPosition != targetPosition) {
            if (Vector2.Distance(currentPosition, targetPosition) < 0.03f)
                transform.position = targetPosition;
            else
                transform.position = Vector2.Lerp(currentPosition, targetPosition, 0.1f);
            return false;
        }
        return true;
    }

    public void OnMouseDown(){
        gameManager.objectWasClicked(gameObject);
    }
}
