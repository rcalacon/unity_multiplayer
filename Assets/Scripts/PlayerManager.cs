using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

namespace Com.MyCompany.MyGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject PlayerUiPrefab;

        public float speedLimit;
        public float jumpForce;
        bool isGrounded;
        float ballSpeed;
        float slowDownSpeed;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }

        void Awake()
        {
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            speedLimit = 10f;
            jumpForce = 5f;
            isGrounded = true;
            ballSpeed = 10.0f;
            slowDownSpeed = .9f;

            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                this.ProcessInputs();
            }
        }

        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }


        void CalledOnLevelWasLoaded(int level)
        {
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }


        void ProcessInputs()
        {
            // Disable Movement In Mid Air
            if (!isGrounded)
            {
                return;
            }

            Rigidbody player = GetComponent<Rigidbody>();

            // General Movement
            if (Input.GetKey(KeyCode.W))
            {
                print("pressing w");
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
}