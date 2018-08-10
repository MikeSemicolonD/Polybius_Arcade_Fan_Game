using UnityEngine;

public class BossShip : MonoBehaviour
{
    public GameObject player;

    public bool dead;
    public float health = 200f;

    public float underMinNumOfShapesShootDelay = 0.25f;

    public int minNumberOfShapesToSpawn = 15;

    public float aboveMinNumOfShapesShootDelay = 0.5f;

    public int maxNumberOfShapesToSpawn = 35;

    public float passMaxNumOfShapesShootDelayMin = 2f;
    public float passMaxNumOfShapesShootDelayMax = 5f;

    private float runTimeShootDelay;

    public float timeBeforeRotationChange = 5f;
    private float runTimeRotationChange;
    public Vector3 rotateDegreesPerSecond;
    public float[] ZSpeeds;
    
    public GameObject ReturnableShapeParent;
    public GameObject ShapeParent;
    public GameObject BossExplosionObject;

    public BossProjectile[] ReturnableShapeInstances;
    public BossProjectile[] ShapeInstances;
    public int numberOfActiveShapes = 0;
    public int SpawnIncrement = 0;

    public GameObject[] MovingPlanes;

    public GameManager gameManager;

    public bool shieldsUp;
    public GameObject shieldObject;

    public new Animation animation;

    private int incrementalDirection;

