using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMenu : MonoBehaviour {

    private GameObject moveButton;
    private GameObject attackButton;
    private GameObject endButton;
    private GameObject skillMainButton;
    private GameObject skillButton1;
    private GameObject skillButton2;
    private GameObject skillButton3;
    private SpriteRenderer skillButton1Renderer;
    private SpriteRenderer skillButton2Renderer;
    private SpriteRenderer skillButton3Renderer;
    private GameObject itemButton;

    public void Awake() {
        moveButton = transform.Find("Move").gameObject;
        attackButton = transform.Find("Attack").gameObject;
        endButton = transform.Find("End").gameObject;
        skillMainButton = transform.Find("Skill").gameObject;
        skillButton1 = transform.Find("SkillButton1").gameObject;
        skillButton2 = transform.Find("SkillButton2").gameObject;
        skillButton3 = transform.Find("SkillButton3").gameObject;
        skillButton1Renderer = skillButton1.GetComponent<SpriteRenderer>();
        skillButton2Renderer = skillButton2.GetComponent<SpriteRenderer>();
        skillButton3Renderer = skillButton3.GetComponent<SpriteRenderer>();
        itemButton = transform.Find("Item").gameObject;
    }

    public void setVisibility(bool visible) {
        setMoveVisibility(visible);
        setAttackVisibility(visible);
        setEndVisibility(visible);
        setSkillVisibility(visible);
        setSkillSubButtonsVisibility(visible);
        setItemVisibility(visible);
    }

    public void setMoveVisibility(bool visible) {
        moveButton.SetActive(visible);
    }

    public void setAttackVisibility(bool visible) {
        attackButton.SetActive(visible);
    }

    public void setEndVisibility(bool visible) {
        endButton.SetActive(visible);
    }

    public void setSkillVisibility(bool visible) {
        skillMainButton.SetActive(visible);
    }

    public void setSkillButton1Visibility(bool visible) {
        skillButton1.SetActive(visible);
    }

    public void setSkillButton2Visibility(bool visible) {
        skillButton2.SetActive(visible);
    }

    public void setSkillButton3Visibility(bool visible) {
        skillButton3.SetActive(visible);
    }

    public void setSkillSubButtonsVisibility(bool visible) {
        setSkillButton1Visibility(visible);
        setSkillButton2Visibility(visible);
        setSkillButton3Visibility(visible);
    }

    public void setSkillButton1Image(Sprite sprite) {
        skillButton1Renderer.sprite = sprite;
    }

    public void setSkillButton2Image(Sprite sprite) {
        skillButton2Renderer.sprite = sprite;
    }

    public void setSkillButton3Image(Sprite sprite) {
        skillButton3Renderer.sprite = sprite;
    }

    public void setItemVisibility(bool visible) {
        itemButton.SetActive(visible);
    }
}
