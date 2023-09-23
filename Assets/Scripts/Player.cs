using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public float movementSpeed = 50f;
    public float rotationSpeed = 130f;
    public NetworkVariable<Color> playerColorNetVar = new NetworkVariable<Color>(Color.red);
    public float maxDistance = 25.0f;

    private Camera playerCamera;
    private GameObject playerBody;

    private void Start()
    {
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

        playerBody = transform.Find("PlayerBody").gameObject;
        ApplyColor();
    }

    private void Update()
    {
        if (IsOwner)
        {
            OwnerHandleInput();
        }
    }

    public void OwnerHandleInput()
    {
        Vector3 movement = CalcMovement();
        Vector3 rotation = CalcRotation();
        if (movement != Vector3.zero || rotation != Vector3.zero)
        {
            MoveServerRpc(movement, rotation);
        }

    }

    private void ApplyColor()
    {
        playerBody.GetComponent<MeshRenderer>().material.color = playerColorNetVar.Value;
        //Debug.Log($" {playerColorNetVar.Value}");
    }

    [ServerRpc(RequireOwnership = true)]
    private void MoveServerRpc(Vector3 movement, Vector3 rotation)
    {
        if (IsHost && OwnerClientId == NetworkManager.LocalClientId)
        {
            transform.Translate(movement);
            transform.Rotate(rotation);
        }
        else if (IsClient)
        {
            Vector3 newPosition = transform.position + movement;
            if (WithinClientBorder(newPosition))
            {
                transform.Translate(movement);
                transform.Rotate(rotation);
            }
            else
            {
                Debug.Log($"Too Far");
            }
        }
        
    }

    private Vector3 CalcRotation()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown)
        {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
        return rotVect;
    }

    private Vector3 CalcMovement()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");

        if (isShiftKeyDown)
        {
            x_move = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed * Time.deltaTime;

        return moveVect;
    }

    private bool WithinClientBorder(Vector3 position)
    {
        float minX = -maxDistance;
        float maxX = maxDistance;
        float minY = -maxDistance;
        float maxY = maxDistance;

        //Debug.Log($"{maxDistance}");
        //Debug.Log($"{position.x}, {position.y}");
        return position.x >= minX && position.x <= maxX && position.y >= minY && position.y <= maxY;
    }

}
