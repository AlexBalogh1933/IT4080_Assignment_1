using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : BasePowerUp
{
    public float newMovementSpeed = 10f;

    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if (thePickerUpper.clientMovementSpeed.Value <= newMovementSpeed)
        {
            return false;
        }
        else
        {
            thePickerUpper.ApplySpeedChange(newMovementSpeed);
            //Debug.Log($"Adding new movement speed to player {thePickerUpper.OwnerClientId}");
            //Debug.Log($"New speed {thePickerUpper.clientMovementSpeed.Value}");
            return true;
        }
    }
}
