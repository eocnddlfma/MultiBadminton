using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Shuttlecock : NetworkBehaviour , ISingleton<Shuttlecock>
{
    public static Shuttlecock instance;
    
    [SerializeField]private Rigidbody2D rd;
    [SerializeField] private TrailRenderer tr;
    public float upPower;
    public float rightPower;
    public float downPower;
    public float hairpinPower;

    public AudioSource _audioSource;
    public AudioClip smallSound;
    public AudioClip middleSound;
    public AudioClip bigSound;

    public float _gravityScale;
    public float movementPower;
    
    private void Awake()
    {
        //instance = this;
        SetInstance(this);
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        setpositionstartClientRpc();
    }
    
    [ClientRpc]
    public void setpositionstartClientRpc()
    {
        SetPositionWithNoRigidbody(0);
    }
    
    void Update()
    {
        SetShuttlecockClientRpc();
        if (Input.GetMouseButtonDown(0))
        {
            //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        
    }
    private void FixedUpdate()
    {
        rd.velocity = new Vector2(rd.velocity.x * (1 - Time.fixedDeltaTime), rd.velocity.y);
    }

    public void SetPositionWithNoRigidbody(int player)
    {
        rd.gravityScale = 0;
        rd.velocity = Vector2.zero;
        if (player == 0)
        {
            transform.position = new Vector3(-12, 4);
        }
        else
        {
            transform.position = new Vector3(12, 4);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        print("entered collision");
        if (other.transform.CompareTag("Floor"))
        {
            print("floorTouch");
            SetTrailRendererClientRpc(false);
            tr.enabled = false;
            GameManager.instance.OnFloorTouch(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        print("entered trigger");
        if (rd.gravityScale == 0)
        {
            SetTrailRendererClientRpc(true);
            tr.enabled = true;
            SetGravityServerRpc();
        }


        if (other.CompareTag("Racket"))
        {
            //print(other.transform.eulerAngles.z);
            PlayerInput pi = other.GetComponentInParent<PlayerInput>();
            bool isleft = pi.isLeft;

            PlayerInput.SwingStyle swingStyle = pi.swingStyle;
            // print(isleft.ToString() + " " + IsHost.ToString());
            Vector2 vector2;
            // ChangeOwnerBecauseOfAuthorityServerRpc(NetworkManager.LocalClientId);
            // print("changed owner to " + NetworkManager.LocalClientId);
            AddforceRigidbody2dVelocityServerRpc(new Vector2(isleft ? 1 : -1, 0), pi.movePowerHorizontal * movementPower);
            AddforceRigidbody2dVelocityServerRpc(new Vector2(0, 1), pi.movePowerVertical * movementPower);
            switch (swingStyle)
            {
                case PlayerInput.SwingStyle.Up:
                    vector2 = new Vector2(isleft ? 1 : -1, 0.5f);
                    _audioSource.clip = bigSound;
                    _audioSource.Play();    
                    print(vector2);
                    SetVelocityzeroServerRpc();
                    AddforceRigidbody2dVelocityServerRpc(new Vector2(vector2.x, vector2.y), upPower);
                    // rd.AddForce(vector2.normalized * clearSmashPower, ForceMode2D.Impulse);
                    break;
                case PlayerInput.SwingStyle.Right:
                    vector2 = new Vector2(isleft ? 1 : -1, 0.1f);
                    _audioSource.clip = bigSound;
                    _audioSource.Play();
                    print(vector2);
                    SetVelocityzeroServerRpc();
                    AddforceRigidbody2dVelocityServerRpc(vector2, rightPower);
                    // rd.AddForce(vector2.normalized * drivePower, ForceMode2D.Impulse);
                    break;
                case PlayerInput.SwingStyle.Down:
                    vector2 = new Vector2(isleft ? 1 : -1, -0.3f);
                    _audioSource.clip = middleSound;
                    _audioSource.Play();
                    SetVelocityzeroServerRpc();
                    AddforceRigidbody2dVelocityServerRpc(vector2, downPower);
                    // rd.AddForce(vector2.normalized * underPower, ForceMode2D.Impulse);
                    print(vector2);
                    break;
                case PlayerInput.SwingStyle.Hairpin:
                    print("hairpin");
                    float angle = other.transform.eulerAngles.z;
                    vector2 = new Vector2(
                        Mathf.Cos((angle - 90) * Mathf.Deg2Rad),
                        Mathf.Sin((angle - 90) * Mathf.Deg2Rad) * 2
                    );
                    _audioSource.clip = smallSound;
                    _audioSource.Play();
                    SetVelocityzeroServerRpc();
                    AddforceRigidbody2dVelocityServerRpc(-vector2, hairpinPower);
                    // rd.AddForce(-vector2.normalized * hairpinPower, ForceMode2D.Impulse);
                    break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddforceRigidbody2dVelocityServerRpc(Vector2 vector2, float power)
    {
        rd.AddForce(vector2.normalized * power, ForceMode2D.Impulse);
    }
    [ClientRpc]
    public void SetTrailRendererClientRpc(bool isrender)
    {
        print("set trailrenderer" + isrender);
        tr.enabled = isrender;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetGravityServerRpc()
    {
        rd.gravityScale = _gravityScale;
    }
    [ClientRpc]
    public void SetShuttlecockClientRpc()
    {
        transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(rd.velocity.y, rd.velocity.x) * Mathf.Rad2Deg -90);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetVelocityzeroServerRpc()
    {
        rd.velocity = Vector3.zero;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeOwnerBecauseOfAuthorityServerRpc(ulong id)
    {
        // print("changed owner to " + NetworkManager.LocalClientId);
        NetworkObject.ChangeOwnership(id);
    }

    public void SetInstance(Shuttlecock thisu)
    {
        if (instance == null)
        {
            instance = thisu;
        }
        else
        {
            print("two instances man");
        }
    }
}
