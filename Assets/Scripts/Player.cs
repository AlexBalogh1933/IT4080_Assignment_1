using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> ScoreNetVar = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerHP = new NetworkVariable<int>();

    public BulletSpawner bulletSpawner;

    //public NetworkVariable<float> clientMovementSpeed = new NetworkVariable<float>(10f);
    public float movementSpeed = 10f;
    public float rotationSpeed = 130f;
    //public float maxDistance = 25.0f;

    private Camera playerCamera;
    private GameObject playerBody;

    private CharacterController characterController;

    private void NetworkInit()
    {
        playerBody = transform.Find("PlayerBody").gameObject;

        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

        ApplyPlayerColor();
        PlayerColor.OnValueChanged += OnPlayerColorChanged;

        //if (IsClient)
        //{
        //    ScoreNetVar.OnValueChanged += ClientOnScoreValueChanged;
        //}
    }

    private void Awake()
    {
        NetworkHelper.Log(this, "Awake");
    }

    private void Start()
    {
        NetworkHelper.Log(this, "Start");
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            OwnerHandleInput();
            if (Input.GetButtonDown("Fire1"))
            {
                //NetworkHelper.Log("Requesting Fire");
                bulletSpawner.FireServerRpc();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkHelper.Log(this, "OnNetworkSpawn");
        NetworkInit();
        base.OnNetworkSpawn();
        playerHP.Value = 100;
    }

    //private void ClientOnScoreValueChanged(int old, int current)
    //{
    //    if (IsOwner)
    //    {
    //        NetworkHelper.Log(this, $"My score is {ScoreNetVar.Value}");
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            ServerHandleCollision(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer)
        {
            if (other.CompareTag("power_up"))
            {
                other.GetComponent<BasePowerUp>().ServerPickUp(this);
            }
        }
        //else if (other.GetComponent<HealthPickup>())
        //{
        //    Debug.Log("Player HP+");
        //    playerHP.Value += 50;
        //}

    }


    private void ServerHandleCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            ulong ownerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            //NetworkHelper.Log(this,
            //    $"Hit by {collision.gameObject.name} " +
            //    $"owned by {ownerId}");
            Player other = NetworkManager.Singleton.ConnectedClients[ownerId].PlayerObject.GetComponent<Player>();
            //other.ScoreNetVar.Value += 1;
            //NetworkManager.Singleton.ConnectedClients[other.GetComponent<NetworkObject>().OwnerClientId]
                //.PlayerObject.GetComponent<NetworkPlayerData>().score.Value += 1;
            Debug.Log("Player dmg");
            playerHP.Value -= 10;
            Destroy(collision.gameObject);
        }
    }

    public void ApplySpeedChange(float newSpeed)
    {
        movementSpeed = newSpeed;
        //clientMovementSpeed.Value = newSpeed;
    }

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        ApplyPlayerColor();
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
            transform.Translate(movement);
            transform.Rotate(rotation);
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
        //moveVect *= clientMovementSpeed.Value * Time.deltaTime;

        // Below was for testing purposes. Floods Console but helped find my issue. 
        //if (IsClient && OwnerClientId == NetworkManager.LocalClientId)
        //{
        //    Debug.Log($"This is the movement speed of player {NetworkManager.LocalClientId} is {clientMovementSpeed.Value}");
        //}

        return moveVect;
    }

    //private bool WithinClientBorder(Vector3 position)
    //{
    //    float minX = -maxDistance;
    //    float maxX = maxDistance;
    //    float minZ = -maxDistance;
    //    float maxZ = maxDistance;

    //    //Debug.Log($"{maxDistance}");
    //    //Debug.Log($"{position.x}, {position.z}");
    //    return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    //}

    private void ApplyPlayerColor()
    {
        NetworkHelper.Log(this, $"Applying color {PlayerColor.Value}");
        playerBody.GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
        //Debug.Log($" {PlayerColor.Value}");
    }

}
