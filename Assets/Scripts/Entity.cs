using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    protected Dictionary<Tile, KeyValuePair<Tile, int>> floodFilledTiles = new Dictionary<Tile, KeyValuePair<Tile, int>>();
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
        attacks = maxAttacks;
    }

    public void calcPossibleActions() {
        movableTiles.Clear();
        attackableTiles.Clear();

        Vector2 startingPos = LevelManager.getIndicesFor(transform.position);
        Tile startingTile = levelManager.tileMatrix[(int)startingPos.x, (int)startingPos.y];

        floodFill(startingTile, moves, tile => levelManager.isMovable(tile));
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in floodFilledTiles)
            movableTiles.Add(pair.Key, pair.Value);

        if (attacks > 0) {
            floodFill(startingTile, attackRange, tile => true);
            foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in floodFilledTiles)
                if (levelManager.isAttackable(pair.Key))
                    attackableTiles.Add(pair.Key, pair.Value);
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

    public void floodFill(Tile startingTile, int searchDepth, Predicate<Tile> filter) {
        floodFilledTiles.Clear();
        List<Tile> tilesToCheck = new List<Tile>();
        tilesToCheck.Add(startingTile);

        while (tilesToCheck.Count > 0) {
            Tile tileToCheck = tilesToCheck[0];
            tilesToCheck.RemoveAt(0);
            if (tileToCheck != startingTile)
                searchDepth = floodFilledTiles[tileToCheck].Value - 1;
            if (searchDepth > 0)
                tilesToCheck.AddRange(getAdjacentTilesFiltered(searchDepth, tileToCheck, filter));
        }
    }

    public List<Tile> getAdjacentTilesFiltered(int searchDepth, Tile tile, Predicate<Tile> filter) {
        List<Tile> surroundingTiles = new List<Tile>();
        tile.getSurroundingTiles().FindAll(t => (!floodFilledTiles.ContainsKey(t) && filter(t))).ForEach(adjacentTile => {
                surroundingTiles.Add(adjacentTile);
                floodFilledTiles.Add(adjacentTile, new KeyValuePair<Tile, int>(tile, searchDepth));
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
