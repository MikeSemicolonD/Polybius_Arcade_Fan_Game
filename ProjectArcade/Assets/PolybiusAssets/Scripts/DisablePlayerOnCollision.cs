using UnityEngine;

public class DisablePlayerOnCollision : MonoBehaviour
{
    public GameObject InitialStateObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player", System.StringComparison.Ordinal))
        {
            other.transform.position = new Vector3(-2.7825f, 17.01687f, 15f);
            other.gameObject.SetActive(false);
            InitialStateObject.SetActive(true);
        }
    }
}
