using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Tooltip("The player score")]
    public int score;
    [SerializeField] TMP_Text scoreText;

    [Tooltip("Counts up the score added from that round")]
    int scoreCounter;
    TMP_Text scoreCounterText;

    [Tooltip("How much the scaling of the scoreCounter is applied for each added score")]
    float scoreCounterScale;

    [Tooltip("The player debt for the round")]
    public int debt;
    [SerializeField] TMP_Text debtText;
    [Tooltip("The amount of debt increase per round")]
    public int debtIncrease;

    [Tooltip("The round number")]
    public int round;
    [SerializeField] TMP_Text roundText;

    [Tooltip("Number of rolls given before a round ends")]
    public int numRolls;
    public int rollsLeft;
    [SerializeField] TMP_Text rollsLeftText;

    [SerializeField] GameObject loseScreen;
    [SerializeField] TMP_Text loseStats;

    [SerializeField] Upgrade[] upgrades;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetScore(score);
        StartRound();
    }
    /// <summary>
    /// Set all variables to start the round
    /// </summary>
    public void StartRound()
    {
        scoreCounterText.gameObject.SetActive(false);
        round++;
        debt += debtIncrease;
        rollsLeft = numRolls;
        SetDebt(debt);
        SetRound(round);
    }

    public void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
    }

    public void SetDebt(int debt)
    {
        this.debt = debt;
        debtText.text = debt.ToString();
    }

    public void SetRound(int round)
    {
        this.round = round;
        roundText.text = round.ToString();  
    }

    public void SetRollsLeft(int rollsLeft)
    {
        this.rollsLeft = rollsLeft;
        rollsLeftText.text = rollsLeft.ToString();
    }

    /// <summary>
    /// Called when the total score of a roll is calcuated and reported to the Game Manager
    /// </summary>
    public void AddScore()
    {
        this.score += scoreCounter;
        scoreCounterText.transform.localScale = Vector3.one;
        scoreCounterText.gameObject.SetActive(true);
        scoreCounter = 0;
        if (rollsLeft <= 0)
        {
            score -= debt;
            if(score < 0)
            {
                LoseGame();
            }
            else
            {
                Shop();
            }
        }
    }

    void Shop()
    {

    }

    void LoseGame()
    {
        loseScreen.SetActive(true);
        int highestRound = PlayerPrefs.GetInt("Highest Round", 0);
        PlayerPrefs.SetInt("Highest Round", Mathf.Max(highestRound, round));
        loseStats.text = "Highest Round: " + highestRound.ToString();
    }
    /// <summary>
    /// Add to score counter, increasing in scale as score gets larger
    /// </summary>
    /// <param name="score"></param>
    public void AddScoreCounter(int score)
    {
        scoreCounterText.gameObject.SetActive(true);
        scoreCounter += score;
        scoreCounterText.text = "+" + score.ToString();
        scoreCounterText.transform.localScale = new Vector3(score / scoreCounterScale, score / scoreCounterScale, 0);    
    }



}
