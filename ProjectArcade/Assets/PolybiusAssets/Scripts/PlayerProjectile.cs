using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    public float Damage = 25f;
    public GameObject explosionObj;
    public Transform player;

    private void Update()
    {
        if(System.Math.Abs(transform.localPosition.x) >= 9 || System.Math.Abs(transform.localPosition.y) >= 5)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Enemy", System.StringComparison.Ordinal))
        {
            if (!collision.transform.name.Equals("BossObject", System.StringComparison.Ordinal))
            {
                collision.transform.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                gameObject.SetActive(false);
            }
            else
            {
                collision.transform.SendMessage("ApplyDamage", Damage, SendMessageOptions.DontRequireReceiver);
                gameObject.SetActive(false);
            }
        }
    }
}
