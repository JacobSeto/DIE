using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Tooltip("The number of dice the player owns")]
    List<Dice> diceList = new List<Dice>();

    [Tooltip("Where all the dice are added to the scene")]
    [SerializeField] Transform diceContainer;

    int diceCount;
    public bool canRoll;
    bool reportScores;

    [SerializeField] float scoreCalcWaitTime;
    [Tooltip("subtract this time to wait time to speed up calculation")]
    [SerializeField] float scoreCalcSpeedBonus;
    [Tooltip("The fastest the score can be calculated")]
    [SerializeField] float minScoreCalcWaitTime;
    [SerializeField] float FinalWaitTime;

    [SerializeField] RollDisplay scoreDisplayPrefab;
    [Tooltip("Add y height to line up above a dice")]
    [SerializeField] float scoreDisplayHeightOffset;

    float waitTime;

    [Header("Dice Properties")]
    [SerializeField] int bonus1Score;
    [SerializeField] int bonusOddsScore;
    [SerializeField] int bonusEvensScore;
    [SerializeField] float nearBonusRadius;
    [SerializeField] LayerMask diceLayer;

    /// <summary>
    /// The dice score rolled that round
    /// </summary>
    List<int> collectedDiceScores = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Adds a dice to the rolling board
    /// </summary>
    /// <param name="dice"></param>
    public void AddDice(Dice dice)
    {
        Dice newDice = Instantiate(dice, new Vector3(0, 5, 3), Quaternion.identity);
        newDice.transform.SetParent(diceContainer);
        diceList.Add(newDice);
    }

    public void OnRoll()
    {
        RollDice();
    }

    /// <summary>
    /// Rolls all dice
    /// </summary>
    public void RollDice()
    {
        if (!canRoll)
        {
            return;
        }
        reportScores = true;
        GameManager.Instance.SetRoll(false);
        diceCount = 0;
        collectedDiceScores.Clear();
        GameManager.Instance.SetRollsLeft(GameManager.Instance.rollsLeft - 1);
        foreach(Dice dice in diceList)
        {
            StartCoroutine(dice.Roll());
        }
    }
    /// <summary>
    /// Report value and property unless not rolling
    /// </summary>
    /// <param name="value"></param>

    public void ReportScore(int value, SideProperties side, Dice dice)
    {
        if (!reportScores)
        {
            return;
        }
        diceCount++;
        CalculateScore(value, side, dice);
        
    }
    /// <summary>
    /// Calculate the score of the roll. If all dice are rolled, add to the round score
    /// and stop rolling logic
    /// </summary>
    void CalculateScore(int value, SideProperties side, Dice dice)
    {
        switch (side)
        {
            case SideProperties.None:
                CreateRollDisplay(dice, value);
                break;
            case SideProperties.Bonus1:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        if (collectedScore == 1)
                        {
                            sum += bonus1Score;
                        }
                    }
                    collectedDiceScores.Add(sum);
                    CreateRollDisplay(dice, value);
                    break;
                }

            case SideProperties.BonusOdds:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        if ((collectedScore % 2) == 1)
                        {
                            sum += bonusOddsScore;
                        }
                    }
                    collectedDiceScores.Add(sum);
                    CreateRollDisplay(dice, value);
                    break;
                }

            case SideProperties.BonusEvens:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        if ((collectedScore % 2) == 0)
                        {
                            sum += bonusEvensScore;
                        }
                    }
                    collectedDiceScores.Add(sum);
                    CreateRollDisplay(dice, value);
                    break;
                }

            case SideProperties.Multiply:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        sum += collectedScore;
                    }
                    collectedDiceScores.Add(sum * (value-1));
                    CreateRollDisplay(dice, "X" + value.ToString());
                    break;
                }

            case SideProperties.ReRoll:
                GameManager.Instance.SetRollsLeft(GameManager.Instance.rollsLeft + 1);
                CreateRollDisplay(dice, "REROLL");
                break;

            case SideProperties.NearBonus:
                {
                    Collider[] hitColliders = Physics.OverlapSphere(dice.transform.position,
                        nearBonusRadius, diceLayer, QueryTriggerInteraction.Ignore);
                    value = value * hitColliders.Length;
                    CreateRollDisplay(dice, value);
                    break;
                }
        }
        collectedDiceScores.Add(value);

        if (diceCount >= diceList.Count)
        {
            reportScores = false;
            StartCoroutine(ReportScoreRound());
        }
    }

    /// <summary>
    /// Creates a score display above a given dice
    /// </summary>
    /// <param name="dice">The dice to place the score above</param>
    /// <param name="specialText">The reported score</param>
    void CreateRollDisplay(Dice dice,string specialText)
    {
        RollDisplay scoreDisplay = Instantiate(scoreDisplayPrefab, dice.transform.position
            + new Vector3(0, scoreDisplayHeightOffset, 0), Quaternion.identity);
        scoreDisplay.SetSpecialDisplay(specialText);
    }
    /// <summary>
    /// Creates a special display above a given dice
    /// </summary>
    /// <param name="dice">The dice to place the score above</param>
    /// <param name="score">The reported score</param>
    void CreateRollDisplay(Dice dice, int score)
    {
        RollDisplay scoreDisplay = Instantiate(scoreDisplayPrefab, dice.transform.position
            + new Vector3(0, scoreDisplayHeightOffset, 0), Quaternion.identity);
        scoreDisplay.SetScoreDisplay(score);
    }
    /// <summary>
    /// Reports the score round by going through all the calculated scores and adding them to the score
    /// counter, then finally adding that sum to the round score and ending the rolling phase
    /// </summary>
    IEnumerator ReportScoreRound()
    {
        waitTime = scoreCalcWaitTime;
        foreach (int score in collectedDiceScores)
        {
            if(score != 0)
            {
                yield return new WaitForSeconds(waitTime);
                GameManager.Instance.AddScoreCounter(score);
                waitTime = Mathf.Max(minScoreCalcWaitTime, waitTime - scoreCalcSpeedBonus);
            }
        }
        yield return new WaitForSeconds(FinalWaitTime);
        GameManager.Instance.AddRoundScore();
    }
}
