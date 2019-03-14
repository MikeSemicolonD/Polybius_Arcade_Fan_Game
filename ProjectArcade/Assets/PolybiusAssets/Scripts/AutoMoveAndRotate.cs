using System;
using UnityEngine;

public class AutoMoveAndRotate : MonoBehaviour
{
    public bool randomlyInvertSpeed;
    public Vector3andSpace moveUnitsPerSecond;
    public Vector3andSpace rotateDegreesPerSecond;
    
    public bool randomizeRotationOnStart;
    public Vector3 MaxRot;
    public Vector3 MinRot;

    public void Start()
    {
        if (randomizeRotationOnStart)
        {
            float x = UnityEngine.Random.Range(MinRot.x, MaxRot.x);
            float y = UnityEngine.Random.Range(MinRot.y, MaxRot.y);
            float z = UnityEngine.Random.Range(MinRot.z, MaxRot.z);

            transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
        }

        if(randomlyInvertSpeed && UnityEngine.Random.Range(0,2) == 0)
        {
            float x = 0f-moveUnitsPerSecond.value.x;
            float y = 0f-moveUnitsPerSecond.value.y;
            float z = 0f-moveUnitsPerSecond.value.z;

            moveUnitsPerSecond.value = new Vector3(x, y, z);

            x = 0f - rotateDegreesPerSecond.value.x;
            y = 0f - rotateDegreesPerSecond.value.y;
            z = 0f - rotateDegreesPerSecond.value.z;

            rotateDegreesPerSecond.value = new Vector3(x, y, z);
        }
    }
    
    private void Update()
    {
        float deltaTime = Time.deltaTime;
        transform.Translate(moveUnitsPerSecond.value * deltaTime, moveUnitsPerSecond.space);
        transform.Rotate(rotateDegreesPerSecond.value * deltaTime, moveUnitsPerSecond.space);
    }
    
    [Serializable]
    public class Vector3andSpace
    {
        public Vector3 value;
        public Space space = Space.Self;
    }
}
