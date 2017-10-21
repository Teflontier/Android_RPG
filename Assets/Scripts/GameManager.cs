﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum GameState {
        INITIALIZE,
        STORY,
        ACT
    }

    private DDOL ddol;
    private LevelManager levelManager;
    private GameState state = GameState.INITIALIZE;
    private bool storyEnded = true;
    private List<Player> players = new List<Player>();
    private List<Mob> mobs = new List<Mob>();
    private List<Entity> turnList = new List<Entity>();

    public void Awake() {
        ddol = GameObject.FindObjectOfType<DDOL>();
        levelManager = GameObject.FindObjectOfType<LevelManager>();
    }

    public void startGame() {
        createPlayers();
        levelManager.createLevel(players);
        resetTurnList();
        state = GameState.STORY;
    }

    private void createPlayers() {
        // the players should actually be created by some sort of ui and be chosen by the gamer
        Player player = GameObject.Instantiate(ddol.players[0].gameObject).GetComponent<Player>();
        Camera.main.transform.SetParent(player.gameObject.transform);
        players.Add(player);
    }

    private void resetTurnList() {
        turnList.Clear();
        players.Clear();
        mobs.Clear();
        levelManager.entities.ForEach(entity => {
                if (entity is Player)
                    players.Add((Player)entity);
                if (entity is Mob)
                    mobs.Add((Mob)entity);
            });
        foreach (Player player in players)
            turnList.Insert(0, player);
        bool addedToTheEnd = false;
        for (int i = 0; i < mobs.Count; i++) {
            int index = i * 2 + 1;
            if (turnList.Count >= index && !addedToTheEnd)
                turnList.Insert(index, mobs[i]);
            else {
                turnList.Add(mobs[i]);
                addedToTheEnd = true;
            }
        }
    }

    public void objectWasClicked(GameObject obj) {
        turnList[0].clickedObject = obj;
    }

    public void Update() {
        Entity activeEntity = turnList[0];
        switch (state) {
            case GameState.INITIALIZE:
                break;
            case GameState.STORY:
                if (storyEnded)
                    state = GameState.ACT;
                break;
            case GameState.ACT:
                if (!activeEntity.act())
                    break;
                turnList.Remove(activeEntity);
                turnList.Add(activeEntity);
                activeEntity.state = Entity.EntityState.INITIALIZE;
                break;
        }
    }
}
