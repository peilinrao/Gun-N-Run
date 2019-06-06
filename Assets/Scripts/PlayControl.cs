using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayControl : MonoBehaviour {

    public float initialSpeed;
    public float incrementation;
    public LayerMask mask;
    public float fallingThreshold;
    public float GunCoolDown;
    public float GunShotTime;
    public float range;

    private AudioSource Sound_Gun;
    private AudioSource Sound_Destroy;
    private AudioSource Sound_Die;
    private AudioSource Sound_BG;
    private AudioSource Sound_SwitchWeapon;

    private GameObject Sword_L_Green;
    private GameObject Sword_R_Blue;
    private GameObject Camera;
    private GameObject Reticle;
    private GameObject gun_laser;
    private GameObject obj;
    private LineRenderer obj_lineRenderer;

    private Rigidbody rb;
    private float speed = 0;
    private float timeSinceShot = 0;
    private float rayShowing = float.PositiveInfinity;
    private bool shooting = true;
    private bool isGrounded = true;
    private float last_velocity = 0;
    // Use this for initialization
    void Start () {
        Utility.score = 0;
        Utility.die = 0;
        Sound_Gun = GameObject.Find("Sound_Gun").GetComponent<AudioSource>();
        Sound_Destroy = GameObject.Find("Sound_Destroy").GetComponent<AudioSource>();
        Sound_Die = GameObject.Find("Sound_Die").GetComponent<AudioSource>();
        Sound_BG = GameObject.Find("Sound_BG").GetComponent<AudioSource>();
        Sound_SwitchWeapon = GameObject.Find("Sound_SwitchWeapon").GetComponent<AudioSource>();
        Sound_BG.Play();

        Sword_L_Green = GameObject.Find("Sword_L_Green");
        Sword_R_Blue = GameObject.Find("Sword_R_Blue");
        Camera = GameObject.Find("CenterEyeAnchor");
        Reticle = GameObject.Find("Reticle");
        gun_laser = GameObject.Find("gun_laser");
        changeWeapon(0);
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        // Dying condition
        if (transform.position.y < fallingThreshold || (speed != 0 && (rb.velocity.z < 0f || rb.velocity.z < (last_velocity-1)/5)))
        {
            if (transform.position.y < fallingThreshold)
                Utility.die = 1;
            else
                Utility.die = 2;
            Sound_Die.Play();
        }
        last_velocity = rb.velocity.z;

        // Jump
        if (Input.GetButtonDown("Fire1") && isGrounded)
        {
            rb.AddForce(new Vector3(0, 7, 0), ForceMode.Impulse);
            isGrounded = false;
        }

        // Side boundary
        float positionX = Mathf.Max(-4.5f, Mathf.Min(4.5f, transform.position.x));
        transform.position = new Vector3(positionX, transform.position.y, transform.position.z);

        // Moving left and right
        float axisX = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch)[0];
        rb.velocity = new Vector3(axisX * 30, rb.velocity.y, speed);
        rb.angularVelocity = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;

        // Handle Shooting
        if (obj)
        {
            timeSinceShot += Time.deltaTime;
            rayShowing += Time.deltaTime;
            if (timeSinceShot > GunCoolDown)
            {
                shooting = true;
            }
            if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0.5f && shooting)
            {
                shooting = false;
                timeSinceShot = 0;
                rayShowing = 0;

                RaycastHit hit;
                if (Physics.Raycast(obj.transform.position, obj.transform.forward, out hit, range, mask))
                {
                    GameObject obj_hit = hit.transform.gameObject;
                    if (obj_hit.name == "Back")
                    {
                    } else if (obj_hit.name == "Start")
                    {
                        Utility.isTutorial = false;
                        Destroy(obj_hit.transform.parent.gameObject);
                        GetComponent<LevelGenerator>().generateFirstPath();
                    } else if (obj_hit.name == "Tutorial")
                    {
                        Utility.isTutorial = true;
                        Destroy(obj_hit.transform.parent.gameObject);
                        GetComponent<LevelGenerator>().generateFirstPath();
                    } else if (obj_hit.name == "Quit")
                    {
                        Utility.Exit();
                    } else
                    {
                        if (obj_hit.name == "RedWall")
                        {
                            changeWeapon();
                            speed = initialSpeed;
                        }
                        else
                        {
                            Utility.score += 1;
                        }
                        Destroy(obj_hit);
                        Sound_Destroy.Play();
                    }
                }
                Sound_Gun.Play();
            }
            if ((obj == gun_laser && rayShowing < GunShotTime) || (speed == 0 && !Utility.noMenu))
            {
                obj_lineRenderer.enabled = true;
                obj_lineRenderer.positionCount = 2;
                obj_lineRenderer.SetPosition(0, obj.transform.position);
                obj_lineRenderer.SetPosition(1, obj.transform.forward * range + obj.transform.position);
                obj_lineRenderer.startWidth = 0.05f;
                obj_lineRenderer.endWidth = 0.05f;
            } else if (speed != 0)
            {
                obj_lineRenderer.enabled = false;
            }
        }
    }

    public void incrementSpeed()
    {
       speed += incrementation;
    }

    private void changeWeapon(int gameState=-1)
    {
        if (speed != 0)
        {
            Sound_SwitchWeapon.Play();
        }
        if (gameState == -1)
        {
            gameState = Utility.gameState;
        }
        switch (gameState)
        {
            case 0:
                gun_laser.SetActive(true);
                obj = gun_laser;
                obj_lineRenderer = gun_laser.GetComponent<LineRenderer>();
                Sword_L_Green.SetActive(false);
                Sword_R_Blue.SetActive(false);
                Reticle.SetActive(false);
                break;
            case 1:
                Sword_L_Green.SetActive(true);
                Sword_R_Blue.SetActive(true);
                obj = null;
                gun_laser.SetActive(false);
                Reticle.SetActive(false);
                break;
            case 2:
                Reticle.SetActive(true);
                obj = Camera;
                gun_laser.SetActive(false);
                Sword_L_Green.SetActive(false);
                Sword_R_Blue.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "GroundCollider")
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destroy(other.gameObject);
        if (other.gameObject.name == "weapon_change_dagger")
        {
            if (Utility.isTutorial)
            {
                changeWeapon(1);
            }
            else
            {
                changeWeapon();
            }
        }
        else if (other.gameObject.name == "weapon_change_canon")
        {
            if (Utility.isTutorial)
            {
                changeWeapon(2);
            }
            else
            {
                changeWeapon();
            }

        }
        else if (other.gameObject.name == "weapon_change_gun")
        {
            if (Utility.isTutorial)
            {
                changeWeapon(0);
                Utility.isTutorial = false;
            }
            else
            {
                changeWeapon();
            }
        }
    }
}
