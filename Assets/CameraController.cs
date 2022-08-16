using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Default value: 0.1")]
    public float speed = 0.1f;
    [Tooltip("Default value: 0.1")]
    public float zoomSpeed = 0.1f;
    [Tooltip("Default value: 0.5")]
    public float minFOV = 0.5f;
    Camera cam;
    Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0f, 0f, -10f);
        cam = this.gameObject.GetComponent<Camera>();
        if(!cam.orthographic) cam.orthographic = true;
    }

    // Update is called once per frame
    void Update()
    {
        pos = Vector3.zero;

        // ----------CAMERA CONTROLS----------
                                                                            // KEYMAP
        if(Input.GetKey(KeyCode.W)) {                                       //  W
            pos += (Vector3.up * speed);
        } if(Input.GetKey(KeyCode.A)) {                                     //  A
            pos += (Vector3.left * speed);
        } if(Input.GetKey(KeyCode.S)) {                                     //  S
            pos += (Vector3.down * speed);
        } if(Input.GetKey(KeyCode.D)) {                                     //  D
            pos += (Vector3.right * speed);
        }

        this.transform.Translate(pos);

        if (Input.mouseScrollDelta.y > 0) {                                 // Scroll up
            cam.orthographicSize -= zoomSpeed;
        } if (Input.mouseScrollDelta.y < 0) {                               // Scroll down
            cam.orthographicSize += zoomSpeed;
        }

        if(cam.orthographicSize < minFOV) {
            cam.orthographicSize = minFOV;
        }
    }
}
