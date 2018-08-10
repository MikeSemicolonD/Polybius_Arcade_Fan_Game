using UnityEngine.UI;
using UnityEngine;

public class TextureAnimator : MonoBehaviour {
    
    public float delay = 0.025f;
    public Texture2D[] images;

    float currentTime;
    RawImage ImageRend;
    int index = 0;

	// Use this for initialization
	void Start ()
    {
        ImageRend = GetComponent<RawImage>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(currentTime < delay)
        {
            currentTime += Time.unscaledDeltaTime;
        }
        else
        {
            ImageRend.texture = images[index++];

            if(index >= images.Length)
            {
                index = 0;
            }

            currentTime = 0;
        }
	}
}
