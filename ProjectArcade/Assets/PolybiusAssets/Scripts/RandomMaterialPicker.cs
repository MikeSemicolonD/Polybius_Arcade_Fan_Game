using UnityEngine;

public class RandomMaterialPicker : MonoBehaviour {

    public Material[] materialColors;
    MeshRenderer[] meshRenderers;

	// Use this for initialization
	void OnEnable ()
    {
        int selectedColor = Random.Range(0, materialColors.Length);

        meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        for(int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = materialColors[selectedColor];
        }
	}
}
