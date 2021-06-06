using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    
    ////// STANDARD VARIABLES //////

    [Header("Controls")]
    [Space(10)]

    //
    [SerializeField]
    private bool toggleCrouch = false;

    //
    [SerializeField]
    private bool toggleADS = false;

    [Space(10)]
    [Header("Core Variables")]
    [Space(10)]

    //
    [SerializeField]
    private bool godMode;

    // The default movement speed of the player
    [SerializeField]
    private float health = 100;

    [SerializeField]
    [Range(1, 5)]
    private int healthChunks = 3;

    //
    [SerializeField]
    private float healthRestoreTime = 3f;

    //
    [SerializeField]
    private float healthRestoreIncrement = 0.1f;

    //
    [SerializeField]
    private float sheild = 30;

    //
    [SerializeField]
    private float sheildRechargeTime = 3f;

    //
    [SerializeField]
    private float sheildIncrement = 0.1f;

    //
    [SerializeField]
    private int lightBulletBankStarting = 150;

    //
    [SerializeField]
    private int lightBulletBankMax = 500;

    //
    [SerializeField]
    private int mediumBulletBankStarting = 100;

    //
    [SerializeField]
    private int mediumBulletBankMax = 300;

    //
    [SerializeField]
    private int heavyBulletBankStarting = 50;

    //
    [SerializeField]
    private int heavyBulletBankMax = 150;

    //
    [SerializeField]
    private int specialBulletBankStarting = 8;

    //
    [SerializeField]
    private int specialBulletBankMax = 30;

    [Space(10)]
    [Header("Movement Variables")]
    [Space(10)]

    // The default movement speed of the player
    [SerializeField]
    private float initSpeed = 10;

    // The sprint movement speed of the player
    [SerializeField]
    private float sprintSpeed = 20;

    // The crouch movement speed of the player
    [SerializeField]
    private float crouchSpeed = 2;

    // Force multiplier used when the player jumps
    [SerializeField]
    private float jumpForce = 5;

    // Minimum time between player jumps
    [SerializeField]
    private float jumpTime = 0.1f;

    // Maximum number of times the player can jump
    [SerializeField]
    private int numJumps = 1;

    // Force multiplier used when the player jumps
    [SerializeField]
    private float maxSlideVelocity = 30;

    // Force multiplier used when the player jumps
    [SerializeField]
    private float minSlideVelocity = 20;

    //
    [SerializeField]
    [Range (0.01f, 1)]
    private float climbSpeed = 0.1f;

    //
    [SerializeField]
    private float climbDistance = 0.5f;
    
    [Space(10)]
    [Header("Look Variables")]
    [Space(10)]

    // Maximum vertical look constraint
    [SerializeField]
    private float lookConstraintYMax = 90;

    // Minimum vertical look constraint
    [SerializeField]
    private float lookConstraintYMin = -90;

    // Maximum vertical look constraint
    [SerializeField]
    private float lookConstraintXMax = 60;

    // Minimum vertical look constraint
    [SerializeField]
    private float lookConstraintXMin = -60;

    // The player's horizontal look sensitivity
    [SerializeField]
    private float lookSensitivityX = 1;

    // The player's vertical look sensitivity
    [SerializeField]
    private float lookSensitivityY = 1;

    [Space(10)]
    [Header("Misc.")]
    [Space(10)]

    //
    [Range (1, 10)]
    [SerializeField]
    private int interactDistance = 3;

    [Space(10)]
    [Header("Gravity Adjustment")]
    [Space(10)]

    // Gravity Mulitplier used to influence rigidbody downward velocity
    [Range(0, 5)]
    [SerializeField]
    private float gravityDownwardMultiplier = 2;


    ////// STANDARD VARIABLES //////

    //
    private float currHealth;

    //
    private float currSheild;

    // The current movement speed of the player
    private float movementSpeed;

    // Float used to clamp vertical looking constraints
    private float yLookClamp = 0;

    // Float used to clamp vertical looking constraints
    private float xLookClamp = 0;

    // The rotational offset of the upper body relative to the player
    private float upperHorizontalRotationalOffset = 0;

    //
    private float verticalRecoil = 0;

    //
    private float horizontalRecoil = 0;

    //
    private int essenceBank;

    //
    private int bulletBankLight;

    //
    private int bulletBankMedium;

    //
    private int bulletBankHeavy;

    //
    private int bulletBankSpecial;

    //
    private int weaponSwapIndex = 0;

    // The current number of jumps that the player has used
    private int currentJumps = 0;

    //
    private bool disabled;

    // Boolean used to check if the player is on touching the ground
    private bool grounded;

    //
    private bool canRestoreHealth;

    //
    private bool canRechargeSheild;

    // Boolean used to check if the player is on touching the ground
    private bool canJump;

    // Boolean used to check if the player has accelerated (a value of zero triggers crouching behavior)
    private bool canCheckSlide;

    // Boolean used to check if the player is on touching the ground
    private bool canAttack = true;

    //
    private bool uiElementBlocking;

    //
    private bool emoteWheelActive;

    //
    private Vector2 movementInputs;

    //
    private IEnumerator decrementRecoilCoroutine;

    //
    private IEnumerator reloadCoroutine;

    // The current movement state of the player
    private MovementState state = MovementState.Default;

    //
    private IEnumerator restoreHealthCoroutine;

    //
    private IEnumerator sheildRechargeCoroutine;


    ////// MONO VARIABLES //////

    //
    private InteractionPoint currentFocus;

    // Upper body player transform
    private Transform playerUpper;

    //
    private FPSCameraController fpsCameraController;

    //
    private HUDController hudController;

    //
    private Weapon weapon;

    //
    private Weapon[] weapons;

    // Rigidbody of the player
    private Rigidbody rb;

    // Enum used to define player movement state
    public enum MovementState
    {
        Default,
        Sprinting,
        Crouched,
        Jumping,
        Sliding,
        Climbing,
    }


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {

        // Set Variables
        currHealth = health;
        currSheild = sheild;
        movementSpeed = initSpeed;
        bulletBankLight = lightBulletBankStarting;
        bulletBankMedium = mediumBulletBankStarting;
        bulletBankHeavy = heavyBulletBankStarting;
        bulletBankSpecial = specialBulletBankStarting;

        //playerUpper = gameObject.GetComponentInChildren<Transform>();
        rb = GetComponent<Rigidbody>();
        playerUpper = GetComponentsInChildren<Transform>()[1];
        hudController = gameObject.GetComponentInChildren<HUDController>();
        fpsCameraController = GetComponentInChildren<FPSCameraController>();
        weapons = GetComponentsInChildren<Weapon>();
        if (weapons.Length != 0)
        {
            foreach (Weapon w in weapons)
            {
                w.Initialize();
            }

            SwapWeapons(0);
        }

        // Pull from Config Data

        // Initialize State
        MouseUtils.LockMouse();
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (!disabled)
        {
            AdjustGravity();
            HandleInput();
            CheckViewFocus();

            if (canRestoreHealth)
            {
                RestoreHealth();
            }

            if (canRechargeSheild)
            {
                RechargeSheild();
            }
        }
    }

    //
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag is "Wall")
        {
            CheckClimb();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        grounded = true;
        canJump = true;
        currentJumps = 0;


        // Check input states
        if (Input.GetButton("Crouch") && Input.GetAxisRaw("Vertical") >= 0)
        {
            Slide();
        } else if (Input.GetButton("Sprint"))
        {
            Sprint();
        } else
        {
            // reset movement to reduce risk of observed fringe cases
            ResetMovement();
            fpsCameraController.Impact();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (!(Physics.OverlapSphere(transform.localPosition - Vector3.up, 0.5f).Length > 1)) 
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
            rb.velocity -= Vector3.up * Physics2D.gravity.y * -gravityDownwardMultiplier * Time.deltaTime;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleInput()
    {
        // TODO: check custom control toggles that are loaded Ex: if crouchToggle CheckCrouchToggle() else CheckCrouch().

        if (!uiElementBlocking)
        {
            if (grounded)
            {

                // Check for sprint
                if (Input.GetButtonDown("Sprint") && state != MovementState.Sprinting)
                {
                    Sprint();
                }
                else if (state == MovementState.Sprinting && (Input.GetButtonUp("Sprint") || Input.GetAxisRaw("Vertical") < 1))
                {
                    ResetMovement();
                }

                // Check for crouch
                if (Input.GetButtonDown("Crouch") && state != MovementState.Crouched)
                {
                    Crouch();
                }
                else if (state == MovementState.Crouched && Input.GetButtonUp("Crouch"))
                {
                    ResetMovement();
                }
            }

            // Check for jump button
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            // Emote Wheel and should block specific controls.
            if (!emoteWheelActive)
            {
                Look();

                if (weapon != null)
                {

                    // Check for Attack Button
                    if (canAttack)
                    {
                        if (Input.GetButton("Fire") && (weapon.varient == Weapon.Varient.Auto || weapon.varient == Weapon.Varient.Burst))
                        {
                            Attack();
                        }
                        else if (Input.GetButtonDown("Fire") && weapon.varient == Weapon.Varient.Semi)
                        {
                            Attack();
                        }
                    }

                    // Check for ADS Button
                    if (Input.GetButtonDown("Aim") && weapon.state != Weapon.State.Aiming && weapon.state != Weapon.State.Reloading)
                    {
                        if (state == MovementState.Sprinting) 
                        {
                            ResetMovement();
                        }
                        ADS();
                    }
                    else if (Input.GetButtonUp("Aim") && weapon.state == Weapon.State.Aiming)
                    {
                        ResetADS();
                    }

                    // Check for ADS Button
                    if (Input.GetButtonDown("Reload") && weapon.GetAmmoPercentage() != 1)
                    {
                        StartReloadCoroutine();
                    }

                    if (weapon != null && weapon.state != Weapon.State.Reloading)
                    {
                        // Check for weapon swap
                        if (Input.GetButtonDown("Equip 1") && weapons[0] != null)
                        {
                            SwapWeapons(0);
                        }
                        else if (Input.GetButtonDown("Equip 2") && weapons.Length > 1)
                        {
                            SwapWeapons(1);
                        }
                        else if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
                        {
                            int weaponIndex = Input.GetAxisRaw("Mouse ScrollWheel") < 0 ? 1 : (int)Input.GetAxisRaw("Mouse ScrollWheel");

                            if (weapons[weaponIndex] != null)
                            {
                                SwapWeapons(weaponIndex);
                            }
                        }
                    }

                // Check for Special Button
                if (Input.GetButtonDown("Special"))
                {

                }

               
                }
            }

            if (Input.GetButtonDown("Emote"))
            {
                if (!emoteWheelActive)
                {
                    emoteWheelActive = true;
                    hudController.DisplayEmoteWheel();
                }
            }
        }

        if (Input.GetButtonUp("Emote") || !Input.GetButton("Emote"))
        {
            if (emoteWheelActive)
            {
                Emote();
            }
        }

        if (Input.GetButtonDown("Inventory"))
        {
            MouseUtils.ReleaseMouse();
            uiElementBlocking = true;
        } else if (Input.GetButtonUp("Inventory") && uiElementBlocking)
        {
            MouseUtils.LockMouse();
            uiElementBlocking = false;
        }

        movementInputs.x = (int)Input.GetAxisRaw("Horizontal");
        movementInputs.y = (int)Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {

        if (!disabled)
        {
            // Check for movement
            HandleMove();
        }
    }

    /// <summary>
    /// Rotates player transforms with look inputs.
    /// </summary>
    private void Look()
    {
        float inputX = Input.GetAxis("Mouse X") * lookSensitivityY;
        float inputY = Input.GetAxis("Mouse Y") * lookSensitivityX;

        Vector3 rotPlayerUpper = playerUpper.localRotation.eulerAngles;

        yLookClamp -= inputY;
        yLookClamp = Mathf.Clamp(yLookClamp, lookConstraintYMin, lookConstraintYMax);
        if (yLookClamp >= lookConstraintYMax)
        {
            rotPlayerUpper.x = lookConstraintYMax;
        }
        else if (yLookClamp <= lookConstraintYMin)
        {
            rotPlayerUpper.x = lookConstraintYMin;
        }
        else
        {
            rotPlayerUpper.x -= inputY;
        }


        // clamp horizontal if sliding
        xLookClamp += inputX;
        xLookClamp = Mathf.Clamp(xLookClamp, lookConstraintXMin, lookConstraintXMax);
        Vector3 rotPlayer = transform.rotation.eulerAngles;
        if (state == MovementState.Sliding)
        {
            if (xLookClamp >= lookConstraintXMax)
            {
                rotPlayerUpper.y = lookConstraintXMax;
            }
            else if (xLookClamp <= lookConstraintXMin)
            {
                rotPlayerUpper.y = lookConstraintXMin;
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
    private void HandleMove()
    {
        float horizontalDirection = movementInputs.x;
        float verticalDirection = movementInputs.y;

        if (state == MovementState.Sliding)
        {
            // Ajdust for slide
            Slide();
        }

        Move(verticalDirection, horizontalDirection);
        
    }

    /// <summary>
    /// Moves player by the given horizontal and vertical movement multipliers.
    /// </summary>
    /// <param name="verticalMovementMultiplier"></param>
    /// <param name="horizontalMovementMultiplier"></param>
    private void Move(float verticalMovementMultiplier, float horizontalMovementMultiplier)
    {

        // NOTE: Due to issues with collision, this code was converted to mimic the functionality of Rigidbody.MovePosition(...)
        if (state != MovementState.Sliding && state != MovementState.Climbing) 
        {
            Vector3 desiredVelocity = (transform.forward * movementSpeed * verticalMovementMultiplier) + (transform.right * movementSpeed * horizontalMovementMultiplier) - rb.velocity;
            rb.AddForce(new Vector3(desiredVelocity.x, 0, desiredVelocity.z), ForceMode.VelocityChange);
        }

        if (verticalMovementMultiplier != 0 || horizontalMovementMultiplier != 0)
        { 
            // Bob camera.
            if (state == MovementState.Default) 
            {
                fpsCameraController.Bob(true);
            } else if (state == MovementState.Sprinting) 
            {
                fpsCameraController.Bob(false);
            }
        } else if (fpsCameraController.GetBobActive())
        {
            fpsCameraController.StopBob();
        }
    }

    /// <summary>
    /// Adds an upward impulse force to the player.
    /// </summary>
    private void Jump()
    {
        if ((grounded || currentJumps < numJumps) && canJump)
        {
            if (state == MovementState.Sprinting) 
            {
                hudController.HideSprintIcon();
            } else if (state == MovementState.Sliding)
            {
                hudController.HideSlideIcon();
            }

            state = MovementState.Jumping;
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            currentJumps++;

            StartCoroutine(LimitJumpTime());

            // reset first person attributes
            if (fpsCameraController.GetBobActive()) 
            {
                fpsCameraController.StopBob();
            }

            if (fpsCameraController.GetVignetteActive())
            {
                fpsCameraController.ResetVignette();
            }
            // TODO: replace transform change with actual animations
            playerUpper.transform.localPosition = Vector3.up;

            // TODO: Reset third person attributes
        }
    }

    /// <summary>
    /// Toggles <see cref="canJump"/> for <see cref="jumpTime"/> in order to prevent multiple jump calls before ground exit trigger is registered.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LimitJumpTime()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpTime);
        canJump = true;
        yield break;
    }

    /// <summary>
    /// Changes player movement speed to "SPRINT_SPEED" and player state to "Sprinting"
    /// </summary>
    private void Sprint()
    {
        if (Input.GetAxisRaw("Vertical") > 0 && state != MovementState.Sliding)
        {
            state = MovementState.Sprinting;
            movementSpeed = sprintSpeed;

            hudController.DisplaySprintIcon();

            if (!fpsCameraController.GetVignetteActive())
            {
                fpsCameraController.DarkenVingette();
            }

            if (weapon != null && weapon.state == Weapon.State.Aiming) 
            {
                ResetADS();
            }

            // TODO: replace transform change with actual animations
            playerUpper.transform.localPosition = Vector3.up;
        }
    }

    /// <summary>
    /// Resets to player movement state and speed to default values.
    /// </summary>
    private void ResetMovement()
    {
        if (state == MovementState.Sprinting) 
        {
            hudController.HideSprintIcon();
        } else if (state == MovementState.Sliding) 
        {
            hudController.HideSlideIcon();
        }

        state = MovementState.Default;
        movementSpeed = initSpeed;
        rb.velocity = Vector3.zero;


        if (fpsCameraController.GetVignetteActive())
        {
            fpsCameraController.ResetVignette();
        }

        // TODO: replace transform change with actual animations
        playerUpper.transform.localPosition = Vector3.up;
    }

    /// <summary>
    /// If sprinting, slides, else, sets the current movement speed to "Crouched".
    /// </summary>
    private void Crouch()
    {
        if (state == MovementState.Sprinting)
        {
            hudController.HideSprintIcon();
            Slide();
        }
        else
        {
            state = MovementState.Crouched;
            movementSpeed = crouchSpeed;

            if (!fpsCameraController.GetVignetteActive())
            {
                fpsCameraController.DarkenVingette();
            }

            // TODO: replace transform change with actual animations
            playerUpper.transform.localPosition = new Vector3(0, 0.3f, 0);
        }

    }

    /// <summary>
    /// Adds fading impulse force while state == "Sliding" and player is moving forward. 
    /// </summary>
    private void Slide()
    {
        if (state != MovementState.Sliding)
        {
            float slideVelocity;

            if (rb.velocity.y < 0)
            {
                float verticalVelocity = Mathf.Abs(rb.velocity.y);
                slideVelocity = Input.GetButton("Vertical") ? Mathf.Clamp(verticalVelocity / 4 * movementSpeed / 2, minSlideVelocity, maxSlideVelocity) : verticalVelocity * 1.5f;
            } else
            {
                slideVelocity = 10;
            }

            // reset horizontal look clamp to prevent camera from snapping if value is too large
            xLookClamp = 0;

            Vector3 direction;

            // Check if player has any horizontal momentum
            if (Input.GetButton("Horizontal"))
            {
                direction = Input.GetAxisRaw("Horizontal") > 0 ? transform.forward + transform.right : transform.forward + -transform.right;
            } else
            {
                direction = transform.forward;
            }

            rb.AddForce(direction * slideVelocity, ForceMode.VelocityChange);



            state = MovementState.Sliding;
            hudController.DisplaySlideIcon();
            fpsCameraController.StopBob();
            StartCoroutine(ToggleCheckSlide());

            // TODO: replace transform change with actual animations
            playerUpper.transform.localPosition = new Vector3(0, 0.3f, 0);
        } else if (!Input.GetButton("Crouch"))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            if (Input.GetButton("Sprint"))
            {
                state = MovementState.Sprinting;
                Sprint();
            } else
            {
                ResetMovement();
            }

            hudController.HideSlideIcon();
        } else if (rb.velocity.x < 0.5f && rb.velocity.x > -0.5f && canCheckSlide)
        {
            ResetMovement();
        } else if (!fpsCameraController.GetVignetteActive())
        {
            fpsCameraController.DarkenVingette();
        }
    }

    /// <summary>
    /// Turns off <see cref="canCheckSlide"/> for a number of seconds in order to prevent movement reset when accelerating value is less than threshold (which should only happen on deceleration).
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
    /// CREDIT: climbing code is inspired Steven, <see href="https://www.youtube.com/watch?v=ANlC_4K3hak"></see>
    /// </summary>
    private void CheckClimb() 
    {
        // Check to make sure the ledge ends and player is not already climbing
        if (!Physics.Raycast(transform.position + (Vector3.up * 2), transform.forward, out _, climbDistance) && state != MovementState.Climbing)
        {
            StartCoroutine(GrabLedge());
        }
    }

    //
    private IEnumerator GrabLedge() 
    {
        ResetMovement();
        state = MovementState.Climbing;
        rb.useGravity = false;

        Debug.DrawRay(transform.position - Vector3.up, transform.forward, Color.green, 10);
        while (Physics.Raycast(transform.position - Vector3.up, transform.forward, out _, climbDistance) && state == MovementState.Climbing)
        {
            rb.position += Vector3.up * climbSpeed;
            yield return null;
        }

        rb.useGravity = true;
        ResetMovement();
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Attack()
    {
        if (weapon.type == Weapon.Type.Gun)
        {
            if (weapon.IsEmpty())
            {
                StartReloadCoroutine();
            } else 
            {
                // fire weapon
                StartCoroutine(LimitFireRate());
                weapon.Shoot();

                // handle recoil
                verticalRecoil += weapon.verticalRecoil;
                horizontalRecoil += weapon.horizontalRecoil;
                Debug.Log(verticalRecoil);
                
                if (verticalRecoil < weapon.maxVerticalRecoil) 
                {
                    playerUpper.localRotation = Quaternion.Euler(playerUpper.localRotation.eulerAngles - (Vector3.right * weapon.verticalRecoil));
                }

                if (horizontalRecoil < weapon.maxHorizontalRecoil)
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + (Vector3.up * weapon.horizontalRecoil * (UnityEngine.Random.Range(0, 2) * 2 - 1)));
                }

                // update hud controller
                hudController.SetReticleScaleFromRecoil(horizontalRecoil, verticalRecoil);
                hudController.UpdateCurrentAmmo(weapon.GetRemainingAmmo(), weapon.GetAmmoPercentage());

                StartDecrementRecoilCoroutine();
            }
        }
    }

    
    private IEnumerator DecrementRecoil() 
    {
        yield return new WaitForSeconds(.1f);
        while (verticalRecoil > 0 && horizontalRecoil > 0) 
        {
            if (verticalRecoil > 0) 
            {
                verticalRecoil -= weapon.verticalRecoil;
            }

            if (horizontalRecoil > 0)
            {
                horizontalRecoil -= weapon.horizontalRecoil;
            }

            hudController.SetReticleScaleFromRecoil(horizontalRecoil, verticalRecoil);
            yield return new WaitForSeconds(.01f);
        }
        verticalRecoil = 0;
        horizontalRecoil = 0;
        yield break;
    }


    private void StartDecrementRecoilCoroutine() 
    {
        StopDecrementRecoilCoroutine();
        decrementRecoilCoroutine = DecrementRecoil();
        StartCoroutine(decrementRecoilCoroutine);
    }


    private void StopDecrementRecoilCoroutine() 
    {
        if(decrementRecoilCoroutine != null) 
        {
            StopCoroutine(decrementRecoilCoroutine);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Reload()
    {
        canAttack = false;
        if (weapon.state == Weapon.State.Aiming)
        {
            ResetADS();
        }

        int ammoConsumed = weapon.GetUsedAmmo();

        // Animator

        // TODO: wait for anim time
        int ammoAvailable = AmmoTypeToAmmount(weapon.ammoType);
        weapon.Reload(ammoAvailable < weapon.GetMagazineSize() ? ammoAvailable : weapon.GetMagazineSize());
        hudController.EnableReloadIcon(2);
        yield return new WaitForSeconds(2);
        IncrementBulletBank(weapon.ammoType, -ammoConsumed);
        hudController.EnableDefaultIcon();
        hudController.UpdateCurrentAmmo(weapon.GetRemainingAmmo(), weapon.GetAmmoPercentage());
        hudController.UpdateTotalAmmo(AmmoTypeToAmmount(weapon.ammoType));
        canAttack = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartReloadCoroutine()
    {
        if (weapon.state != Weapon.State.Reloading && AmmoTypeToAmmount(weapon.ammoType) > 0)
        {
            StopReloadCoroutine();
            reloadCoroutine = Reload();
            StartCoroutine(reloadCoroutine);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void StopReloadCoroutine()
    {
        if (reloadCoroutine != null) 
        {
            StopCoroutine(reloadCoroutine);
            hudController.EnableDefaultIcon();
            hudController.UpdateCurrentAmmo(weapon.GetRemainingAmmo(), weapon.GetAmmoPercentage());
            hudController.UpdateTotalAmmo(AmmoTypeToAmmount(weapon.ammoType));
            weapon.state = Weapon.State.Idle;
            canAttack = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void ADS()
    {
        weapon.ToggleADS();
        fpsCameraController.Zoom(weapon.zoom);
    }

    /// <summary>
    /// 
    /// </summary>
    private void ResetADS()
    {
        weapon.ToggleADS();
        fpsCameraController.ResetZoom();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LimitFireRate()
    {
        canAttack = false;
        yield return new WaitForSeconds(weapon.FireRateToSeconds());
        canAttack = true;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RechargeSheild()
    {
        if (currSheild == sheild || currSheild <= 0)
        {
            canRechargeSheild = false;
        } else
        {
            currSheild += sheildIncrement;
            hudController.UpdateFromPlayerSheild(SheildPercentage());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartSheildRechargeCoroutine()
    {
        StopSheildRechargeCoroutine();
        sheildRechargeCoroutine = SheildRechargeCoroutine();
        StartCoroutine(sheildRechargeCoroutine);
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopSheildRechargeCoroutine()
    {
        if (sheildRechargeCoroutine != null)
        {
            StopCoroutine(sheildRechargeCoroutine);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator SheildRechargeCoroutine()
    {
        canRechargeSheild = false;
        yield return new WaitForSeconds(healthRestoreTime);
        canRechargeSheild = true;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    private void RestoreHealth()
    {
        if (currHealth >= health * 0.2f)
        {
            canRestoreHealth = false;
            currHealth = health * 0.2f;
            fpsCameraController.StopDangerMode();
        } else
        {
            currHealth += healthRestoreIncrement;
            hudController.UpdateFromPlayerHealth(HealthPercentage());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartRestoreHealthCoroutine()
    {
        StopRestoreHealthCoroutine();
        restoreHealthCoroutine = RestoreHealthCoroutine();
        StartCoroutine(restoreHealthCoroutine);
    }

    /// <summary>
    /// 
    /// </summary>
    private void StopRestoreHealthCoroutine()
    {
        if (restoreHealthCoroutine != null)
        {
            StopCoroutine(restoreHealthCoroutine);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestoreHealthCoroutine()
    {
        hudController.DisplayDangerMode();
        fpsCameraController.DangerMode();
        canRestoreHealth = false;
        yield return new WaitForSeconds(healthRestoreTime);
        canRestoreHealth = true;
        yield break;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float SheildPercentage()
    {
        return currSheild / sheild;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float HealthPercentage()
    {
        return currHealth / health;
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckViewFocus()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactDistance))
        {
            InteractionPoint newFocus = hit.collider.gameObject.GetComponent<InteractionPoint>();

            if (newFocus != null)
            {

                if (currentFocus != null && !currentFocus.Equals(newFocus))
                {
                    currentFocus.Disable();
                }
                currentFocus = newFocus;
                currentFocus.Enable();
            }
        }
        else
        {
            if (currentFocus != null && currentFocus.GetActive())
            {
                currentFocus.Disable();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="swapIndex"></param>
    private void SwapWeapons(int swapIndex)
    {
        if (weapon != null)
        {
            // TODO: Play swap animation down

            if (weapon.state == Weapon.State.Aiming)
            {
                ResetADS();
            }

            weapon.gameObject.SetActive(false);
        }

        weapon = weapons[swapIndex];
        weapon.gameObject.SetActive(true);
        weaponSwapIndex = swapIndex;
        weapon.AssignWeaponUIToHUD();
        hudController.UpdateWeaponEquiped(swapIndex);
        hudController.UpdateCurrentAmmo(weapon.GetRemainingAmmo(), weapon.GetAmmoPercentage());
        hudController.UpdateTotalAmmo(AmmoTypeToAmmount(weapon.ammoType));

        // TODO: wait for swap animation up to finish

        weapon.PlayAnimIfFirstTime();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    private void IncrementBulletBank(Ammo.Type type, int amount)
    {
        switch(type)
        {
            case Ammo.Type.Light:
                bulletBankLight = bulletBankLight + amount < 0 ? 0 : bulletBankLight + amount;
            break;
            case Ammo.Type.Medium:
                bulletBankMedium = bulletBankMedium + amount < 0 ? 0 : bulletBankMedium + amount;
                break;
            case Ammo.Type.Heavy:
                bulletBankHeavy = bulletBankHeavy + amount < 0 ? 0 : bulletBankHeavy + amount;
                break;
            case Ammo.Type.Special:
                bulletBankSpecial = bulletBankSpecial + amount < 0 ? 0 : bulletBankSpecial + amount;
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private int AmmoTypeToAmmount(Ammo.Type type)
    {
        switch (type)
        {
            case Ammo.Type.Light:
                return bulletBankLight;
            case Ammo.Type.Heavy:
                return bulletBankHeavy;
            case Ammo.Type.Special:
                return bulletBankSpecial;
            default:
                return bulletBankMedium;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Die()
    {
        disabled = true;

    }


    ////// PUBLIC //////

    /// <summary>
    /// d
    /// </summary>
    public void DamagePlayer(float damage)
    {
        // TODO: Adjust damage based on damage type and player damage modifier
        if (currSheild > 0)
        {
            if (currSheild - damage < 0)
            {
                currSheild = 0;
                currHealth -= damage - currSheild;
                hudController.UpdateFromPlayerSheild(SheildPercentage());
                hudController.UpdateFromPlayerHealth(HealthPercentage());
            }
            else
            {
                currSheild -= damage;
                StartSheildRechargeCoroutine();
                hudController.UpdateFromPlayerSheild(SheildPercentage());
            }
        } else
        {
            currHealth -= damage;
            hudController.UpdateFromPlayerHealth(HealthPercentage());
            fpsCameraController.Damage();
        }

        if (HealthPercentage() <= 0.2f)
        {
            StartRestoreHealthCoroutine();
        }

        if (currHealth <= 0 && !godMode)
        {
            Die();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Revive()
    {
        // TODO: Cap health at 30%. play animation
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="weapon"></param>
    public void AddWeapon(Weapon weapon)
    {
        weapon.transform.parent = transform.GetChild(0).GetChild(0);
        weapon.transform.position = weapon.transform.parent.position;
        weapon.transform.rotation = weapon.transform.parent.rotation;
        weapon.Initialize();

        if (this.weapon == null) 
        {
            weapons = new Weapon[1];
            weapons[0] = weapon;
            SwapWeapons(0);
        } else 
        {
            if (this.weapon.state == Weapon.State.Reloading)
            {
                StopReloadCoroutine();
            }

            if (weapons.Length > 1)
            {
                this.weapon.Drop();
                weapons[weaponSwapIndex] = weapon;
                this.weapon = weapon;
                SwapWeapons(weaponSwapIndex);
            }
            else if (weapons.Length == 1)
            {
                weapons = new Weapon[2];
                weapons[0] = this.weapon;
                weapons[1] = weapon;
                SwapWeapons(1);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void Emote()
    {
        int emoteToPlay = hudController.GetEmoteSelected();
        if (emoteToPlay != -1)
        {
            Debug.Log("Play: Emote: " + emoteToPlay);
        }

        hudController.HideEmoteWheel();
        emoteWheelActive = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void AddEssence(int amount)
    {
        essenceBank += amount;
        Debug.Log("Total Essence: " + essenceBank);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public void AddAmmo(Ammo.Type type, int amount)
    {
        IncrementBulletBank(type, amount);

        if (weapon != null && weapon.ammoType == type)
        {
            hudController.UpdateTotalAmmo(AmmoTypeToAmmount(type));
        }

        Debug.Log("Ammo Added: " + type.ToString());
    }
}


//// check if weapon is less than ROF threshold for break and that the gun has reset (aka finished settle time/ settle animation
//if (weapon.GetROF() < ROF_BREAK_THRESHOLD && weapon.GetSpread() != uiSight.transform.localScale.x - 1)
//{
//    StartBreakSightCoroutine();
//}
//else
//{
//    IncrementSightScaleWithRecoil();
//}