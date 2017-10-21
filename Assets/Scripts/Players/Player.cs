using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {
    private const string MOVE = "Move";
    private const string ATTACK = "Attack";
    private const string END = "End";

    private Tile tileToUseActionOn;
    private GameObject lastMenuClicked;
    private List<Tile> tilesToMove = new List<Tile>();

    public override bool act() {
        switch (state) {
            case EntityState.INITIALIZE:
                initialize();
                state = EntityState.SHOW_POSSIBLE_MOVES;
                break;
            case EntityState.SHOW_POSSIBLE_MOVES:
                calcPossibleMoves();
                state = EntityState.WAIT_FOR_ACTION;
                break;
            case EntityState.WAIT_FOR_ACTION:
                handleWaitForAction();
                break;
            case EntityState.CREATE_COMMAND_MENU:
                handleCreateCommandMenu();
                break;
            case EntityState.WAIT_FOR_MENU_SELECTION:
                handleWaitForMenuSelection();
                break;
            case EntityState.CALCULATE_MOVEMENT_FIELDS:
                handleCalculateMovementFields();
                break;
            case EntityState.MOVE:
                handleMove();
                break;
            case EntityState.MOVE_ENDED:
                handleMoveEnded();
                break;
            case EntityState.ATTACK:
                handleAttack();
                break;
            case EntityState.END_TURN:
                commandMenu.setVisibility(false);
                return true;
        }
        return false;
    }

    private void handleWaitForAction() {
        if (clickedObject == null)
            return;
        tileToUseActionOn = clickedObject.GetComponent<Tile>();
        clickedObject = null;
        state = EntityState.CREATE_COMMAND_MENU;
    }

    private void handleCreateCommandMenu() {
        commandMenu.setVisibility(false);
        Vector2 clickedPos = tileToUseActionOn.gameObject.transform.position;
        commandMenu.transform.position = new Vector2(clickedPos.x, clickedPos.y + 1);
        Tile tile = tileToUseActionOn;
        if (tile != null && movableTiles.ContainsKey(tile))
            commandMenu.setMoveVisibility(true);
        if (tile != null && attackableTiles.ContainsKey(tile))
            commandMenu.setAttackVisibility(true);
        commandMenu.setEndVisibility(true);
        state = EntityState.WAIT_FOR_MENU_SELECTION;
    }

    private void handleWaitForMenuSelection() {
        if (clickedObject == null)
            return;
        if (clickedObject.GetComponent<Tile>() != null) {
            handleWaitForAction();
            return;
        }
        lastMenuClicked = clickedObject;
        clickedObject = null;
        if (lastMenuClicked.name.Equals(END)) {
            levelManager.destroyOverlays();
            commandMenu.setVisibility(false);
            state = EntityState.END_TURN;
            return;
        }
        if (lastMenuClicked.name.Equals(MOVE) || lastMenuClicked.name.Equals(ATTACK)) {
            levelManager.destroyOverlays();
            commandMenu.setVisibility(false);
            state = EntityState.CALCULATE_MOVEMENT_FIELDS;
            return;
        }
    }

    private void handleCalculateMovementFields() {
        Tile tile = tileToUseActionOn;
        tilesToMove.Clear();
        if (lastMenuClicked.name.Equals(MOVE))
            tilesToMove.Add(tile);
        if (lastMenuClicked.name.Equals(ATTACK)) {
            tile = attackableTiles[tile].Key;
            tilesToMove.Add(tile);
        }
        while (movableTiles.ContainsKey(tile)) {
            tile = movableTiles[tile].Key;
            tilesToMove.Insert(0, tile);
        }
        tilesToMove.RemoveAt(0);
        state = EntityState.MOVE;
    }

    private void handleMove() {
        if (tilesToMove.Count == 0) {
            state = EntityState.MOVE_ENDED;
            return;
        }
        Vector2 tileToMoveToPos = tilesToMove[0].transform.position;
        if (Vector2.Distance(transform.position, tileToMoveToPos) > 0.05f) {
            transform.position = Vector2.MoveTowards(transform.position, tileToMoveToPos, 3f * Time.deltaTime);
            return;
        }
        transform.position = tileToMoveToPos;
        moves--;
        tilesToMove.RemoveAt(0);
    }

    private void handleMoveEnded() {
        switch (lastMenuClicked.name) {
            case MOVE:
                tileToUseActionOn = null;
                state = EntityState.SHOW_POSSIBLE_MOVES;
                break;
            case ATTACK:
                state = EntityState.ATTACK;
                break;
        }
        lastMenuClicked = null;
    }

    private void handleAttack(){
        Mob mob = levelManager.getGameObjectOnTile(tileToUseActionOn).GetComponent<Mob>();
        mob.increaseHp(-1);
        state = EntityState.SHOW_POSSIBLE_MOVES;
    }
}
