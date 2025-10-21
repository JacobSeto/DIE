using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour
{

    [Tooltip("The minimum amount of force applied")]
    [SerializeField] float minTorqueStrength;
    [Tooltip("The maximum amount of force applied")]
    [SerializeField] float maxTorqueStrength;
    [Tooltip("The minimum amount of force applied")]
    [SerializeField] float minForceStrength;
    [Tooltip("The maximum amount of force applied")]
    [SerializeField] float maxForceStrength;

    [Tooltip("Time dice waits before trying to correct itself if stuck")]
    [SerializeField] float tryAgainWaitTime;

    float tryWaitTime;

    Rigidbody rb;
    [HideInInspector] public bool isRolling;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isRolling)
        {
            tryWaitTime -= Time.deltaTime;
            if (tryWaitTime <= 0)
            {
                StartCoroutine(Roll());
            }
        }
    }

    /// <summary>
    /// Applies a random vertical force
    /// </summary>
    public void ApplyTorque()
    {
        // Multiply a vector with a random torque variable
        // (Random.Range(0,2) == 0 ? -1 : 1) is 50% chance of being negative
        float x = (Random.Range(0, 2) == 0 ? -1 : 1) * Random.Range(minTorqueStrength, maxTorqueStrength);
        float y = (Random.Range(0, 2) == 0 ? -1 : 1) * Random.Range(minTorqueStrength, maxTorqueStrength);
        float z = (Random.Range(0, 2) == 0 ? -1 : 1) * Random.Range(minTorqueStrength, maxTorqueStrength);
        rb.AddTorque(new Vector3(x, y, z), ForceMode.Impulse);
    }

    /// <summary>
    /// Applies a random vertical force
    /// </summary>
    public void ApplyForce()
    {
        // Multiply a vector pointing upwards by a random force strength
        rb.AddForce(Vector3.up * Random.Range(minForceStrength, maxForceStrength), ForceMode.Impulse);
    }


    public IEnumerator Roll()
    {
        tryWaitTime = tryAgainWaitTime;
        ApplyForce();
        yield return new WaitForSeconds(.075f);
        ApplyTorque();
        isRolling = true;
    }
}
