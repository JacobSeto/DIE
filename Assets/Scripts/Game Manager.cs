using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Tooltip("The player score")]
    public int score;
    [Space(40)]
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
    [Tooltip("The amount debtRoundIncrease increases by per round")]
    [SerializeField] int debtBonusIncrease;
    [Tooltip("The multiplicative scaling of the debtBonusIncrease, rounded to nearest int")]
    [SerializeField] float debtBonusMultiplier;
    [Tooltip("Interval of rounds before debtBonusIncrease is applied")]
    [SerializeField] int debtBonusIncreaseIntervals;

    int bonusDebt = 0;
    int bonusIncrease = 0;
    int intervalCounter = 0;

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
        shop.AddNearbyDie();
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
        SetRoll(true);
        round++;
        IncreaseDebt();

        rollsLeft = numRolls;
        SetDebt(debt);
        SetRound(round);
        SetRollsLeft(rollsLeft);
    }
    /// <summary>
    /// Increase the debt by checking how many debtInvervals has passed and apply the debtBonusMultiplier.
    /// Then add bonus debt to the debt round increase, and then add that to the debt
    /// </summary>
    void IncreaseDebt()
    {
        if (round % debtBonusIncreaseIntervals == 0)
        {
            bonusIncrease = Mathf.RoundToInt(debtBonusIncrease * (debtBonusMultiplier * intervalCounter));
            intervalCounter++;
            bonusDebt += bonusIncrease;
        }
        debtRoundIncrease += bonusDebt;
        debt += debtRoundIncrease;
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
        SetRoll(true);
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
    /// Sets the roll button component's active component state and determines if the player
    /// can roll their dice
    /// </summary>
    /// <param name="toggle"></param>
    public void SetRoll(bool toggle)
    {
        rollButton.enabled = toggle;
        DiceManager.Instance.canRoll = toggle;
    }

    void LoseGame()
    {
        loseScreen.SetActive(true);
        SetRoll(false);
        PlayerPrefs.SetInt("Highest Round", Mathf.Max(PlayerPrefs.GetInt("Highest Round", 0), round));
        int highestRound = PlayerPrefs.GetInt("Highest Round", 0);
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

    public void ChangeTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }


}
