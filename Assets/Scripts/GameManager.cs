using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum GameState {
        INITIALIZE,
        ACT,
        ANIMATION
    }

    private DDOL ddol;
    private LevelManager levelManager;
    private GameState state = GameState.INITIALIZE;
    private List<Player> players = new List<Player>();

    public void Awake() {
        ddol = GameObject.FindObjectOfType<DDOL>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    public void startGame() {
        createPlayers();
        levelManager.createLevel(players);
        state = GameState.ACT;
    }

    private void createPlayers() {
        players.Add(GameObject.Instantiate(ddol.players[0].gameObject).GetComponent<Player>());
    }

    public void Update() {
        switch (state) {
            case GameState.INITIALIZE:
                break;
            case GameState.ACT:
                bool allPlayersDoneActing = true;
                foreach (Player player in players)
                    if (!player.act())
                        allPlayersDoneActing = false;
                if (allPlayersDoneActing)
                    state = GameState.ANIMATION;
                break;
            case GameState.ANIMATION:
                bool allPlayersDoneAnimating = true;
                foreach (Player player in players)
                    if (!player.animate())
                        allPlayersDoneAnimating = false;
                if (allPlayersDoneAnimating)
                    state = GameState.ACT;
                break;
        }
    }
}
