using UnityEngine;

public class ExplosionReturn : MonoBehaviour {

    public Transform parentObj;
    bool transitionMidExplosion;

    private void OnDisable()
    {
        if (transform.parent != parentObj)
        transitionMidExplosion = true;
    }

    private void OnEnable()
    {
        if (transitionMidExplosion)
            returnToParent();
    }

    public void returnToParent()
    {
        transform.parent = parentObj;
        transitionMidExplosion = false;
        gameObject.SetActive(false);
    }
}
