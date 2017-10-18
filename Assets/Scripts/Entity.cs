using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {

    public enum EntityState {
        INITIALIZE,
        SHOW_POSSIBLE_MOVES,
        WAIT_FOR_ACTION,
        CREATE_COMMAND_MENU,
        WAIT_FOR_MENU_SELECTION,
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
    protected Dictionary<Tile, int> movableTiles = new Dictionary<Tile, int>();

    protected DDOL ddol;
    protected LevelManager levelManager;
    protected GameManager gameManager;
    protected CommandMenu commandMenu;

    public void Awake() {
        ddol = GameObject.FindObjectOfType<DDOL>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        commandMenu = GameObject.FindObjectOfType<CommandMenu>();
    }

    public abstract bool act();

    public void initialize() {
        moves = maxMoves;
    }

    public void calcPossibleMoves() {
        movableTiles.Clear();
        List<Tile> tilesToCheck = new List<Tile>();
        tilesToCheck.AddRange(getSurroundingMovableTilesNotMarked(x, y, moves));

        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            Vector2 tilePos = tileToCheck.transform.position;
            tilesToCheck.RemoveAt(0);
            int movesLeft = movableTiles[tileToCheck] - 1;
            if (movesLeft > 0)
                tilesToCheck.AddRange(getSurroundingMovableTilesNotMarked((int)tilePos.x, (int)tilePos.y, movesLeft));
        }

        foreach (KeyValuePair<Tile, int> pair in movableTiles)
            levelManager.createOverlay(ddol.greenOverlay, pair.Key.transform.position);
    }

    private List<Tile> getSurroundingMovableTilesNotMarked(int x, int y, int movesLeft) {
        List<Tile> surroundingTiles = new List<Tile>();
        Tile temp;
        if (x - 1 >= 0 && isMovable(temp = levelManager.tiles[x - 1, y]) && !movableTiles.ContainsKey(temp)) {
            surroundingTiles.Add(temp);
            movableTiles.Add(temp, movesLeft);
        }
        if (x + 1 < levelManager.tiles.GetLength(0) && isMovable(temp = levelManager.tiles[x + 1, y]) && !movableTiles.ContainsKey(temp)) {
            surroundingTiles.Add(temp);
            movableTiles.Add(temp, movesLeft);
        }
        if (y - 1 >= 0 && isMovable(temp = levelManager.tiles[x, y - 1]) && !movableTiles.ContainsKey(temp)) {
            surroundingTiles.Add(temp);
            movableTiles.Add(temp, movesLeft);
        }
        if (y + 1 < levelManager.tiles.GetLength(1) && isMovable(temp = levelManager.tiles[x, y + 1]) && !movableTiles.ContainsKey(temp)) {
            surroundingTiles.Add(temp);
            movableTiles.Add(temp, movesLeft);
        }
        return surroundingTiles;
    }

    private bool isMovable(Tile tile) {
        Vector2 pos = tile.transform.position;
        return levelManager.blockers[(int)pos.x, (int)pos.y] == null && levelManager.entities[(int)pos.x, (int)pos.y] == null;
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

    public void OnMouseDown() {
        gameManager.objectWasClicked(gameObject);
    }
}
