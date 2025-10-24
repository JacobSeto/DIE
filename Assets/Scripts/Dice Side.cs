using TMPro;
using UnityEngine;

/// <summary>
/// Sets the dice text to the number side this represents
/// </summary>
public class DiceSide : MonoBehaviour
{
    [Tooltip("Value of dice side")]
    [SerializeField] float value;
    [Tooltip("Dice side properties")]
    [SerializeField] SideProperties sideProperty;
    [SerializeField] Dice dice;

    [SerializeField] LayerMask groundLayer;
    [Tooltip("The velocity threshold to be considered no longer moving")]
    [SerializeField] float reportVelocity;
    [SerializeField] float checkDistance;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    /// <summary>
    /// Report the score only when the dice is grounded and no longer considered moving
    /// </summary>
    /// <param name="other"></param>
    private void Update()
    {
        if (dice.isRolling && (rb.linearVelocity.magnitude + rb.angularVelocity.magnitude) < reportVelocity)
        {
            if (Physics.Raycast(transform.position, transform.forward, checkDistance, groundLayer))
            {
                DiceManager.Instance.ReportScore(dice, value, sideProperty);
                dice.isRolling = false;
            }
        }
    }


}
