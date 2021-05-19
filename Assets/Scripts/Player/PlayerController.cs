using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    ////// CONSTANT VARIABLES //////

    // The default movement speed of the player
    [SerializeField]
    private float INIT_SPEED = 10;

    // The sprint movement speed of the player
    [SerializeField]
    private float SPRINT_SPEED = 20;

    // The crouch movement speed of the player
    [SerializeField]
    private float CROUCH_SPEED = 2;

    // Force multiplier used when the player jumps
    [SerializeField]
    private float JUMP_FORCE = 20;

    // Minimum time between player jumps
    [SerializeField]
    private float JUMP_TIME = 0.1f;

    // Force multiplier used when the player jumps
    [SerializeField]
    private float MAX_SLIDE_VELOCITY = 30;

    // Force multiplier used when the player jumps
    [SerializeField]
    private float MIN_SLIDE_VELOCITY = 20;

    // Maximum number of times the player can jump
    [SerializeField]
    private int NUM_JUMPS = 1;

    // Multiplier used to influence horizontal air drag
    [SerializeField]
    private float HORIZONTAL_AIR_DRAG_MULTIPLIER = 0.8f;

    // Multiplier used to influence horizontal air drag
    [SerializeField]
    private float VERTICAL_AIR_DRAG_MULTIPLIER = 1;

    // Multiplier used to influence horizontal friction
    [SerializeField]
    private float HORIZONTAL_FRICTION_MULTIPLIER = 0.02f;

    // Multiplier used to influence vertical friciton
    [SerializeField]
    private float VERTICAL_FRICTION_MULTIPLIER = 0f;

    // Maximum vertical look constraint
    [SerializeField]
    private float LOOK_CONSTRAINT_Y_MAX = 90;

    // Minimum vertical look constraint
    [SerializeField]
    private float LOOK_CONSTRAINT_Y_MIN = -90;

    // Maximum vertical look constraint
    [SerializeField]
    private float LOOK_CONSTRAINT_X_MAX = 60;

    // Minimum vertical look constraint
    [SerializeField]
    private float LOOK_CONSTRAINT_X_MIN = -60;

    // The player's horizontal look sensitivity
    [SerializeField]
    private float LOOK_SENSITIVITY_X = 1;

    // The player's vertical look sensitivity
    [SerializeField]
    private float LOOK_SENSITIVITY_Y = 1;

    // Gravity Mulitplier used to influence rigidbody downward velocity
    [SerializeField]
    private float GRAVITY_MULTIPLIER = 1.5f;


    ////// STANDARD VARIABLES //////

    // The current movement speed of the player
    private float movementSpeed;

    // Float used to clamp vertical looking constraints
    private float yLookClamp = 0;

    // Float used to clamp vertical looking constraints
    private float xLookClamp = 0;

    // The current number of jumps that the player has used
    private int currentJumps = 0;

    // Boolean used to check if the player is on touching the ground
    private bool grounded;

    // Boolean used to check if the player is on touching the ground
    private bool canJump;

    // Boolean used to check if the player has accelerated (a value of zero triggers crouching behavior)
    private bool canCheckSlide;

    // Boolean used to check if the player is on touching the ground
    private bool canAttack = true;

    // Enum used to define player movement state
    private enum State
    {
        Default,
        Sprinting,
        Crouched,
        Sliding,
    }

    // The current movement state of the player
    private State state = State.Default;


    ////// MONO VARIABLES //////

    // Upper body player transform
    private Transform playerUpper;

    // The rotational offset of the upper body relative to the player
    private float upperHorizontalRotationalOffset = 0;

    //
    private Weapon weapon;

    //
    private GameObject weaponObject;

    // Rigidbody of the player
    private Rigidbody rb;


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {

        // Set Variables
        //playerUpper = gameObject.GetComponentInChildren<Transform>();
        rb = GetComponent<Rigidbody>();
        playerUpper = GetComponentsInChildren<Transform>()[1];
        weapon = GetComponentInChildren<Weapon>();
        weaponObject = weapon.gameObject;

        movementSpeed = INIT_SPEED;

        // Pull from Config Data


        // Initialize State
        MouseUtils.LockMouse();

    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        AdjustGravity();

        ////// HANDLE INPUT //////
        
        Look();

        if (grounded)
        {

            // Check for sprint
            if (Input.GetButtonDown("Sprint") && state != State.Sprinting)
            {
                Sprint();
            }
            else if (state == State.Sprinting && (Input.GetButtonUp("Sprint") || Input.GetAxisRaw("Vertical") < 1))
            {
                ResetMovement();
            }

            // Check for crouch
            if (Input.GetButtonDown("Crouch") && state != State.Crouched)
            {
                Crouch();
            }
            else if (state == State.Crouched && Input.GetButtonUp("Crouch"))
            {
                ResetMovement();
            }
        }

        // Check for jump button
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Check for Attack Button
        if (canAttack)
        {
            if (Input.GetButton("Fire") && weapon.type == Weapon.Type.Auto) 
            {
                Shoot();
            } else if (Input.GetButtonDown("Fire") && weapon.type == Weapon.Type.Semi) 
            {
                Shoot();
            }
        }

        // Check for ADS Button
        if (Input.GetButtonDown("Aim"))
        {

        }

        // Check for Special Button
        if (Input.GetButtonDown("Special"))
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        // Check for movement
        HandleMove((int)Input.GetAxisRaw("Vertical"), (int)Input.GetAxisRaw("Horizontal"));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag is "Ground")
        {
            grounded = true;
            canJump = true;
            currentJumps = 0;


            // Check input states
            if (Input.GetButton("Crouch"))
            {
                Slide();
            }
            else if (Input.GetButton("Sprint") && state == State.Sprinting)
            {
                Sprint();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.tag is "Ground")
        {
            grounded = false;
        }
    }


    ////// PRIVATE //////

    /// <summary>
    /// Adjusts the rigidbody velocity by the gravity multiplier when falling. 
    /// </summary>
    private void AdjustGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity -= Vector3.up * Physics2D.gravity.y * -GRAVITY_MULTIPLIER * Time.deltaTime;
        }
    }

    /// <summary>
    /// Rotates player transforms with look inputs.
    /// </summary>
    private void Look()
    {
        float inputX = Input.GetAxis("Mouse X") * LOOK_SENSITIVITY_Y;
        float inputY = Input.GetAxis("Mouse Y") * LOOK_SENSITIVITY_X;

        Vector3 rotPlayerUpper = playerUpper.localRotation.eulerAngles;

        yLookClamp -= inputY;
        yLookClamp = Mathf.Clamp(yLookClamp, LOOK_CONSTRAINT_Y_MIN, LOOK_CONSTRAINT_Y_MAX);
        if (yLookClamp >= LOOK_CONSTRAINT_Y_MAX)
        {
            rotPlayerUpper.x = LOOK_CONSTRAINT_Y_MAX;
        }
        else if (yLookClamp <= LOOK_CONSTRAINT_Y_MIN)
        {
            rotPlayerUpper.x = LOOK_CONSTRAINT_Y_MIN;
        }
        else
        {
            rotPlayerUpper.x -= inputY;
        }


        // clamp horizontal if sliding
        xLookClamp += inputX;
        xLookClamp = Mathf.Clamp(xLookClamp, LOOK_CONSTRAINT_X_MIN, LOOK_CONSTRAINT_X_MAX);
        Vector3 rotPlayer = transform.rotation.eulerAngles;
        if (state == State.Sliding)
        {
            if (xLookClamp >= LOOK_CONSTRAINT_X_MAX)
            {
                rotPlayerUpper.y = LOOK_CONSTRAINT_X_MAX;
            }
            else if (xLookClamp <= LOOK_CONSTRAINT_X_MIN)
            {
                rotPlayerUpper.y = LOOK_CONSTRAINT_X_MIN;
            }
            else
            {
                rotPlayerUpper.y += inputX;
                upperHorizontalRotationalOffset = rotPlayerUpper.y;
            }
        }
        else
        {
            // fix rotational offset caused by sliding
            if (upperHorizontalRotationalOffset != 0)
            {
                //convert to global rotation
                rotPlayer.y += upperHorizontalRotationalOffset;
                rotPlayerUpper.y = 0;
                xLookClamp = 0;
                upperHorizontalRotationalOffset = 0;
            }
            else
            {
                rotPlayer.y += inputX;
            }
        }

        playerUpper.localRotation = Quaternion.Euler(rotPlayerUpper);
        transform.rotation = Quaternion.Euler(rotPlayer);
    }

    /// <summary>
    /// Multiplies directional input by friction/drag multipliers based on current state.
    /// </summary>
    /// <param name="verticalDirection"></param>
    /// <param name="horizontalDirection"></param>
    private void HandleMove(int verticalDirection = 0, int horizontalDirection = 0)
    {
        if (state == State.Sliding)
        {
            // Ajdust for slide
            Slide();
            Move(verticalDirection * VERTICAL_FRICTION_MULTIPLIER * rb.velocity.x, horizontalDirection * HORIZONTAL_FRICTION_MULTIPLIER * rb.velocity.z);
        }
        else if (!grounded)
        {
            // Adjust for air drag
            if (verticalDirection < 0)
            {
                Move(verticalDirection * VERTICAL_AIR_DRAG_MULTIPLIER, horizontalDirection * HORIZONTAL_AIR_DRAG_MULTIPLIER);
            }
            else
            {
                Move(verticalDirection, horizontalDirection * HORIZONTAL_AIR_DRAG_MULTIPLIER);
            }
        }
        else
        {
            // Default
            Move(verticalDirection, horizontalDirection);
        }
    }

    /// <summary>
    /// Moves player by the given horizontal and vertical movement multipliers.
    /// </summary>
    /// <param name="verticalMovementMultiplier"></param>
    /// <param name="horizontalMovementMultiplier"></param>
    private void Move(float verticalMovementMultiplier, float horizontalMovementMultiplier)
    {
        rb.MovePosition(transform.position + (transform.forward * movementSpeed * verticalMovementMultiplier * Time.deltaTime)
            + (transform.right * movementSpeed * horizontalMovementMultiplier * Time.deltaTime));
    }

    /// <summary>
    /// Adds an upward impulse force to the player.
    /// </summary>
    private void Jump()
    {
        if ((grounded || currentJumps < NUM_JUMPS) && canJump)
        {
            rb.AddForce(transform.up * JUMP_FORCE, ForceMode.Impulse);
            currentJumps++;

            StartCoroutine(LimitJumpTime());

            // Animator
        }
    }

    /// <summary>
    /// Toggles <see cref="canJump"/> for <see cref="JUMP_TIME"/> in order to prevent multiple jump calls before ground exit trigger is registered.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LimitJumpTime() 
    {
        canJump = false;
        yield return new WaitForSeconds(JUMP_TIME);
        canJump = true;
        yield break;
    }

    /// <summary>
    /// Changes player movement speed to "SPRINT_SPEED" and player state to "Sprinting"
    /// </summary>
    private void Sprint()
    {
        if (Input.GetAxisRaw("Vertical") > 0 && state != State.Sliding)
        {
            state = State.Sprinting;
            movementSpeed = SPRINT_SPEED;

            // TODO: replace transform change with actual animations
            playerUpper.transform.localPosition = Vector3.up;
        }
    }

    /// <summary>
    /// Resets to player movement state and speed to default values.
    /// </summary>
    private void ResetMovement()
    {
        state = State.Default;
        movementSpeed = INIT_SPEED;

        // TODO: replace transform change with actual animations
        playerUpper.transform.localPosition = Vector3.up;
    }

    /// <summary>
    /// If sprinting, slides, else, sets the current movement speed to "Crouched".
    /// </summary>
    private void Crouch()
    {
        if (state == State.Sprinting)
        {
            Slide();
        }
        else
        {
            state = State.Crouched;
            movementSpeed = CROUCH_SPEED;

            // TODO: replace transform change with actual animations
            playerUpper.transform.localPosition = new Vector3(0, 0.3f, 0);
        }
    }

    /// <summary>
    /// Adds fading impulse force while state == "Sliding" and player is moving forward. 
    /// </summary>
    private void Slide()
    {
        if (state != State.Sliding)
        {
            float slideVelocity;

            if (rb.velocity.y < 0)
            {
                float verticalVelocity = Mathf.Abs(rb.velocity.y);
                slideVelocity = Input.GetButton("Vertical") ? Mathf.Clamp(verticalVelocity / 4 * movementSpeed, MIN_SLIDE_VELOCITY, MAX_SLIDE_VELOCITY) : verticalVelocity * 1.5f;
            }
            else
            {
                slideVelocity = 25;
            }

            // reset horizontal look clamp to prevent camera from snapping if value is too large
            xLookClamp = 0;

            rb.AddForce(transform.forward * slideVelocity, ForceMode.VelocityChange);
            state = State.Sliding;
            StartCoroutine(ToggleCheckSlide());

            // TODO: replace transform change with actual animations
            playerUpper.transform.localPosition = new Vector3(0, 0.3f, 0);
        }
        else if (!Input.GetButton("Crouch"))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if (Input.GetButton("Sprint"))
            {
                state = State.Sprinting;
                Sprint();
            }
            else
            {
                ResetMovement();
            }
        }
        else if (rb.velocity.x < 0.5f && rb.velocity.x > -0.5f && canCheckSlide)
        {
            ResetMovement();
        }
    }

    /// <summary>
    /// Turns off <see cref="canCheckSlide"/> for a number of seconds in order to prevent movement reset when accelerating (this should only happen on deceleration).
    /// </summary>
    /// <returns></returns>
    private IEnumerator ToggleCheckSlide()
    {
        canCheckSlide = false;
        yield return new WaitForSeconds(.25f);
        canCheckSlide = true;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Shoot()
    {
        StartCoroutine(LimitFireRate());
        weapon.SpawnProjectile();

        // Animator
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LimitFireRate() 
    {
        canAttack = false;
        yield return new WaitForSeconds(weapon.rateOfFire);
        canAttack = true;
        yield break;
    }


    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    public void PickUpWeapon(GameObject newWeapon)
    {
        weapon.Drop();

        weapon = newWeapon.GetComponent<Weapon>();
        weaponObject = newWeapon;

        // Animator
    }


    ////// MUTATORS //////


}
