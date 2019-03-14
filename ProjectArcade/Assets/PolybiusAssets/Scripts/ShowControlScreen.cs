using UnityEngine;

public class ShowControlScreen : MonoBehaviour
{
    public GameObject ControlScreen;

    void Update()
    {
        if (!ControlScreen.activeSelf && Input.GetButtonDown("Controls"))
        {
            ControlScreen.SetActive(true);
        }
        else if (ControlScreen.activeSelf && Input.GetButtonUp("Controls"))
        {
            ControlScreen.SetActive(false);
        }
        else
            return;
    }
}
