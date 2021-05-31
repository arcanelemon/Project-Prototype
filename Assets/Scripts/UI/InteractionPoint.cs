using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPoint : MonoBehaviour
{
    ////// STANDARD VARIABLES //////

    [Header ("Settings")]
    [Space (10)]

    //
    [SerializeField]
    private float interactTime;

    //
    [SerializeField]
    private float markerFadeTime;

    //
    [SerializeField]
    private float triggerRadius;

    //
    [SerializeField]
    private float distanceToTrigger;

    [Space (10)]
    [Header("Components")]
    [Space(10)]

    //
    [SerializeField]
    private Image progressFillImage;

    //
    [SerializeField]
    private Image markerImage;

    //
    private float interactTimer;

    //
    private bool triggered;

    //
    private Animator animator;

    //
    private GameObject assignee;


    ////// OVERRIDES //////

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (assignee == null)
        {
            gameObject.SetActive(false);
        }

        if (triggered)
        {
            if (Input.GetButton("Interact"))
            {
                interactTimer += Time.deltaTime;
                progressFillImage.fillAmount = interactTimer / interactTime;
            } else if (interactTimer > 0)
            {
                interactTimer -= Time.deltaTime * 3;
                progressFillImage.fillAmount = interactTimer / interactTime;
            }

            if (interactTimer >= interactTime)
            {
                assignee.GetComponent<Interactable>().Interact();
                gameObject.SetActive(false);
            }

            if (interactTimer < 0)
            {
                interactTimer = 0;
            }
        }

        Spin();
        transform.position = assignee.transform.position + Vector3.up;
    } 


    ////// PRIVATE //////

    /// <summary>
    /// 
    /// </summary>
    private void Spin()
    {
        Vector3 targetPoint = Camera.main.transform.position;

        // project camera position onto xz plane
        targetPoint.y = transform.position.y;

        // Vector3.up is a normal of the xz plane
        transform.LookAt(targetPoint, Vector3.up);
    }


    ////// PUBLIC //////

    /// <summary>
    /// 
    /// </summary>
    public void Enable()
    {
        triggered = true;
        animator.Play("Open", 0);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Disable()
    {
        triggered = false;
        animator.Play("Close", 0);
        progressFillImage.fillAmount = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assignee"></param>
    public void Assign(GameObject assignee)
    {
        this.assignee = assignee;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool GetActive()
    {
        return triggered;
    }
}
