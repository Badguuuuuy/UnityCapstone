using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 cameraPosition = new Vector3(0, 0, -10);

    public GameObject player;

    public float cameraMoveSpeed = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position + cameraPosition,
                                  Time.deltaTime * cameraMoveSpeed);
    }
}
