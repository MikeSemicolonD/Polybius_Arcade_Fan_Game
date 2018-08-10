using System;
using UnityEngine;

public class AutoMoveAndRotate : MonoBehaviour
{
    public Vector3andSpace moveUnitsPerSecond;
    public Vector3andSpace rotateDegreesPerSecond;
    
    // Update is called once per frame
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
