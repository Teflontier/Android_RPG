using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {

    private LevelManager levelManager;
    private int moves = 2;
    public int x;
    public int y;

    public void Awake() {
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    public bool act() {
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
}
