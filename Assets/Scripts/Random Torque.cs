using UnityEngine;

/// <summary>
/// Applies a random torque to this object
/// </summary>
public class RandomTorque : MonoBehaviour
{
    [Tooltip("The minimum amount of force applied")]
    [SerializeField] float minTorqueStrength;
    [Tooltip("The maximum amount of force applied")]
    [SerializeField] float maxTorqueStrength;

    Rigidbody rb;
    private void Start()
    {
        // Get reference to the rigidbody
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Applies a random vertical force
    /// </summary>
    public void ApplyTorque()
    {
        // Multiply a vector with a random torque variable
        float x = Random.Range(minTorqueStrength, maxTorqueStrength);
        float y = Random.Range(minTorqueStrength, maxTorqueStrength);
        float z = Random.Range(minTorqueStrength, maxTorqueStrength);
        rb.AddTorque(new Vector3(x, y, z), ForceMode.Impulse);
    }
}
