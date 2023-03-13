using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class folow : MonoBehaviour
{
    public Transform Player;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.position.x + 20, Player.position.y, transform.position.z);
    }
}
