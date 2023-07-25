using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] List<Color> playerColorList;

    [SerializeField] float accelPower;
    [SerializeField] float rotateSpeed;
    [SerializeField] float maxSpeed;

    [SerializeField] Bullet bulletPrefab;

    [SerializeField] int count;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Vector2 inputDir;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    
        SetPlayerColor();

        if (!photonView.IsMine)
            Destroy(playerInput);
        
        if (photonView.IsMine)
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 20, 0);    
        }
    }

    private void Update()
    {
        Accelate(inputDir.y);
        Rotate(inputDir.x);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);

        count++;
    }

    private void OnMove(InputValue value)
    {
        inputDir = value.Get<Vector2>();
    }

    private void OnFire(InputValue value)
    {
        photonView.RPC("CreateBullet", RpcTarget.All);
    }

    [PunRPC]
    private void CreateBullet()
    {
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }

    private void Accelate(float input)
    {
        rb.AddForce(input * accelPower * transform.forward, ForceMode.Force);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void Rotate(float input) 
    {
        transform.Rotate(Vector3.up, input * rotateSpeed * Time.deltaTime);
    }

    private void SetPlayerColor()
    {
        int playerNumber = photonView.Owner.GetPlayerNumber();

        if (playerColorList == null || playerColorList.Count <= playerNumber)
            return;

        Renderer render = GetComponent<Renderer>();
        render.material.color = playerColorList[playerNumber];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(count);
        }
        else
        {
            count = (int)stream.ReceiveNext();
        }
    }
}
