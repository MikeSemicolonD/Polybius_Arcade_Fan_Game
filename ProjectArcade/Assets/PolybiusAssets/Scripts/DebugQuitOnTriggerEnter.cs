using UnityEngine;

public class DebugQuitOnTriggerEnter : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player",System.StringComparison.Ordinal))
        {
            Application.Quit();
        }
    }
}
