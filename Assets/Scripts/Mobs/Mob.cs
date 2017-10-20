using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : Entity {

    public override bool act() {
        return true;
    }
}
//----------Ablauf Monster--------------------------------------------------------------------------------	
		//Player in Angriffsreichweite ? 
			//wenn ja gehe in ANGRIFF
			//wenn nicht gehen in BEWEGUNG 
		//---------------ANGRIFF---------------:	
			// Wähle Helden der angegriffen wird (ausnahme skill zb Spott würde dies verhindern)
			// würfle vorgesehene Würfel, werte diese aus
			// wenn POWER gewürfelt wurde:
				//Prüfe Debuffs des Helden (vergiftet,usw.)
				//nutze Power für Debuffs von denen der Held noch nicht betroffen ist (siehe BUFFS DEBUFF ORDNER)	
				//nutze Power um Schaden zu erhöhen
		//--------------BEWEGUNG--------------:
			//Prüfe welche Player nahe genug um ihn zu ereichen 
				//wenn 2 oder mehr Player ereichbar sind prüfe wenigsten hp 
				//laufe zu player mit geringsten HP (ausnahme skill zb Spott würde dies verhindern)
				//Wenn kein Spieler ereichbar ???
