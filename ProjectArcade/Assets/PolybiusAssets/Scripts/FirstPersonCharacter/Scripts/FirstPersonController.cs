using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private bool m_IsCrouching;
    [SerializeField] private bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce;
    [SerializeField] private float m_GravityMultiplier;
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private float m_StepInterval;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

    public Vector3 desiredMove;
    private Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private float currentSpeed;
    private float speed;
    private bool m_Jumping;
    private AudioSource m_AudioSource;
    private const float crouchTranslateConst = 0.39762f;
    private float characterControllerHeight = 1.8f;
    private bool waswalking;
    private bool instantiated;

    // Use this for initialization
    private void Start()
    {
        if (!instantiated)
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            //m_AudioSource.volume = PlayerPrefs.GetFloat("Sound");
            m_AudioSource.enabled = false;
            instantiated = true;
        }
    }

    private void Awake()
    {
        if (m_AudioSource == null)
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        m_AudioSource.volume = PlayerPrefs.GetFloat("Sound");
    }

    // Update is called once per frame
    private void Update()
    {
        GetInput();

        RotateView();

        UpdateMovement();

        if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
        {

            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_CharacterController.isGrounded;
    }

    public void reduceSpeed(int multiplier)
    {
        m_WalkSpeed /= multiplier;
        m_RunSpeed /= multiplier;
    }

    public void increaseSpeed(int multiplier)
    {
        m_WalkSpeed *= multiplier;
        m_RunSpeed *= multiplier;
    }

    public bool isCrouching()
    {
        return m_IsCrouching;
    }

    public bool AudioSourceIsPlaying()
    {
        return m_AudioSource.isPlaying;
    }

    private void PlayLandingSound()
    {
        if (!m_AudioSource.enabled)
        {
            m_AudioSource.enabled = true;
        }
        else
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }

    private void UpdateMovement()
    {

        if (m_Input.y != 0)
        {
            currentSpeed = (m_IsWalking) ? m_Input.y : m_Input.y;
        }
        else if (m_Input.x != 0)
        {
            currentSpeed = (m_IsWalking) ? m_Input.x : m_Input.x;
        }
        else
        {
            currentSpeed = 0;
        }

        // always move along the camera forward as it is the direction that it being aimed at
        desiredMove = (m_Camera.transform.forward * m_Input.y + m_Camera.transform.right * m_Input.x).normalized;

        m_MoveDir.x = desiredMove.x * speed;
        m_MoveDir.z = desiredMove.z * speed;


        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;

            if (m_Jump)
            {
                m_MoveDir.y = m_JumpSpeed;
                PlayJumpSound();
                m_Jump = false;
                m_Jumping = true;
            }
        }
        else
        {
            m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
        }

        if (m_IsCrouching && m_CharacterController.height == characterControllerHeight)
        {
            m_CharacterController.height /= 2;
            transform.position = new Vector3(transform.position.x, transform.position.y - crouchTranslateConst, transform.position.z);
            m_Camera.transform.Translate(0, 0.385f, 0);
        }

        if (!m_IsCrouching)
        {
            if (m_CharacterController.height != characterControllerHeight)
            {
                m_CharacterController.height = characterControllerHeight;
                transform.position = new Vector3(transform.position.x, transform.position.y + crouchTranslateConst, transform.position.z);
                m_Camera.transform.Translate(0, 0, 0);
            }
        }

        m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);

        m_MouseLook.UpdateCursorLock();
    }


    private void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }


    private void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                         Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!m_CharacterController.isGrounded || m_IsCrouching)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }


    private void UpdateCameraPosition(float speed)
    {
        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                  (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }


    private void GetInput()
    {
        // Read input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        waswalking = m_IsWalking;

       // m_IsCrouching = Input.GetButton("Crouch");
        
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        //if (!m_IsCrouching)
        {
         //   m_IsWalking = !Input.GetButton("Sprint");

          //  if (!m_Jump)
            {
          //      m_Jump = Input.GetButtonDown("Jump");
            }
        }
        
        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }


    private void RotateView()
    {
        m_MouseLook.LookRotation(m_Camera.transform);
    }

    public void UnlockCursor()
    {
        m_MouseLook.SetCursorLock(false);
    }

    public void LockCursor()
    {
        m_MouseLook.SetCursorLock(true);
    }

    public void FOVKickDown()
    {
        StopAllCoroutines();
        StartCoroutine(m_FovKick.FOVKickUp());
    }

    public void FOVKickUp()
    {
        StopAllCoroutines();
        StartCoroutine(m_FovKick.FOVKickDown());
    }

    public bool IsRunning()
    {
        return (m_IsWalking) ? false : true;
    }

    public CharacterController GetPhysicsController()
    {
        return m_CharacterController;
    }
}

