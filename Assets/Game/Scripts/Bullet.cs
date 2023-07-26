using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class Bullet : MonoBehaviour
{
    [SerializeField] float moveSpeed;

    public Player player;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * moveSpeed;
        Destroy(gameObject, 3f);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void ApplyLag(float lag)
    {
        transform.position += rb.velocity * lag;
    }

    public int GetScore()
    {
        return player.GetScore();
    }
}
