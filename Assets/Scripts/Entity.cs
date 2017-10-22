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
    public int maxAttacks = 2;

    public EntityState state = EntityState.INITIALIZE;
    public GameObject clickedObject;

    [SerializeField] protected int hp = 0;
    protected int moves = 0;
    protected int attacks = 0;
    protected Dictionary<Tile, KeyValuePair<Tile, int>> movableTiles = new Dictionary<Tile, KeyValuePair<Tile, int>>();
    protected Dictionary<Tile, KeyValuePair<Tile, int>> attackableTiles = new Dictionary<Tile, KeyValuePair<Tile, int>>();

    protected DDOL ddol;
    protected LevelManager levelManager;
    protected CommandMenu commandMenu;

    private int maxScanRange;

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
        attacks = maxAttacks;
    }

    public void calcPossibleActions() {
        movableTiles.Clear();
        attackableTiles.Clear();

        Vector2 startingPos = LevelManager.getIndicesFor(transform.position);
        Tile startingTile = levelManager.tileMatrix[(int)startingPos.x, (int)startingPos.y];

        List<Tile> tilesToCheck = new List<Tile>();
        tilesToCheck.Add(startingTile);

        maxScanRange = Mathf.Max(moves, attackRange);
        int scanRange = maxScanRange;
        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            tilesToCheck.RemoveAt(0);
            if (tileToCheck != startingTile)
                scanRange = movableTiles[tileToCheck].Value - 1;
            if (scanRange > 0)
                tilesToCheck.AddRange(getSurroundingTilesNotMarked(scanRange, tileToCheck));
        }

        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in movableTiles) {
            Tile key = pair.Key;
            if (!key.transform.position.Equals(startingTile.transform.position))
                levelManager.createOverlay(ddol.greenOverlay, key.transform.position);
        }
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in attackableTiles)
            if (!pair.Key.transform.position.Equals(startingTile.transform.position) && !movableTiles.ContainsKey(pair.Key))
                levelManager.createOverlay(ddol.redOverlay, pair.Key.transform.position);
    }

    private List<Tile> getSurroundingTilesNotMarked(int movesLeft, Tile centerTile) {
        List<Tile> surroundingTiles = new List<Tile>();
        centerTile.getSurroundingTiles().FindAll(tile => !movableTiles.ContainsKey(tile)).ForEach(tile => {
            print(maxScanRange + " " + movesLeft + " " + moves);
            if (levelManager.isMovable(tile) && maxScanRange - movesLeft <= moves) {
                    surroundingTiles.Add(tile);
                    if (attackableTiles.ContainsKey(tile))
                        attackableTiles.Remove(tile);
                    movableTiles.Add(tile, new KeyValuePair<Tile, int>(centerTile, movesLeft));
                }
                else if (!attackableTiles.ContainsKey(tile) && levelManager.isAttackable(tile) && moves - movesLeft < attackRange && attacks > 0)
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
