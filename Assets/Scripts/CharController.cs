using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public float speedLimit = 10f;
    public float jumpForce = 1f;
    bool isGrounded;
    float ballSpeed = 20.0f;
    float slowDownSpeed = .9f;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
        // Disable Movement In Mid Air
        if (!isGrounded) return;

        Rigidbody player = GetComponent<Rigidbody>();

        // General Movement
        if (Input.GetKey(KeyCode.W))
        {
            player.AddForce(Camera.main.transform.forward * ballSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            player.AddForce(-Camera.main.transform.right * ballSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            player.AddForce(Camera.main.transform.right * ballSpeed);
        }

        // Slow Down
        if (Input.GetKey(KeyCode.S))
        {
            player.velocity = new Vector3(player.velocity.x * slowDownSpeed, player.velocity.y, player.velocity.z * slowDownSpeed);
        }
        // Jump
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            player.velocity = new Vector3(player.velocity.x, jumpForce, player.velocity.z);

        throttleVelocity(player);
    }

    void throttleVelocity(Rigidbody player)
    {
        if (player.velocity.x > speedLimit)
        {
            player.velocity = new Vector3(speedLimit, player.velocity.y, player.velocity.z);
        }
        else if (player.velocity.x < -speedLimit)
        {
            player.velocity = new Vector3(-speedLimit, player.velocity.y, player.velocity.z);
        }
        if (player.velocity.z > speedLimit)
        {
            player.velocity = new Vector3(player.velocity.x, player.velocity.y, speedLimit);
        }
        else if (player.velocity.z < -speedLimit)
        {
            player.velocity = new Vector3(player.velocity.x, player.velocity.y, -speedLimit);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // Character Landed From Jump
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }

        else
        {
            string materialToApply = "SplitMetalBall";
            // Change Characters
            if (col.gameObject.name == "Wood")
            {
                materialToApply = "WoodenBall";
            }
            else if (col.gameObject.name == "Eye")
            {
                materialToApply = "EyeBall";
            }
            else if (col.gameObject.name == "Water")
            {
                materialToApply = "Stylize Water Diffuse";
            }
            else if (col.gameObject.name == "Lava")
            {
                materialToApply = "Lava pattern";
            }
            else if (col.gameObject.name == "Metal")
            {
                materialToApply = "SplitMetalBall";
            }
            foreach (Material mat in Resources.FindObjectsOfTypeAll(typeof(Material)) as Material[])
            {
                if (mat.name == materialToApply)
                {
                    GetComponent<MeshRenderer>().material = mat;
                }
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
