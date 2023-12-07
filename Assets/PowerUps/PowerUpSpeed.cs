using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : BasePowerUp
{
    public float newMovementSpeed = 50f;

    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if (thePickerUpper.movementSpeed <= newMovementSpeed)
        {
            return false;
        }
        else
        {
            thePickerUpper.ApplySpeedChange(newMovementSpeed);
            return true;
        }
    }
}
