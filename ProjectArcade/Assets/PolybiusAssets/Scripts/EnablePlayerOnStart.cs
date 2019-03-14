using UnityEngine;

public class EnablePlayerOnStart : MonoBehaviour
{
    public GameObject Player;
    
    void Update()
    {
        if (Input.GetButtonDown("Back") || Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            Debug.Log("Quit called");
        }
        else if ((Input.GetButtonDown("Start") || Input.GetButtonDown("Fire")) && !Player.gameObject.activeSelf)
        {
            Player.SetActive(true);
            Player.transform.position = new Vector3(-2.7825f, 17.01687f, 15f);
            Player.transform.GetChild(0).rotation = new Quaternion(0,0,0,0);
            gameObject.SetActive(false);
        }
    }
}
