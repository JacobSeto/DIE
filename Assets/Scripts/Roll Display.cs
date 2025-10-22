using TMPro;
using UnityEngine;

/// <summary>
/// Displays a dice's resulting roll. Score text is scaled based on value, while
/// special rolls have unique color and scaling
/// </summary>
public class RollDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text rollText;
    [Tooltip("The scale of the text is 1 + score / scaleCoefficient")]
    [SerializeField] float scaleCoefficient;
    [SerializeField] float displayTime;
    [Range(0, 1)]
    [SerializeField] float xRange;
    [Range(0, 1)]
    [SerializeField] float zRange;

    [Tooltip("y movement constant")]
    [Range(0, 1)]
    [SerializeField] float yMove;

    Vector3 randomMove;

    [SerializeField] Color specialRollColor;
    [SerializeField] float specialRollScale;

    /// <summary>
    /// Sets a random move vector and destroys the display at a certain time
    /// </summary>
    void SetDisplay()
    {
        randomMove = new Vector3(Random.Range(0, xRange), yMove, Random.Range(0, xRange));
        Destroy(gameObject, displayTime);
    }

    /// <summary>
    /// Sets the display for a score
    /// </summary>
    /// <param name="score">The score of the dice</param>
    public void SetScoreDisplay(int score)
    {
        rollText.text = score.ToString();
        transform.localScale *= 1 + (score / scaleCoefficient);
        SetDisplay();
    }

    public void SetSpecialDisplay(string specialText)
    {
        rollText.text = specialText;
        rollText.color = specialRollColor;
        transform.localScale *= specialRollScale;
        SetDisplay();
    }

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
        transform.position += randomMove * Time.deltaTime;
    }
}
