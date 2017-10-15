using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {

	public enum PlayerState{
		CHOOSE_ACTION, MOVEMENT, END_TURN
	}

	public int maxHp;
	public int x;
	public int y;

	private PlayerState state;
    private LevelManager levelManager;
    private int moves = 2;

    public void Awake() {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }
	//--------------------------playeractions-----------------------------
    public bool act() {
		state = PlayerState.CHOOSE_ACTION;

		switch (state) {
		case PlayerState.CHOOSE_ACTION:
			showPossibleMoves (); // zeige freie felder grün an, felder mit blockern orange, felder mit gegnern rot
			checkIfSomethingClicked ();
			if (tileWasClicked ())
				state = PlayerState.MOVEMENT;
			if(endTurnWasClicked())
				state = PlayerState.END_TURN;
			break;

		case PlayerState.MOVEMENT: // man kann nicht auf ein feld gehen, wo sich verbündete befinden
		// state movement 
		// move player to target position
		// subtract moves from actions
		// switch state to choose action
			break;
		}





        int movesBefore = moves;
        if (Input.GetKey(KeyCode.A) && levelManager.movePlayer(this, x - 1, y))
            moves--;
        else if (Input.GetKey(KeyCode.D) && levelManager.movePlayer(this, x + 1, y))
            moves--;
        else if (Input.GetKey(KeyCode.S) && levelManager.movePlayer(this, x, y - 1))
            moves--;
        else if (Input.GetKey(KeyCode.W) && levelManager.movePlayer(this, x, y + 1))
            moves--;
        return moves != movesBefore;
    }

    public bool animate() {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = new Vector2(x, y);
        if (currentPosition != targetPosition) {
            if (Vector2.Distance(currentPosition, targetPosition) < 0.03f)
                transform.position = targetPosition;
            else
                transform.position = Vector2.Lerp(currentPosition, targetPosition, 0.1f);
            return false;
        }
        return true;
    }

	private void showPossibleMoves(){

	}

	private void checkIfSomethingClicked(){

	}

	private bool tileWasClicked(){
		return false;
	}

	private bool endTurnWasClicked(){
		return false;
	}
}