    private void Start()
    {
        ReturnableShapeInstances = ReturnableShapeParent.GetComponentsInChildren<BossProjectile>();
        ShapeInstances = ShapeParent.GetComponentsInChildren<BossProjectile>();

        for (int i = 0; i < ReturnableShapeInstances.Length; i++)
        {
            ReturnableShapeInstances[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < ShapeInstances.Length; i++)
        {
            ShapeInstances[i].gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (shieldsUp)
            ShieldsDown();
    }

    private void OnEnable()
    {
        if (dead)
        {
            BossExplosionObject.SetActive(false);
            StartPlaneRotations();
            dead = false;
        }

        animation.Play();
    }
    
    private void Update ()
    {
        if (!dead)
        {
            transform.Rotate(rotateDegreesPerSecond * Time.deltaTime);

            if (runTimeRotationChange <= 0)
            {
                int index = Random.Range(0, ZSpeeds.Length);
                NewRotation(ZSpeeds[index]);
                runTimeRotationChange = timeBeforeRotationChange;

                int chanceOfShootingReturnable = (!shieldsUp) ? 1 : Random.Range(0, 10);
                
                if(chanceOfShootingReturnable == 0)
                {
                    ShootProjectile(incrementalDirection++,4,true);

                    if (incrementalDirection >= 4)
                    {
                        incrementalDirection = 0;
                    }
                }
            }
            else
            {
                runTimeRotationChange -= Time.deltaTime;
            }

            if(runTimeShootDelay <= 0)
            {
                int chanceOfShootingReturnable = (!shieldsUp) ? 1 : Random.Range(0, 10);

                if (chanceOfShootingReturnable == 0)
                {
                    ShootProjectile(incrementalDirection++, 4, true);

                    if (incrementalDirection >= 4)
                    {
                        incrementalDirection = 0;
                    }
                }

                if (numberOfActiveShapes <= minNumberOfShapesToSpawn)
                {
                    ShootProjectile(incrementalDirection++, 0);

                    if(incrementalDirection >= 4)
                    {
                        incrementalDirection = 0;
                    }

                    runTimeShootDelay = underMinNumOfShapesShootDelay;

                }
                else if (numberOfActiveShapes > minNumberOfShapesToSpawn && numberOfActiveShapes <= maxNumberOfShapesToSpawn)
                {
                    ShootProjectile(incrementalDirection++, Random.Range(1,4));

                    if (incrementalDirection >= 4)
                    {
                        incrementalDirection = 0;
                        SendRandomShapeToPlayer();
                    }

                    runTimeShootDelay = aboveMinNumOfShapesShootDelay;
                }
                else
                {
                    runTimeShootDelay = Random.Range(passMaxNumOfShapesShootDelayMin, passMaxNumOfShapesShootDelayMax);

                    //Select random shape to fly towards player
                    SendRandomShapeToPlayer();
                }
            }
            else
            {
                runTimeShootDelay -= Time.deltaTime;
            }
        }
        else
        {
            return;
        }
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public void LostAProjectile(bool returnable, bool addPoints)
    {
        if (addPoints)
        {
            if (!returnable)
                gameManager.AddPoints(125);
            else
                gameManager.AddPoints(275);
        }

        numberOfActiveShapes--;
    }

    private void SendRandomShapeToPlayer()
    {
        for (int i = 0; i < ShapeInstances.Length; i++)
        {
            if (ShapeInstances[i].gameObject.activeSelf && !ShapeInstances[i].goingToPlayer() && player.gameObject.activeSelf)
            {
                ShapeInstances[i].GoToPlayer(ref player);
                break;
            }
        }
    }

    private void ShootProjectile(int direction, int multiplier,bool returnable = false)
    {
        int index = Random.Range(0, MovingPlanes.Length);

        if (!returnable)
        {
            for (int i = 0; i < ShapeInstances.Length; i++)
            { 
                if(!ShapeInstances[i].gameObject.activeSelf && !ShapeInstances[i].explosionEnabled())
                {
                    ShapeInstances[i].gameObject.SetActive(true);
                    ShapeInstances[i].transform.parent = transform;
                    ShapeInstances[i].transform.localPosition = Vector2.zero;
                    ShapeInstances[i].SetDirectionAndMultiplier(direction, multiplier);
                    ShapeInstances[i].EndPositionPlane(MovingPlanes[index], this);
                    numberOfActiveShapes++;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < ReturnableShapeInstances.Length; i++)
            {
                if (!ReturnableShapeInstances[i].gameObject.activeSelf)
                {
                    ReturnableShapeInstances[i].gameObject.SetActive(true);
                    ReturnableShapeInstances[i].transform.parent = transform;
                    ReturnableShapeInstances[i].transform.localPosition = Vector2.zero;
                    ReturnableShapeInstances[i].SetDirectionAndMultiplier(direction, multiplier);
                    ReturnableShapeInstances[i].EndPositionPlane(MovingPlanes[index], this);
                    ReturnableShapeInstances[i].GoToPlayer(ref player);
                    break;
                }
            }
        }
    }

    public void DisableActiveShapes()
    {
        for (int i = 0; i < ShapeInstances.Length; i++)
        {
            if (ShapeInstances[i].gameObject.activeSelf || ShapeInstances[i].explosionObject.activeSelf)
            {
                ShapeInstances[i].ReturnAndDisable();
            }
        }

        numberOfActiveShapes = 0;
    }

    private void StartPlaneRotations()
    {
        for (int i = 0; i < MovingPlanes.Length; i++)
        {
            MovingPlanes[i].GetComponent<AutoMoveAndRotate>().enabled = true;
        }
    }

    private void StopPlaneRotations()
    {
        for (int i = 0; i < MovingPlanes.Length; i++)
        {
            if(!MovingPlanes[i].name.Equals("BossObject",System.StringComparison.Ordinal))
            MovingPlanes[i].GetComponent<AutoMoveAndRotate>().enabled = false;
        }
    }

    private void NewRotation(float newZ)
    {
        rotateDegreesPerSecond = new Vector3(0f, 0f, newZ);
    }

    private void ShieldsUp()
    {
        shieldsUp = true;
        shieldObject.SetActive(shieldsUp);
    }

    public void ShieldsDown()
    {
        shieldsUp = false;
        shieldObject.SetActive(shieldsUp);
    }

    public void ReturnToSenderDamage()
    {
        ShieldsDown();
    }

    public void DisableExplosion()
    {
        BossExplosionObject.SetActive(false);
    }

    public void ApplyDamage(float amount)
    {
        if (!shieldsUp)
        {
            health -= amount;

            if (health <= 0)
            {
                DisableActiveShapes();
                StopPlaneRotations();
                BossExplosionObject.SetActive(true);
                gameManager.NextRound();
                dead = true;
                gameObject.SetActive(false);
            }
            else
            {
                int shieldChance = Random.Range(0, 50);

                if(shieldChance == 0)
                {
                    ShieldsUp();
                }
            }
        }
    }
}
