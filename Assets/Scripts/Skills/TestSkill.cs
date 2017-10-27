using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : Skill {

    public override void activate(GameObject target){
        print(owner + " activated TestSkill on " + target);
    }

    public override bool canBeUsed(GameObject target) {
        
        return true;
    }
}
