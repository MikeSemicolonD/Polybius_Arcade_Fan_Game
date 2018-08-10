using System.Collections.Generic;
using UnityEngine;

public class MaterialColorFlicker : MonoBehaviour {

    public float delay = 1;
    private float runtimeDelay = 0;

    public Material mat;

    public List<Color> colors;

    int i = 0;

    // Update is called once per frame
    void Update()
    {
        if (runtimeDelay <= 0)
        {
            mat.color = colors[i++];

            if (i >= colors.Count)
            {
                i = 0;
            }

            runtimeDelay = delay;
        }
        else
        {
            runtimeDelay -= Time.deltaTime;
        }
    }
}
