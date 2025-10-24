using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Tooltip("The player score")]
    public int score;
    [Space(20)]
    [SerializeField] TMP_Text scoreText;
    [Tooltip("Counts up the score added from that round")]
    [HideInInspector] int scoreCounter;
    [SerializeField] TMP_Text scoreCounterText;
    [Tooltip("Score that doubles the text size")]
    [SerializeField] float scoreCounterScale;
    [SerializeField] float maxScoreScale;

    [Header("Round")]
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

    [Tooltip("Round")]
    [HideInInspector] public int round;
    [SerializeField] TMP_Text roundText;
    [Tooltip("rolls given per round")]
    public int numRolls;
    [SerializeField] int maxRollsPerRound;
    [Tooltip("Decreases each roll to prevent infinite rolling")]
    [HideInInspector] public int maxRollsLeft;
    [HideInInspector] public int rollsLeft;
    [SerializeField] TMP_Text rollsLeftText;
    bool canRoll;

    [Header("Game UI")]

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
        shop.AddD8();
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
        SetDebt(debt);
        SetRound(round);
        maxRollsLeft = maxRollsPerRound;
        SetRollsLeft(numRolls);
    }
    /// <summary>
    /// Attempts to roll if the player hasn't rolled yet. Reduces the max rolls by 1 and
    /// tells the <see cref="DiceManager"/> to roll all Dice
    /// </summary>
    public void OnRoll()
    {
        if (canRoll)
        {
            SetRoll(false);
            maxRollsLeft--;
            SetRollsLeft(rollsLeft - 1);
            DiceManager.Instance.RollDice();
        }
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
    /// <summary>
    /// Set rolls left, with numRolls being the max rolls
    /// </summary>
    /// <param name="rollsLeft"></param>
    public void SetRollsLeft(int rollsLeft)
    {
        this.rollsLeft = Mathf.Min(maxRollsLeft, rollsLeft);
        rollsLeftText.text = "Rolls: " + rollsLeft.ToString() + '/' + maxRollsLeft.ToString();
    }

    /// <summary>
    /// Called when the total score of a roll is calcuated and reported to the Game Manager
    /// </summary>
    public void AddRoundScore()
    {
        SetScore(score += scoreCounter);
        scoreCounterText.transform.localScale = Vector3.one;
        scoreCounterText.gameObject.SetActive(false);
        scoreCounter = 0;
        if (rollsLeft <= 0)
        {
            SetScore(score - debt);
            SetRoll(false);
            if (score < 0)
            {
                LoseGame();
            }
            else
            {
                shop.OpenShop();
            }
        }
        else
        {
            SetRoll(true);
        }
    }
    /// <summary>
    /// Toggles whether or not the player can roll their dice
    /// </summary>
    /// <param name="toggle"></param>
    public void SetRoll(bool toggle)
    {
        rollButton.enabled = toggle;
        canRoll = toggle;
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
        float scale = Mathf.Min(1 + scoreCounter / scoreCounterScale, maxScoreScale);
        scoreCounterText.transform.localScale = new Vector3(scale, scale, 0);
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
