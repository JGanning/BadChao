using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{
    private InputManager playerControls;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject carModel;


    private bool grounded = true;
    [SerializeField] private float gravityForce = 9.8f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundRayPoint;
    [SerializeField] private float groundRayLength = 2f;

    #region Accelerate variables
    [SerializeField] public float assignedAcceleration = 5f;
    [HideInInspector]  public float accelerationSpeed;
    private float speedInput = 0f;
    #endregion

    #region Turning variables
    [SerializeField] public float assignedTurning = 5f;
    [HideInInspector] public float turnStrength;
    private float turnInput = 0f;
    #endregion

    #region Drifting variables
    private bool isDrifting = false;
    //[SerializeField] private float hopStrength = 10f;
    [SerializeField] private float driftStrength = 1.5f;
    [SerializeField] private ParticleSystem[] sparks;
    [SerializeField] private Color[] sparksColor;
    private int driftDirection = 0;
    private float driftTime = 1f;
    private float driftCountdown;
    private int driftLevel = 0;
    #endregion

    #region Equipment variables
    private bool equipmentReady = true;
    [HideInInspector] public float equipmentDuration = 0f;
    [HideInInspector] public float equipmentCooldown = 0f;
    [HideInInspector] public EquipmentScriptableObject currentEquipment;

    // move this to equipment pieces
    [SerializeField] private ParticleSystem equipmentParticles;
    #endregion

    #region UI Elements
    [SerializeField] private Image equipmentIcon;
    #endregion

    private void OnEnable()
    {
        playerControls.Enable();

        playerControls.Player.Drift.started -= StartDrift;
        playerControls.Player.ConsumeItem.started -= UseEquipment;
    }

    private void OnDisable()
    {
        playerControls.Disable();

        playerControls.Player.Drift.started -= StartDrift;
        playerControls.Player.ConsumeItem.started -= UseEquipment;
    }

    private void Awake()
    {
        playerControls = new InputManager();

        accelerationSpeed = assignedAcceleration;
        turnStrength = assignedTurning;
        driftCountdown = driftTime;
    }

    private void Start()
    {
        rb.gameObject.transform.parent = null;

        playerControls.Player.Drift.performed += StartDrift;
        playerControls.Player.ConsumeItem.performed += UseEquipment;
    }

    private void Update()
    {
        Debug.Log(accelerationSpeed);
        speedInput = 0f;

        if (playerControls.Player.Accelerate.IsPressed())
        {
            speedInput = accelerationSpeed * 1000f;
        }

        if (!isDrifting) HandleTurning();
        if (isDrifting) HandleDrifting();

        transform.position = rb.transform.position;

        if (rb.velocity.magnitude > 2)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime, 0f));
        }
    }

    private void FixedUpdate()
    {
        grounded = false;

        RaycastHit hit;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if (grounded)
        {

        }
        else
        {
            rb.AddForce(Vector3.up * -gravityForce * 100f);
        }

        HandleAccelerate();
    }

    private void HandleAccelerate()
    {
        if (playerControls.Player.Accelerate.IsPressed())
        {
            rb.AddForce(transform.forward * accelerationSpeed * 1000f);
        }
    }

    private void HandleTurning()
    {
        turnInput = playerControls.Player.Turn.ReadValue<Vector2>().x;
    }

    private void StartDrift(InputAction.CallbackContext context)
    {
        isDrifting = true;

        if (playerControls.Player.Turn.ReadValue<Vector2>().x != 0)
        {
            if (playerControls.Player.Turn.ReadValue<Vector2>().x > 0)
                driftDirection = 1;
            else
                driftDirection = -1;
        }

        //Hop
        //rb.AddForce(transform.up * hopStrength * 1000f);
    }

    private void HandleDrifting()
    {
        if (playerControls.Player.Drift.IsPressed())
        {
            if (grounded)
            {
                if (!sparks[0].isPlaying)
                {
                    sparks[0].Play();
                    sparks[1].Play();
                }

                if (driftCountdown > 0)
                {
                    driftCountdown -= Time.deltaTime;
                }
                else
                {
                    driftCountdown = driftTime;
                    driftLevel++;
                    driftLevel = Mathf.Clamp(driftLevel, 0, 3);
                    sparks[0].startColor = sparksColor[driftLevel];
                    sparks[1].startColor = sparksColor[driftLevel];
                }

                // try increading factor to match drift input over time
                turnInput = ((playerControls.Player.Turn.ReadValue<Vector2>().x + driftDirection) / 2) * 0.3f;
                float driftInput = turnInput + ((playerControls.Player.Turn.ReadValue<Vector2>().x + driftDirection) / 2) * 1.1f;
                
                carModel.transform.rotation = Quaternion.Euler(carModel.transform.rotation.eulerAngles + new Vector3(0f, ClampAngle(driftInput * driftStrength * Time.deltaTime, -30, 30), 0f));
            }
        }
        else
        {
            isDrifting = false;
            driftDirection = 0;
            driftLevel = 0;
            sparks[0].startColor = sparksColor[0];
            sparks[1].startColor = sparksColor[0];
            driftCountdown = driftTime;

            if (sparks[0].isPlaying)
            {
                sparks[0].Stop();
                sparks[1].Stop();
            }
            
            transform.rotation = Quaternion.Euler(carModel.transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime, 0f));
            carModel.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, 0 * Time.deltaTime, 0f));
        }
    }

    public void EquipItem(EquipmentScriptableObject newEquipment)
    {
        currentEquipment = newEquipment;
        equipmentReady = true;
    }

    private void UseEquipment(InputAction.CallbackContext context)
    {
        if (currentEquipment != null && equipmentReady)
        {
            equipmentReady = false;
            currentEquipment.UseEquipment(this);
            StartCoroutine(EquipmentCooldown());
        }
    }

    private float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    IEnumerator EquipmentCooldown()
    {
        float usageCountdown = equipmentDuration;
        float countdown = 0f;

        equipmentParticles.Play();

        while (true)
        {
            usageCountdown -= Time.deltaTime;

            equipmentIcon.fillAmount = usageCountdown / equipmentDuration;

            if (usageCountdown <= 0)
            {
                break;
            }

            yield return null;
        }

        equipmentParticles.Stop();

        while (true)
        {
            countdown += Time.deltaTime;

            equipmentIcon.fillAmount = countdown / equipmentCooldown;

            if (countdown >= equipmentCooldown)
            {
                break;
            }

            yield return null;
        }
        
        equipmentIcon.fillAmount = 1;
        currentEquipment.EquipmentUsed(this);
        equipmentReady = true;
    }
}
