using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Player : Entity {
    private const string MOVE = "Move";
    private const string ATTACK = "Attack";
    private const string END = "End";
    private const string SKILL = "Skill";
    private const string SKILL1 = "SkillButton1";
    private const string SKILL2 = "SkillButton2";
    private const string SKILL3 = "SkillButton3";

    private GameObject tileToUseActionOn;
    private GameObject lastMenuClicked;
    private List<Tile> tilesToMove = new List<Tile>();

    public override bool act() {
        switch (state) {
            case EntityState.INITIALIZE:
                initialize();
                state = EntityState.SHOW_POSSIBLE_MOVES;
                break;
            case EntityState.SHOW_POSSIBLE_MOVES:
                calcPossibleActions();
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
            case EntityState.CALCULATE_ACTION_FIELDS:
                handleCalculateActionFields();
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
            case EntityState.SHOW_SKILLS:
                handleShowSkills();
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
        tileToUseActionOn = clickedObject;
        clickedObject = null;
        state = EntityState.CREATE_COMMAND_MENU;
    }

    private void handleCreateCommandMenu() {
        commandMenu.setVisibility(false);
        Vector2 clickedPos = tileToUseActionOn.transform.position;
        commandMenu.transform.position = new Vector2(clickedPos.x, clickedPos.y + 1);
        Tile tile = tileToUseActionOn.GetComponent<Tile>();
        if (tile != null && TileUtilities.getWrappedTileInFloodFilledTiles(movableTiles, tile) != null)
            commandMenu.setMoveVisibility(true);
        if (tile != null && TileUtilities.getWrappedTileInFloodFilledTiles(attackableTiles, tile) != null && levelManager.isAttackable(tile))
            commandMenu.setAttackVisibility(true);
        foreach (Skill skill in skills) {
            if (skill.canBeUsed(tileToUseActionOn)) {
                commandMenu.setSkillVisibility(true);
                break;
            }
        }
        commandMenu.setEndVisibility(true);
        state = EntityState.WAIT_FOR_MENU_SELECTION;
    }

    private void handleWaitForMenuSelection() {
        if (clickedObject == null)
            return;
        if (clickedObject.GetComponent<Tile>() != null || clickedObject.GetComponent<Blocker>() != null) {
            handleWaitForAction();
            return;
        }
        lastMenuClicked = clickedObject;
        clickedObject = null;
        switch (lastMenuClicked.name) {
            case END:
                levelManager.destroyOverlays();
                commandMenu.setVisibility(false);
                state = EntityState.END_TURN;
                return;
            case MOVE:
                levelManager.destroyOverlays();
                commandMenu.setVisibility(false);
                state = EntityState.CALCULATE_ACTION_FIELDS;
                return;
            case ATTACK:
                levelManager.destroyOverlays();
                commandMenu.setVisibility(false);
                state = EntityState.ATTACK;
                return;
            case SKILL:
                state = EntityState.SHOW_SKILLS;
                return;
            case SKILL1:
                skills[0].activate(tileToUseActionOn);
                commandMenu.setVisibility(false);
                levelManager.destroyOverlays();
                state = EntityState.SHOW_POSSIBLE_MOVES;
                return;
            case SKILL2:
                skills[1].activate(tileToUseActionOn);
                commandMenu.setVisibility(false);
                levelManager.destroyOverlays();
                state = EntityState.SHOW_POSSIBLE_MOVES;
                return;
            case SKILL3:
                skills[2].activate(tileToUseActionOn);
                commandMenu.setVisibility(false);
                levelManager.destroyOverlays();
                state = EntityState.SHOW_POSSIBLE_MOVES;
                return;
        }
    }

    private void handleCalculateActionFields() {
        Tile targetTile = tileToUseActionOn.GetComponent<Tile>();
        tilesToMove.Clear();
        tilesToMove = TileUtilities.getShortestWayFromFloodFilledTiles<int>(movableTiles, targetTile);
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

    private void handleAttack() {
        Mob mob = levelManager.getGameObjectOnTile(tileToUseActionOn.GetComponent<Tile>()).GetComponent<Mob>();
        attacks--;
        mob.increaseHp(-1);
        state = EntityState.SHOW_POSSIBLE_MOVES;
    }

    private void handleShowSkills() {
        for (int i = 0; i < skills.Length; i++) {
            if (skills[i].canBeUsed(tileToUseActionOn)) {
                MethodInfo method = typeof(CommandMenu).GetMethod("setSkillButton" + (i + 1) + "Visibility");
                method.Invoke(commandMenu, new object[]{ true });
            }
        }
        state = EntityState.WAIT_FOR_MENU_SELECTION;
    }
}
