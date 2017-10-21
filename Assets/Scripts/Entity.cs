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
        MOVE_ENDED,
        ATTACK,
        END_TURN
    }

    public int maxHp = 3;
    public int maxMoves = 3;

    public EntityState state = EntityState.INITIALIZE;
    public GameObject clickedObject;

    [SerializeField] protected int hp = 3;
    protected int moves;
    protected Dictionary<Tile, KeyValuePair<Tile, int>> movableTiles = new Dictionary<Tile, KeyValuePair<Tile, int>>();
    protected Dictionary<Tile, KeyValuePair<Tile, int>> attackableTiles = new Dictionary<Tile, KeyValuePair<Tile, int>>();

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
        attackableTiles.Clear();
        if (moves == 0)
            return;
        Vector2 startingPos = LevelManager.getIndicesFor(transform.position);
        Tile startingTile = levelManager.tileMatrix[(int)startingPos.x, (int)startingPos.y];
        List<Tile> tilesToCheck = new List<Tile>();
        tilesToCheck.AddRange(getSurroundingTilesNotMarked(moves, startingTile));

        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            Vector2 tilePos = tileToCheck.transform.position;
            tilesToCheck.RemoveAt(0);
            int movesLeft = movableTiles[tileToCheck].Value - 1;
            if (movesLeft > 0)
                tilesToCheck.AddRange(getSurroundingTilesNotMarked(movesLeft, tileToCheck));
        }
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in movableTiles)
            levelManager.createOverlay(ddol.greenOverlay, pair.Key.transform.position);
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in attackableTiles)
            levelManager.createOverlay(ddol.redOverlay, pair.Key.transform.position);
    }

    private List<Tile> getSurroundingTilesNotMarked(int movesLeft, Tile centerTile) {
        List<Tile> surroundingTiles = new List<Tile>();
        centerTile.getSurroundingTiles().FindAll(tile => levelManager.isMovable(tile) && !movableTiles.ContainsKey(tile) && !attackableTiles.ContainsKey(tile)).ForEach(tile => {
                surroundingTiles.Add(tile);
                movableTiles.Add(tile, new KeyValuePair<Tile, int>(centerTile, movesLeft));
            });
        centerTile.getSurroundingTiles().FindAll(tile => levelManager.isAttackable(tile) && !movableTiles.ContainsKey(tile) && !attackableTiles.ContainsKey(tile)).ForEach(tile => {
                attackableTiles.Add(tile, new KeyValuePair<Tile, int>(centerTile, movesLeft));
            });
        return surroundingTiles;
    }

    public void increaseHp(int increment) {
        hp += increment;
        if (hp > 0)
            return;
        gameManager.destroyEntity(this);
    }
}
