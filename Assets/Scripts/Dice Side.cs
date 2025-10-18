using TMPro;
using UnityEngine;

/// <summary>
/// Sets the dice text to the number side this represents
/// </summary>
public class DiceSide : MonoBehaviour
{
    // SerializeField allows us to set a value through the Unity Editor
    [SerializeField] int diceSide;
    [SerializeField] TMP_Text diceText;

    private void OnTriggerEnter(Collider other)
    {
        // If the gameobject has the Floor tag
        if (other.CompareTag("Floor"))
        {
            // Set the diceText to the diceSide
            diceText.text = diceSide.ToString();
        }
    }
}
