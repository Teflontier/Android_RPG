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
        USING_SKILL,
        END_TURN
    }

    public int attackRange = 1;

    public float attackDamage
    {
        get { 
            float temp = attackDamageBase + attackDamageModifiersAbsolute;
            return temp + temp * attackDamageModifiersPercentage; 
        }
    }

    public int attacks{ get { return attackCountCurrent + attackCountExtra; } }

    public int maxHp = 3;
    public float hp = 0;

    public int maxMoves = 3;
    public int moves = 0;

    public float attackDamageBase = 1f;
    public float attackDamageModifiersAbsolute = 0f;
    public float attackDamageModifiersPercentage = 0f;

    public int attackCountBase = 2;
    public int attackCountExtra = 0;
    public int attackCountCurrent = 0;

    public bool enemyKilled = false;

    public EntityState state = EntityState.INITIALIZE;
    public GameObject clickedObject;
    public Skill[] skills;

    [HideInInspector]public SpriteRenderer spriteRenderer;

    protected Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> movableTiles = new Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>>();
    protected Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> attackableTiles = new Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>>();

    protected DDOL ddol;
    protected LevelManager levelManager;
    protected CommandMenu commandMenu;

    public override void Awake() {
        base.Awake();
        ddol = GameObject.FindObjectOfType<DDOL>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        commandMenu = GameObject.FindObjectOfType<CommandMenu>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public abstract bool act();

    public void initialize() {
        moves = maxMoves;
        attackCountCurrent = attackCountBase;
        skills = GetComponents<Skill>();
        changeModifiersAndRecalculate(modifier => {
                if (modifier.runTime > 0)
                    modifier.runTime--;
            });
    }

    public void changeModifiersAndRecalculate(Action<Modifier> change) {
        List<Modifier> modifierExclusionList = new List<Modifier>();
        foreach (Modifier modifier in gameObject.GetComponents<Modifier>()) {
            change(modifier);
            if (!modifier.checkAliveConditions()) {
                Destroy(modifier);
                modifierExclusionList.Add(modifier);
            }
        }
        recalculateModifiers(modifierExclusionList);
    }

    public void calcPossibleActions() {
        movableTiles.Clear();
        attackableTiles.Clear();

        Tile startingTile = levelManager.getTileForPosition(transform.position);
         
        Dictionary<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> floodFilledTiles = TileUtilities.floodFill<int>(startingTile, moves, adjacentTilesFilter: tile => levelManager.isMovable(tile));
        foreach (KeyValuePair<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> pair in floodFilledTiles)
            movableTiles.Add(pair.Key, pair.Value);

        if (attacks > 0) {
            floodFilledTiles = TileUtilities.floodFill<int>(startingTile, attackRange);
            foreach (KeyValuePair<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> pair in floodFilledTiles)
                if (levelManager.isAttackable(pair.Key.tile) && !movableTiles.ContainsKey(pair.Key))
                    attackableTiles.Add(pair.Key, pair.Value);
        }

        foreach (KeyValuePair<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> pair in movableTiles) {
            Tile key = pair.Key.tile;
            if (!key.transform.position.Equals(startingTile.transform.position))
                levelManager.createOverlay(ddol.greenOverlay, key.transform.position);
        }
        foreach (KeyValuePair<WrappedTile<int>, KeyValuePair<WrappedTile<int>, int>> pair in attackableTiles)
            if (!pair.Key.tile.transform.position.Equals(startingTile.transform.position))
                levelManager.createOverlay(ddol.redOverlay, pair.Key.tile.transform.position);
    }

    public void increaseHp(float increment) {
        hp += increment;
        recalculateModifiers();
        if (hp > 0)
            return;
        gameManager.destroyEntity(this);
    }

    public void recalculateModifiers(List<Modifier> exclusionList = null) {
        attackDamageModifiersAbsolute = 0;
        attackDamageModifiersPercentage = 0;
        foreach (Modifier modifier in gameObject.GetComponents<Modifier>())
            if (exclusionList == null || exclusionList != null && !exclusionList.Contains(modifier))
                modifier.modify();
    }
}
