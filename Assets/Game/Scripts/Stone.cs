using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviourPun
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.InstantiationData != null)
        {
            Vector3 force = (Vector3)photonView.InstantiationData[0];
            Vector3 torque = (Vector3)photonView.InstantiationData[1];

            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) // PhotonNetwork.IsMasterClient
            return;

        if (transform.position.magnitude > 200f)
        {
            PhotonNetwork.Destroy(photonView);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) // PhotonNetwork.IsMasterClient
            return;

        if (other.gameObject.name == "LaserBolt(Clone)")
        {
            Debug.Log("���� �߰�");
            // �ε������� ����
            // �� �Ѿ� ���������� ���� �߰�
            Player player = other.gameObject.GetComponent<Bullet>().player;
            int score = player.GetScore();
            player.SetScore(++score);
            PhotonNetwork.Destroy(photonView);
        }
    }
}
