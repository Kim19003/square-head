using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformsSkip : MonoBehaviour
{
    Transform skipLocation;

    void Start()
    {
        skipLocation = transform.Find("SkipLocation");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent.transform.position = (Vector2)skipLocation.position;
        }
    }
}
