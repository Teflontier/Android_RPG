using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenu : MonoBehaviour {

    public Transform content;
    private GameObject itemMenu;

    public void Awake() {
        itemMenu = transform.Find("ItemMenu").gameObject;
    }

    public void setVisibility(bool visible) {
        itemMenu.SetActive(visible);
    }

    public void addItem(Item item) {
        item.gameObject.transform.SetParent(content);
    }
}
