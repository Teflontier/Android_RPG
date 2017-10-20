using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : Clickable {

    public enum EntityState {
        INITIALIZE,
        SHOW_POSSIBLE_MOVES,
        WAIT_FOR_ACTION,
        CREATE_COMMAND_MENU,
        WAIT_FOR_MENU_SELECTION,
        CALCULATE_MOVEMENT_FIELDS,
        MOVE,
        END_TURN
    }

    public int maxHp;
    public int maxMoves = 3;

    public EntityState state = EntityState.INITIALIZE;
    public GameObject clickedObject;

    protected int hp;
    protected int moves;
    protected Dictionary<Tile, KeyValuePair<Tile, int>> movableTiles = new Dictionary<Tile, KeyValuePair<Tile, int>>();

    protected DDOL ddol;
    protected LevelManager levelManager;
    protected CommandMenu commandMenu;

    public override void Awake() {
        base.Awake();
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

        if (moves == 0)
            return;

        Vector2 startingPos = LevelManager.getIndicesFor(transform.position);
        Tile startingTile = levelManager.tileMatrix[(int)startingPos.x, (int)startingPos.y];
        tilesToCheck.AddRange(getSurroundingMovableTilesNotMarked(moves, startingTile));

        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            Vector2 tilePos = tileToCheck.transform.position;
            tilesToCheck.RemoveAt(0);
            int movesLeft = movableTiles[tileToCheck].Value - 1;
            if (movesLeft > 0)
                tilesToCheck.AddRange(getSurroundingMovableTilesNotMarked(movesLeft, tileToCheck));
        }
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in movableTiles)
            levelManager.createOverlay(ddol.greenOverlay, pair.Key.transform.position);
    }

    private List<Tile> getSurroundingMovableTilesNotMarked(int movesLeft, Tile centerTile) {
        List<Tile> surroundingTiles = new List<Tile>();
        centerTile.getSurroundingTiles().FindAll(tile => isMovable(tile) && !movableTiles.ContainsKey(tile)).ForEach(tile => {
                surroundingTiles.Add(tile);
                movableTiles.Add(tile, new KeyValuePair<Tile, int>(centerTile, movesLeft));
            });
        return surroundingTiles;
    }

    private bool isMovable(Tile tile) {
        if (tile == null)
            return false;
        Vector2 pos = tile.transform.position;
        return getGameObjectOnTile(tile) == null;
    }

    public GameObject getGameObjectOnTile(Tile tile) {
        Blocker blocker = levelManager.blockers.Find(b => b.transform.position.Equals(tile.transform.position));
        if (blocker != null)
            return blocker.gameObject;
        Entity entity = levelManager.entities.Find(e => e.transform.position.Equals(tile.transform.position));
        if (entity != null)
            return entity.gameObject;
        return null;
    }
}
