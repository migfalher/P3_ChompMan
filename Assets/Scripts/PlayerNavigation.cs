using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerNavigation : MonoBehaviour
{
    // public components
    public float speed = 1.0f;

    // private components
    private GameManager manager_Script;
    private GameObject mesh;
    private Rigidbody rigidBody;
    private Vector3 forward = Vector3.zero;
    private Vector3 currentDirection = Vector3.zero;
    private Vector3[] directionsList =
    {
        new Vector3 (0, 0, 1),     // top
        new Vector3 (1, 0, 1),     // right-top
        new Vector3 (1, 0, 0),     // right
        new Vector3 (1, 0, -1),    // bottom-right
        new Vector3 (0, 0, -1),    // bottom
        new Vector3 (-1, 0, -1),   // bottom-left
        new Vector3 (-1, 0, 0),    // left
        new Vector3 (-1, 0, 1)     // top-left
    };

    private void Start()
    {
        manager_Script = GameObject.Find("GameManager").GetComponent<GameManager>();
        mesh = this.transform.GetChild(0).gameObject;
        rigidBody = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        currentDirection = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );
        forward = currentDirection * rigidBody.mass * speed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        rigidBody.linearVelocity = forward;
    }

    public void MovementAxis(InputAction.CallbackContext context)
    {
        Vector3 newAngle = new Vector3(0, 0, 0);
        currentDirection = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );

        if (context.performed)
        {
            if (currentDirection == directionsList[0]) { newAngle.y = 0; }
            else if (currentDirection == directionsList[1]) { newAngle.y = 45; }
            else if (currentDirection == directionsList[2]) { newAngle.y = 90; }
            else if (currentDirection == directionsList[3]) { newAngle.y = 135; }
            else if (currentDirection == directionsList[4]) { newAngle.y = 180; }
            else if (currentDirection == directionsList[5]) { newAngle.y = 225; }
            else if (currentDirection == directionsList[6]) { newAngle.y = 270; }
            else if (currentDirection == directionsList[7]) { newAngle.y = 315; }
            
            mesh.transform.localEulerAngles = newAngle;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        string tag = go.tag;
        switch (tag)
        {
            case "PowerUp":
                manager_Script.TouchPowerUp(other.gameObject);
                break;
            case "Enemy":
                manager_Script.TouchEnemy(other.gameObject);
                break;
            case "Sphere":
                manager_Script.TouchSphere(go);
                break;
            case "Finish":
                manager_Script.TouchFinishPlane();
                break;
            case "Entry_A":
                this.transform.position = GameObject.Find("Entry_Z").transform.position;
                break;
            case "Entry_Z":
                this.transform.position = GameObject.Find("Entry_A").transform.position;
                break;
            default:
                Debug.LogError("Unexpected tag at trigger " + other.name);
                break;
        }
    }
}