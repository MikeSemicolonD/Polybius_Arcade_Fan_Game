using UnityEngine;

public class DebugDisableOnUse : MonoBehaviour {

    bool beingUsed;

    public GameManager manager;
    public Transform cameraSpotToBe;
    public Transform spotToBe;
    public float xRot = 15f;
    public GameObject backgroundObjects;
    public GameObject fpsCharacter;
    public GameObject fpsCharacterCamera;

    // Update is called once per frame
    void Update ()
    {
        if(!beingUsed && (manager.transform.position - fpsCharacter.transform.position).magnitude <= 1.5f && Input.GetButtonDown("Use"))
        {
            fpsCharacter.transform.position = spotToBe.position;
            fpsCharacter.transform.rotation = spotToBe.rotation;
            fpsCharacterCamera.transform.eulerAngles = new Vector3(xRot, 0f, 0f);
            fpsCharacter.GetComponent<FirstPersonController>().enabled = beingUsed;
            fpsCharacterCamera.transform.position = cameraSpotToBe.position;
            manager.Use();
            backgroundObjects.SetActive(false);
            beingUsed = true;
        }
        else if(beingUsed && Input.GetButtonDown("Use"))
        {
            fpsCharacter.GetComponent<FirstPersonController>().enabled = beingUsed;
            fpsCharacterCamera.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            manager.Use();
            backgroundObjects.SetActive(true);
            beingUsed = false;
        }
        else
        {
            return;
        }
	}
}
