using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    private GameObject objectToUseActionOn;
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
                clickedObject = null;
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

        objectToUseActionOn = clickedObject;
        clickedObject = null;
        state = EntityState.CREATE_COMMAND_MENU;
    }

    private void handleCreateCommandMenu() {
        commandMenu.setVisibility(false);
        Vector2 clickedPos = objectToUseActionOn.transform.position;
        commandMenu.transform.position = new Vector2(clickedPos.x, clickedPos.y + 1);
        Tile tile = objectToUseActionOn.GetComponent<Tile>();
        if (tile != null && movableTiles.ContainsKey(tile))
            commandMenu.setMoveVisibility(true);
        commandMenu.setEndVisibility(true);
        state = EntityState.WAIT_FOR_MENU_SELECTION;
    }

    private void handleWaitForMenuSelection() {
        if (clickedObject == null)
            return;
        if (clickedObject.name.Equals("End")) {
            clickedObject = null;
            levelManager.destroyOverlays();
            commandMenu.setVisibility(false);
            state = EntityState.END_TURN;
            return;
        }
        if (clickedObject.name.Equals("Move")) {
            clickedObject = null;
            levelManager.destroyOverlays();
            commandMenu.setVisibility(false);
            state = EntityState.CALCULATE_MOVEMENT_FIELDS;
            return;
        }
    }

    private void handleCalculateMovementFields() {
        Tile tile = objectToUseActionOn.GetComponent<Tile>();
        tilesToMove.Clear();
        objectToUseActionOn = null;

        tilesToMove.Add(tile);
        while (movableTiles.ContainsKey(tile)) {
            tile = movableTiles[tile].Key;
            tilesToMove.Insert(0, tile);
        }
        tilesToMove.RemoveAt(0);
        state = EntityState.MOVE;
    }

    private void handleMove() {
        if (tilesToMove.Count == 0) {
            state = EntityState.SHOW_POSSIBLE_MOVES;
            return;
        }
        Vector2 tileToMoveToPos = tilesToMove[0].transform.position;
        if (Vector2.Distance(transform.position, tileToMoveToPos) <= 0.05f) {
            transform.position = tileToMoveToPos;
            moves--;
            tilesToMove.RemoveAt(0);
        }
        else {
            transform.position = Vector2.MoveTowards(transform.position, tileToMoveToPos, 3f * Time.deltaTime);
        }
    }
}
