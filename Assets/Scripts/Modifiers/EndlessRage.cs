using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRage : Modifier {

    public float chance = 0.2f;

    public override void modify() {
        if (owner.enemyKilled && Random.value < chance)
            owner.attackCountExtra++;            
    }
}
