using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public enum GameState {
        INITIALIZE,
        STORY,
        ACT
    }

    public LayerMask menuLayer;
    public Dictionary<Mesh, Transform> meshesToCheckClick = new Dictionary<Mesh, Transform>();

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
        player.hp = player.maxHp;
        player.mana = player.maxMana;
        player.stamina = player.maxStamina;
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

    public void objectWasClicked(GameObject obj, Vector2 position) {
        if (menuLayer == (menuLayer | (1 << obj.layer))) {
            turnList[0].clickedObject = obj;
            return;
        }
            

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(position);
        foreach (KeyValuePair<Mesh, Transform> pair in meshesToCheckClick) {
            Mesh mesh = pair.Key;
            Transform trans = pair.Value;

            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            int triangleCount = triangles.Length / 3;
            bool inTriangle = false;
            for (int i = 0; i < triangleCount; i++) {
                Vector2 v1 = vertices[triangles[i * 3]];
                Vector2 v2 = vertices[triangles[i * 3 + 1]];
                Vector2 v3 = vertices[triangles[i * 3 + 2]];
                if (checkPointInTriangle(trans.InverseTransformPoint(worldPosition), v1, v2, v3)) {
                    inTriangle = true;
                    break;
                }
            }
            if (inTriangle) {
                turnList[0].clickedObject = obj;
                return;
            }
        }
    }

    private bool checkPointInTriangle(Vector2 p, Vector2 v1, Vector2 v2, Vector2 v3) {
        float alpha = ((v2.y - v3.y) * (p.x - v3.x) + (v3.x - v2.x) * (p.y - v3.y)) /
                      ((v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y));
        float beta = ((v3.y - v1.y) * (p.x - v3.x) + (v1.x - v3.x) * (p.y - v3.y)) /
                     ((v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y));
        float gamma = 1.0f - alpha - beta;
        return alpha > 0 && beta > 0 && gamma > 0;
    }

    public void destroyEntity(Entity entity) {
        turnList.Remove(entity);
        if (entity is Player)
            players.Remove((Player)entity);
        if (entity is Mob)
            mobs.Remove((Mob)entity);
        levelManager.entities.Remove(entity);
        Destroy(entity.gameObject);
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
