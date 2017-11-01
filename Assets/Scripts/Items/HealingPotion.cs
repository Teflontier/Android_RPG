using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : Item {
    
    public override bool activate(Tile target) {
        user.increaseHp(5);
        return true;
    }

    public override bool canBeUsed(Tile target) {
        return levelManager.getEntityOnTile(target) == user;
    }
}
