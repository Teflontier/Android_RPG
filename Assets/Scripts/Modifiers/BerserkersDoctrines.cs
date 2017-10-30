using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkersDoctrines : Modifier {

    public float damageModifier = 2f;
    public float hpThreshold = 3f;

    public override void modify() {
        if (owner.hp < hpThreshold)
            owner.attackDamageModifiersAbsolute += damageModifier;
    }
}
