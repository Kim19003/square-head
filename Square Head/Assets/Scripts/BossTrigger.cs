using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    GameObject boss;
    
    void Start()
    {
        boss = GameObject.Find("Boss");
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!other.transform.parent.gameObject.GetComponent<Player>().HasAttackedEnemy)
            {
                boss.GetComponent<FirstLevelBoss>().Die(false);
            }
        }
    }
}
