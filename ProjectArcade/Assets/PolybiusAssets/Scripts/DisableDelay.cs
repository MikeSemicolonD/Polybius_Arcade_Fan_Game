using UnityEngine;

public class DisableDelay : MonoBehaviour
{
    public float TimeUntilDisable = 1.5f;
    public float TimeUntilEnable = 0.75f;
    public GameObject targetObject;

    private float DisableEnableTimer;

    private void Start()
    {
        DisableEnableTimer = TimeUntilDisable;
    }
    
    void Update()
    {
        if (DisableEnableTimer <= 0)
        {
            if (targetObject.activeSelf)
            {
                targetObject.SetActive(false);
                DisableEnableTimer = TimeUntilEnable;
            }
            else
            {
                targetObject.SetActive(true);
                DisableEnableTimer = TimeUntilDisable;
            }
        }
        else
        {
            DisableEnableTimer -= Time.deltaTime;
        }
    }
}
