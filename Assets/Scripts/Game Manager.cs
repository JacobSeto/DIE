using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Tooltip("The player score")]
    public int score;
    [Space]
    [SerializeField] TMP_Text scoreText;

    [Tooltip("Counts up the score added from that round")]
    [HideInInspector] int scoreCounter;
    [SerializeField] TMP_Text scoreCounterText;

    [Tooltip("Score that doubles the text size")]
    [SerializeField] float scoreCounterScale;

    [Tooltip("The player debt for the round")]
    [HideInInspector] public int debt;
    [SerializeField] TMP_Text debtText;
    [Tooltip("The amount of debt increase per round")]
    public int debtRoundIncrease;
    [Tooltip("The amount round debt increases by")]
    [SerializeField] int debtBonusIncrease;
    [Tooltip("Interval of rounds before debtBonusIncrease is applied")]
    [SerializeField] int debtBonusIncreaseIntervals;

    int bonusDebt = 0;

    [Tooltip("The round number")]
    [HideInInspector] public int round;
    [SerializeField] TMP_Text roundText;

    [Tooltip("Number of rolls given before a round ends")]
    public int numRolls;
    [HideInInspector] public int rollsLeft;
    [SerializeField] TMP_Text rollsLeftText;

    [SerializeField] GameObject loseScreen;
    [SerializeField] TMP_Text loseStats;

    [SerializeField] UpgradeShop shop;

    [SerializeField] Button rollButton;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        shop.AddD6();
        shop.AddCoin();
        shop.AddDoubleCoin();
        shop.AddOddEvenCoin();
        shop.AddOnesCoin();
        SetScore(score);
        StartRound();
    }
    /// <summary>
    /// Set all variables to start the round
    /// </summary>
    public void StartRound()
    {
        shop.HideShop();
        scoreCounterText.gameObject.SetActive(false);
        round++;
        bonusDebt = round % debtBonusIncreaseIntervals == 0 ? bonusDebt + debtBonusIncrease : bonusDebt;
        debt += debtRoundIncrease + bonusDebt;

        rollsLeft = numRolls;
        SetDebt(debt);
        SetRound(round);
        SetRollsLeft(rollsLeft);
    }

    public void SetScore(int score)
    {
        this.score = score;
        scoreText.text = "Score: " + score.ToString();
    }

    public void SetDebt(int debt)
    {
        this.debt = debt;
        debtText.text = "Debt: " + debt.ToString();
    }

    public void SetRound(int round)
    {
        this.round = round;
        roundText.text = "Round: " + round.ToString();  
    }

    public void SetRollsLeft(int rollsLeft)
    {
        this.rollsLeft = rollsLeft;
        rollsLeftText.text = "Rolls: " + rollsLeft.ToString();
    }

    /// <summary>
    /// Called when the total score of a roll is calcuated and reported to the Game Manager
    /// </summary>
    public void AddRoundScore()
    {
        SetScore(score += scoreCounter);
        SetRollButton(true);
        scoreCounterText.transform.localScale = Vector3.one;
        scoreCounterText.gameObject.SetActive(false);
        scoreCounter = 0;
        if (rollsLeft <= 0)
        {
            SetScore(score - debt);
            if(score < 0)
            {
                LoseGame();
            }
            else
            {
                shop.OpenShop();
            }
        }
    }
    /// <summary>
    /// Sets the roll button component's active component state
    /// </summary>
    /// <param name="toggle"></param>
    public void SetRollButton(bool toggle)
    {
        rollButton.enabled = toggle;
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
        scoreCounterText.text = "+" + scoreCounter.ToString();
        scoreCounterText.transform.localScale = new Vector3(1 + scoreCounter / scoreCounterScale, 1 + scoreCounter / scoreCounterScale, 0);    
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



}
