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
        SHOW_SKILLS,
        END_TURN
    }

    public int maxHp = 3;
    public int maxMoves = 3;
    public int attackRange = 1;
    public int maxAttacks = 2;

    public EntityState state = EntityState.INITIALIZE;
    public GameObject clickedObject;
    public Skill[] skills;

    [SerializeField] protected int hp = 0;
    protected int moves = 0;
    protected int attacks = 0;
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
        skills = GetComponents<Skill>();
    }

    public void calcPossibleActions() {
        movableTiles.Clear();
        attackableTiles.Clear();

        Vector2 startingPos = LevelManager.getIndicesFor(transform.position);
        Tile startingTile = levelManager.tileMatrix[(int)startingPos.x, (int)startingPos.y];

        Dictionary<Tile, KeyValuePair<Tile, int>> floodFilledTiles = TileUtilities.floodFill(startingTile, moves, tile => levelManager.isMovable(tile));
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in floodFilledTiles)
            movableTiles.Add(pair.Key, pair.Value);

        if (attacks > 0) {
            floodFilledTiles = TileUtilities.floodFill(startingTile, attackRange, tile => true);
            foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in floodFilledTiles)
                if (levelManager.isAttackable(pair.Key) && !movableTiles.ContainsKey(pair.Key))
                    attackableTiles.Add(pair.Key, pair.Value);
        }

        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in movableTiles) {
            Tile key = pair.Key;
            if (!key.transform.position.Equals(startingTile.transform.position))
                levelManager.createOverlay(ddol.greenOverlay, key.transform.position);
        }
        foreach (KeyValuePair<Tile, KeyValuePair<Tile, int>> pair in attackableTiles)
            if (!pair.Key.transform.position.Equals(startingTile.transform.position))
                levelManager.createOverlay(ddol.redOverlay, pair.Key.transform.position);
    }

    public void increaseHp(int increment) {
        hp += increment;
        if (hp > 0)
            return;
        gameManager.destroyEntity(this);
    }
}
