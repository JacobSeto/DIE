using UnityEngine;

/// <summary>
/// Applies a random vertical force to this object
/// </summary>
public class RandomForce : MonoBehaviour
{
    [Tooltip("The minimum amount of force applied")]
    [SerializeField] float minForceStrength;
    [Tooltip("The maximum amount of force applied")]
    [SerializeField] float maxForceStrength;

    Rigidbody rb;
    private void Start()
    {
        // Get reference to the rigidbody
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Applies a random vertical force
    /// </summary>
    public void ApplyForce()
    {
        // Multiply a vector pointing upwards by a random force strength
        rb.AddForce(Vector3.up * Random.Range(minForceStrength, maxForceStrength), ForceMode.Impulse);
    }


}
