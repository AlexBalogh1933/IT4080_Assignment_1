using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : BasePowerUp
{
    public float newMovementSpeed = 10f;

    protected override bool ApplyToPlayer(Player thePickerUpper)
    {
        if (thePickerUpper.movementSpeed <= newMovementSpeed)
        {
            return false;
        }
        else
        {
            thePickerUpper.movementSpeed = newMovementSpeed;
            Debug.Log($"Adding new movement speed to player {NetworkManager.LocalClientId}");
            return true;
        }
    }
}
