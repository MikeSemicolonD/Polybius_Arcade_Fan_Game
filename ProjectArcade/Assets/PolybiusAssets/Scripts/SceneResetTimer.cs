using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneResetTimer : MonoBehaviour
{

    public float timeUntilReset = 180f;

    public float realtimeTimer;

    private void Start()
    {
        realtimeTimer = timeUntilReset;
    }

    // Update is called once per frame
    void Update()
    {
        //If timer hits 0        or        A + Start + B + X
        if(realtimeTimer <= 0 || (Input.GetButton("Fire") && Input.GetButton("Start") && Input.GetButton("Back") && Input.GetButton("Use")))
        {
            SceneManager.LoadScene(gameObject.scene.name);
        }

        if (Input.GetButtonDown("Use") || Input.GetButtonDown("Controls") || Input.GetButtonDown("Back") || Input.GetButtonDown("Start") || Input.GetButtonDown("Fire") || Input.GetAxis("Vertical") >= 0.75f || Input.GetAxis("Vertical") <= -0.75f || Input.GetAxis("Horizontal") <= -0.75f || Input.GetAxis("Horizontal") >= 0.75f)
        {
            realtimeTimer = timeUntilReset;
        }
        else
        {
            realtimeTimer -= Time.deltaTime;
        }
    }
}
