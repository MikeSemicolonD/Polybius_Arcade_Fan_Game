using UnityEngine;

public class BossProjectile : MonoBehaviour {

    Vector3 playerPostion;
    public GameObject[] ShapeAppearence;
    public bool ReturnToSenderOnHit;
    public new Rigidbody2D rigidbody2D;
    bool returning;
    bool getToPosition;
    GameObject restingPlane;
    BossShip bossInstance;
    float speed = 10f;
    float step;
    Vector2[] directions = { Vector2.right, Vector2.up, Vector2.left, Vector2.down};
    int setDirection = 0;
    float[] directionMultiplier = { 6.0f, 7.5f, 9.0f, 10.5f, 12.0f };
    int setMultiplier = 0;
    private bool goToPlayer;
    public GameObject explosionObject;

    public bool goingToPlayer()
    {
        return goToPlayer;
    }

    private void OnEnable()
    {
        //Enable a random shape 
        if (!ReturnToSenderOnHit)
        {
            transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            SelectShape(Random.Range(0,ShapeAppearence.Length));
        }
        else
        {
            gameObject.layer = 11;
            transform.localScale = new Vector3(0.3624098f, 0.3624098f, 0.3624098f);
        }

        explosionObject.SetActive(false);
        getToPosition = true;
        goToPlayer = false;
    }

    public void ReturnAndDisable()
    {
        transform.parent = bossInstance.transform;
        transform.localPosition = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void SetDirectionAndMultiplier(int directionIndex, int multiplierIndex)
    {
        setDirection = directionIndex;
        setMultiplier = multiplierIndex;
    }

    private void SelectShape(int index)
    {
        for(int i = 0; i < ShapeAppearence.Length; i++)
        {
            if(index == i)
            {
                ShapeAppearence[i].SetActive(true);
            }
            else
            {
                ShapeAppearence[i].SetActive(false);
            }
        }
    }

    public void EndPositionPlane(GameObject plane, BossShip boss)
    {
        restingPlane = plane;
        bossInstance = boss;
    }

    public void GoToPlayer(ref GameObject player)
    {
        if (getToPosition)
            getToPosition = false;

        transform.parent = bossInstance.transform.parent;
        playerPostion = player.transform.localPosition;

        goToPlayer = true;
    }

    // Update is called once per frame
    void Update ()
    {
        step = speed * Time.deltaTime;

        if(getToPosition)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, transform.TransformDirection(directions[setDirection] * directionMultiplier[setMultiplier]),step);
            
            if((transform.TransformDirection(directions[setDirection] * directionMultiplier[setMultiplier]) - transform.localPosition).magnitude <= 0.01f)
            {
                getToPosition = false;
                transform.parent = restingPlane.transform;
            }
            else if (transform.localScale.x >= 3f)
            {
                transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            }
            else
            {
                return;
            }
        }
        else if(goToPlayer)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, playerPostion * 4, step);


            if (System.Math.Abs(transform.localPosition.x) >= 20 || System.Math.Abs(transform.localPosition.y) >= 11)
            {
                bossInstance.LostAProjectile(ReturnToSenderOnHit, false);
                gameObject.SetActive(false);
                goToPlayer = false;
            }
            else if ((playerPostion - transform.position).magnitude-23 <= 0.55f)
            {
                bossInstance.LostAProjectile(ReturnToSenderOnHit, false);
                gameObject.SetActive(false);
                goToPlayer = false;
            }
            else
            {
                return;
            }
        }
        else if(returning)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, transform.TransformDirection(bossInstance.transform.localPosition), step);
            
            if((bossInstance.transform.localPosition - transform.localPosition).magnitude <= 0.1f)
            {
                bossInstance.ReturnToSenderDamage();
                returning = false;
                gameObject.SetActive(false);
            }
            else
            {
                return;
            }
        }
        else
        {
            return;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Player", System.StringComparison.Ordinal))
        {
            collision.transform.GetComponent<PlayerShip>().ApplyDamage();

            if (!ReturnToSenderOnHit)
            {
                bossInstance.LostAProjectile(ReturnToSenderOnHit, true);
                explosionObject.transform.localPosition = Vector2.zero;
                explosionObject.transform.parent = transform.parent;
                explosionObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

    public bool explosionEnabled()
    {
        return explosionObject.activeSelf;
    }

    public void ApplyDamage(float amount)
    {
        if (ReturnToSenderOnHit)
        {
            gameObject.layer = 9;
            goToPlayer = false;
            returning = true;
        }
        else
        {
            bossInstance.LostAProjectile(ReturnToSenderOnHit, true);
            explosionObject.transform.localPosition = Vector2.zero;
            explosionObject.transform.parent = transform.parent;
            explosionObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
