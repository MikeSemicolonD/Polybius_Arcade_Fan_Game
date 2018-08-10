using System;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes axes = RotationAxes.MouseXAndY;

        public float XSensitivity = 2f;
        public float YSensitivity = 2f;

        public float minimumX = -360F;
        public float maximumX = 360F;

        public float minimumY = -60F;
        public float maximumY = 60F;

        public bool smooth;
        public float smoothTime = 5f;

        public bool lockCursor = true;

        private float yRot,xRot;

        private bool m_cursorIsLocked = true;
        
        public void LookRotation(Transform camera)
        {

            if (axes == RotationAxes.MouseXAndY)
            {
                xRot = camera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * XSensitivity;

                yRot += Input.GetAxis("Mouse Y") * YSensitivity;
                yRot = Mathf.Clamp(yRot, minimumY, maximumY);

                camera.transform.localEulerAngles = new Vector3(-yRot, xRot, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                camera.transform.Rotate(0, Input.GetAxis("Mouse X") * XSensitivity, 0);
            }
            else
            {
                yRot += Input.GetAxis("Mouse Y") * YSensitivity;
                yRot = Mathf.Clamp(yRot, minimumY, maximumY);

                camera.transform.localEulerAngles = new Vector3(-yRot, camera.transform.localEulerAngles.y, 0);
            }

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            if(!value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
