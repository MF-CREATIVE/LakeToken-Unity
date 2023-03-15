using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MirrorBoatController : NetworkBehaviour
{
    [SyncVar] public bool _InUse = false;
    [SyncVar(hook = nameof(OnRiderChanged))] public NetworkIdentity _Rider;
    [SerializeField] private float _Acceleration = 10f;
    [SerializeField] private float _MaxSpeed = 10f;
    [SerializeField] private float _RotationSpeed = 2f;
    [SerializeField] private LayerMask _WallLayerMask;
    public Transform seat;
    public Transform ExitPosition;
    private Rigidbody _rigidbody;
    private Vector3 _inputDirection;
    private bool _isMoving = false;
    float Action = 3f;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (_InUse && _Rider != null && _Rider.isLocalPlayer)
        {   
            if (Input.GetKeyDown(KeyCode.E) &&Action<=1f)
            {
                Action = 3f;
                _Rider.transform.position = ExitPosition.position;
                _Rider.GetComponent<TestPlayerController>().SetStateSitting(false);
                ExitBoat();
                return;
            }
            _Rider.transform.position = seat.position;
            _Rider.transform.rotation = seat.rotation;
        }
    }
    void FixedUpdate()
    {
        if (_InUse && _Rider != null && _Rider.isLocalPlayer)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            _inputDirection = new Vector3(0, 0, Input.GetAxis("Vertical")).normalized;
            if (_inputDirection.magnitude > 0f)
            {
                _isMoving = true;
                _rigidbody.AddForce(transform.forward * _inputDirection.magnitude * _Acceleration * Time.fixedDeltaTime, ForceMode.Acceleration);
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _MaxSpeed);
            }
            else
            {
                _isMoving = false;
            }

            transform.Rotate(Vector3.up * horizontalInput * _RotationSpeed);
           
        }
        if(Action >= 0)
        {
            Action += -Time.deltaTime;
        }
    }

    [Command(requiresAuthority = false)]
    void CmdSetAuthority(NetworkIdentity identity)
    {
        if (identity != null)
        {
                _InUse = true;
                _Rider = identity;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !_InUse && Input.GetKeyUp(KeyCode.E) && Action <=1f)
        {
            NetworkIdentity id = other.GetComponent<NetworkIdentity>();
            if (id != null)
            {
                CmdSetAuthority(id); // Set authority for the player's network identity
                Action = 3f;
                id.GetComponent<TestPlayerController>().SetStateSitting(true);
            }
            return;
        }
 
    }
    [Command(requiresAuthority =false)]
    void ExitBoat()
    {
          _InUse = false;
          _Rider = null;
    }

    void OnRiderChanged(NetworkIdentity oldRider, NetworkIdentity newRider)
    {
        if (newRider == null)
        {
            _InUse = false;
            StopMoving();
        }
    }

    void StopMoving()
    {
        _isMoving = false;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}