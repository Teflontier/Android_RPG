using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;

public class DDOL : MonoBehaviour {

    public List<Tile> tiles;
    public List<Blocker> blockers;
    public List<Player> players;
    public List<Mob> mobs;

    public GameObject greenOverlay;
    public GameObject redOverlay;
    public GameObject skillButton;

    public void Awake() {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Main");
    }

    public void Start(){
        transform.Find("GameManager").GetComponent<GameManager>().startGame();
    }
}
