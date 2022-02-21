using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(player.name, new Vector2(-1, -1), Quaternion.identity);
    }
}
