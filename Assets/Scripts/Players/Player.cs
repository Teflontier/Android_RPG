using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    private GameObject objectToUseActionOn;

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
            case EntityState.MOVEMENT:
                handleMovement();
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
        End endButton = clickedObject.GetComponent<End>();
        if (endButton != null) {
            clickedObject = null;
            levelManager.destroyOverlays();
            commandMenu.setVisibility(false);
            state = EntityState.END_TURN;
            return;
        }
        Move moveButton = clickedObject.GetComponent<Move>();
        if (moveButton != null) {
            clickedObject = null;
            levelManager.destroyOverlays();
            commandMenu.setVisibility(false);
            state = EntityState.MOVEMENT;
            return;
        }
    }

    private bool handleMovement() {
        print(objectToUseActionOn.transform.position);
        state = EntityState.SHOW_POSSIBLE_MOVES;

        // state movement 
        // move player to target position
        // subtract moves from actions
        // switch state to choose action
        return true;
    }
}
