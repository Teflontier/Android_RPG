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
        CALCULATE_ACTION_FIELDS,
        MOVE,
        MOVE_ENDED,
        ATTACK,
        END_TURN
    }

    public int maxHp = 3;
    public int maxMoves = 3;
    public int attackRange = 1;

    public EntityState state = EntityState.INITIALIZE;
    public GameObject clickedObject;

    [SerializeField] protected int hp = 3;
    protected int moves = 3;
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

    public void calcPossibleActions() {
        movableTiles.Clear();
        attackableTiles.Clear();

        Vector2 startingPos = LevelManager.getIndicesFor(transform.position);
        Tile startingTile = levelManager.tileMatrix[(int)startingPos.x, (int)startingPos.y];

        calcMovementFields(startingTile);
        calcAttackFields(startingTile);

        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in movableTiles)
            if (!pair.Key.transform.position.Equals(startingTile.transform.position))
                levelManager.createOverlay(ddol.greenOverlay, pair.Key.transform.position);
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in attackableTiles)
            if (!pair.Key.transform.position.Equals(startingTile.transform.position))
                levelManager.createOverlay(ddol.redOverlay, pair.Key.transform.position);
    }

    private void calcMovementFields(Tile startingTile) {
        List<Tile> tilesToCheck = new List<Tile>();
        tilesToCheck.Add(startingTile);

        int movesLeft = moves;
        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            tilesToCheck.RemoveAt(0);
            if (tileToCheck != startingTile)
                movesLeft = movableTiles[tileToCheck].Value - 1;
            if (movesLeft > 0)
                tilesToCheck.AddRange(getSurroundingMovementTilesNotMarked(movesLeft, tileToCheck));
        }
    }

    private List<Tile> getSurroundingMovementTilesNotMarked(int movesLeft, Tile centerTile) {
        List<Tile> surroundingTiles = new List<Tile>();
        centerTile.getSurroundingTiles().FindAll(tile => levelManager.isMovable(tile) && !movableTiles.ContainsKey(tile)).ForEach(tile => {
                surroundingTiles.Add(tile);
                movableTiles.Add(tile, new KeyValuePair<Tile, int>(centerTile, movesLeft));
            });
        return surroundingTiles;
    }

    private void calcAttackFields(Tile startingTile) {
        List<Tile> tilesToCheck = new List<Tile>();
        tilesToCheck.Add(startingTile);
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in movableTiles)
            tilesToCheck.Add(pair.Key);

        int attackRangeLeft = attackRange;
        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            tilesToCheck.RemoveAt(0);
            if (movableTiles.ContainsKey(tileToCheck))
                attackRangeLeft = attackRange;
            if (attackableTiles.ContainsKey(tileToCheck))
                attackRangeLeft = attackableTiles[tileToCheck].Value - 1;
            if (attackRangeLeft > 0)
                tilesToCheck.AddRange(getSurroundingAttackTilesNotMarked(attackRangeLeft, tileToCheck));
        }
    }

    private List<Tile> getSurroundingAttackTilesNotMarked(int rangeLeft, Tile centerTile) {
        List<Tile> surroundingTiles = new List<Tile>();
        centerTile.getSurroundingTiles().FindAll(tile => !movableTiles.ContainsKey(tile) && !attackableTiles.ContainsKey(tile)).ForEach(tile => {
                surroundingTiles.Add(tile);
                attackableTiles.Add(tile, new KeyValuePair<Tile, int>(centerTile, rangeLeft));
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
