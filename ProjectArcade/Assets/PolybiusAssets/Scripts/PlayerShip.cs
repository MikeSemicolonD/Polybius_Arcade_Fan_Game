using System.Collections.Generic;
using UnityEngine;

public class PlayerShip : MonoBehaviour {

    public Transform bossTarget;
    public GameObject PlayerExplosionObject;
    public GameObject ShieldObject;

    public float MaxX = 15;
    public float MinX = -15f;
    public float MaxY = 5;
    public float MinY = -5f;

    public new Animation animation;

    public float ProjectileSpeedMultiplier = 5;

    public Transform ProjectileSpawnPoint;
    public Transform ProjectilePoolParent;

    public float XInput, YInput;
    private List<Transform> Projectiles;
    private Vector3 movement;
    private bool dead;
    private bool canBeDamaged;
    private float StartXPosRef = 7;

    public float secondsTilCanGetDamaged = 5;
    private float RunTimeSecondsTilCanGetDamaged = 0;
    private bool BeingUsed;

    private void OnEnable()
    {
        transform.parent.localPosition = new Vector3(StartXPosRef, 0f, 0f);

        if (dead)
        {
            animation.Play();
            dead = false;
            canBeDamaged = false;
            ShieldObject.SetActive(!canBeDamaged);
            RunTimeSecondsTilCanGetDamaged = secondsTilCanGetDamaged;
            ShieldObject.GetComponentInChildren<Renderer>().material.SetFloat("_Thickness", RunTimeSecondsTilCanGetDamaged);
            PlayerExplosionObject.SetActive(dead);
        }
        else
        {
            animation.Play();
        }
    }

    private void OnDisable()
    {
        if(!canBeDamaged)
        {
            canBeDamaged = true;
            ShieldObject.SetActive(!canBeDamaged);
        }
    }

    //Get projectiles to be pool
    private void Start()
    {
        if (Projectiles == null)
        {
            Projectiles = new List<Transform>(ProjectilePoolParent.childCount);

            for (int i = 0; i < ProjectilePoolParent.childCount; i++)
            {
                Projectiles.Add(ProjectilePoolParent.GetChild(i));
            }
        }
    }

    public void Use()
    {
        if (!BeingUsed)
        {
            BeingUsed = true;
        }
        else
        {
            BeingUsed = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if (!dead)
        {
            if(!canBeDamaged)
            {
                if(RunTimeSecondsTilCanGetDamaged <= 0)
                {
                    canBeDamaged = true;
                    ShieldObject.SetActive(!canBeDamaged);
                }
                else
                {
                    RunTimeSecondsTilCanGetDamaged -= Time.deltaTime;
                    ShieldObject.GetComponentInChildren<Renderer>().material.SetFloat("_Thickness", RunTimeSecondsTilCanGetDamaged);
                }
            }

            transform.parent.LookAt(bossTarget);
            
            XInput = (BeingUsed) ? Input.GetAxis("Horizontal") : 0f;
            YInput = (BeingUsed) ? Input.GetAxis("Vertical") : 0f;

            movement = new Vector3(XInput, YInput, 0f) / 100;

            if (BeingUsed && Input.GetKeyDown(KeyCode.Space) && bossTarget.gameObject.activeSelf) //Fire projectile at direction of boss
            {
                for (int i = 0; i < Projectiles.Count; i++)
                {
                    if (!Projectiles[i].gameObject.activeSelf)
                    {
                        Projectiles[i].gameObject.SetActive(true);

                        Projectiles[i].position = ProjectileSpawnPoint.position;
                        Projectiles[i].rotation = ProjectileSpawnPoint.rotation;

                        if (Projectiles[i].parent != transform.parent.parent)
                            Projectiles[i].parent = transform.parent.parent;

                        Projectiles[i].GetComponent<Rigidbody2D>().velocity = ProjectileSpeedMultiplier * ProjectileSpawnPoint.TransformDirection(Vector3.forward);

                        break;
                    }
                }
            }

            if (YInput != 0 || XInput != 0) //Movement
            {
                transform.parent.Translate(movement, bossTarget);
            }

            if (transform.parent.localPosition.x <= MinX) //left
            {
                transform.parent.localPosition = new Vector3(MinX, transform.parent.localPosition.y, transform.parent.localPosition.z);
            }
            else if (transform.parent.localPosition.x >= MaxX) //right
            {
                transform.parent.localPosition = new Vector3(MaxX, transform.parent.localPosition.y, transform.parent.localPosition.z);
            }

            if (transform.parent.localPosition.y >= MaxY) //up
            {
                transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, MaxY, transform.parent.localPosition.z);
            }
            else if (transform.parent.localPosition.y <= MinY) //down
            {
                transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, MinY, transform.parent.localPosition.z);
            }
        }
        else
        {
            return;
        }
    }

    private void DisableProjectiles()
    {
        for (int i = 0; i < Projectiles.Count; i++)
        {
            Projectiles[i].gameObject.SetActive(false);
        }
    }

    public void ResetObject()
    {
        gameObject.SetActive(true);
        dead = false;
        DisableProjectiles();
    }

    public void ApplyDamage()
    {
        if (canBeDamaged)
        {
            dead = true;
            PlayerExplosionObject.SetActive(dead);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag.Equals("Enemy", System.StringComparison.Ordinal))
        {
            ApplyDamage();
        }
    }
}
