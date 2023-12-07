using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHealth : BasePowerUp
{
    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        thePickerUpper.playerHP.Value += 10;
        Debug.Log("Added Health");
        return true;
    }
}
