using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MainCamera : MonoBehaviour
{
    Camera thisCamera;

    GameObject player;
    Player playerScript;

    GameObject levelEndArea;

    public Vector3 DefaultCameraPosition { get; private set; }
    public float DefaultCameraSize { get; private set; }

    bool isDefaultSize = true;

    float cameraMaxXPosition;

    void Start()
    {
        thisCamera = GetComponent<Camera>();

        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();

        levelEndArea = GameObject.Find("LevelEndArea");

        DefaultCameraPosition = transform.position;
        DefaultCameraSize = thisCamera.orthographicSize;

        cameraMaxXPosition = levelEndArea.transform.position.x - 10;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isDefaultSize)
            {
                thisCamera.orthographicSize = DefaultCameraSize * 2;
                isDefaultSize = false;
            }
            else
            {
                thisCamera.orthographicSize = DefaultCameraSize;
                isDefaultSize = true;
            }
        }

        if (!playerScript.IsDead)
        {
            if (player.transform.position.x > DefaultCameraPosition.x && player.transform.position.x < cameraMaxXPosition)
            {
                transform.position = new Vector3(player.transform.position.x, DefaultCameraPosition.y, DefaultCameraPosition.z);
            }
            else
            {
                transform.position = new Vector3(player.transform.position.x > cameraMaxXPosition ? cameraMaxXPosition : DefaultCameraPosition.x,
                    DefaultCameraPosition.y, DefaultCameraPosition.z);
            }
        }
    }
}
