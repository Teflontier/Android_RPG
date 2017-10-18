using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

    public override bool act() {
        switch (state) {
            case EntityState.INITIALIZE:
                initialize();
                state = EntityState.SHOW_POSSIBLE_MOVES;
                break;
            case EntityState.SHOW_POSSIBLE_MOVES:
                calcPossibleMoves();
                state = EntityState.CHOOSE_ACTION;
                break;
            case EntityState.CHOOSE_ACTION:
                handleChooseAction();
                break;
            case EntityState.MOVEMENT:
                handleMovement();
                break;
            case EntityState.END_TURN:
                return true;
        }
        return false;
    }

    private bool handleChooseAction() {
        if (clickedObject != null)
            state = EntityState.MOVEMENT;
        if (endTurnWasClicked())
            state = EntityState.END_TURN;
        return false;
    }

    private bool handleMovement() {
        // man kann nicht auf ein feld gehen, wo sich verbündete befinden
        // state movement 
        // move player to target position
        // subtract moves from actions
        // switch state to choose action
        return true;
    }
}
