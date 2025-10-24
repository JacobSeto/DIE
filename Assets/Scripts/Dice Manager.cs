using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
/// <summary>
/// Controls the logic of all dice in the game
/// </summary>
public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Tooltip("The number of dice the player owns")]
    List<Dice> diceList = new List<Dice>();

    [Tooltip("Where all the dice are added to the scene")]
    [SerializeField] Transform diceContainer;

    int diceCount;
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

    /// <summary>
    /// Rolls all dice 
    /// </summary>
    public void RollDice()
    {
        reportScores = true;
        diceCount = 0;
        collectedDiceScores.Clear();
        foreach(Dice dice in diceList)
        {
            StartCoroutine(dice.Roll());
        }
    }

    /// <summary>
    /// Reports value and dice property unless not rolling
    /// </summary>

    public void ReportScore(Dice dice, float value, SideProperties side)
    {
        if (!reportScores)
        {
            return;
        }
        diceCount++;
        CalculateScore(dice, value, side);
        
    }

    /// <summary>
    /// Gives a bonus for each 1 scored
    /// </summary>
    void Bonus1(Dice dice, int score)
    {
        int sum = 0;
        foreach (int collectedScore in collectedDiceScores)
        {
            if (collectedScore == 1)
            {
                sum += bonus1Score;
            }
        }
        sum += score;
        collectedDiceScores.Add(sum);
        CreateRollDisplay(dice, sum);
    }
    /// <summary>
    /// Adds bonuses to the parity of previous rolls
    /// </summary>
    void Parity(Dice dice, int score, bool isOdd)
    {
        int sum = 0;
        if (isOdd)
        {
            foreach (int collectedScore in collectedDiceScores)
            {
                if ((collectedScore % 2) == 1)
                {
                    sum += bonusOddsScore;
                }
            }
        }
        else
        {
            foreach (int collectedScore in collectedDiceScores)
            {
                if ((collectedScore % 2) == 1)
                {
                    sum += bonusEvensScore;
                }
            }
        }
        sum += score;
        collectedDiceScores.Add(sum);
        CreateRollDisplay(dice, sum);
    }
    /// <summary>
    /// Score based on the multiplication of previous rolls
    /// </summary>
    void Multiply(Dice dice, float value)
    {
        float sum = 0;
        foreach (int collectedScore in collectedDiceScores)
        {
            sum += collectedScore;
        }
        collectedDiceScores.Add((int)Mathf.Round(sum * (value - 1)));
        CreateRollDisplay(dice, "X" + value.ToString());
    }

    /// <summary>
    /// Grants another reroll for the round
    /// </summary>
    void ReRoll(Dice dice)
    {
        GameManager.Instance.SetRollsLeft(GameManager.Instance.rollsLeft + 1);
        CreateRollDisplay(dice, "REROLL");
    }
    /// <summary>
    /// Score that scales with the number of other dice around it
    /// </summary>
    void NearBonus(Dice  dice, float value)
    {
        Collider[] hitColliders = Physics.OverlapSphere(dice.transform.position,
                        nearBonusRadius, diceLayer, QueryTriggerInteraction.Ignore);
        int score = (int)Mathf.Round(value * hitColliders.Length);
        collectedDiceScores.Add(score);
        CreateRollDisplay(dice, score);
    }

    /// <summary>
    /// Calculate the score of the roll. If all dice are rolled, add to the round score
    /// and stop rolling logic
    /// </summary>
    void CalculateScore(Dice dice, float value, SideProperties side)
    {
        switch (side)
        {
            case SideProperties.None:
                collectedDiceScores.Add((int)value);
                CreateRollDisplay(dice, (int)value);
                break;
            case SideProperties.Bonus1:
                Bonus1(dice, (int)value);
                break;
            case SideProperties.BonusOdds:
                Parity(dice, (int)value, true);
                break;
            case SideProperties.BonusEvens:
                Parity(dice, (int)value, false);
                break;
            case SideProperties.Multiply:
                Multiply(dice, value);
                break;
            case SideProperties.ReRoll:
                ReRoll(dice);
                break;
            case SideProperties.NearBonus:
                NearBonus(dice, value);
                break;
        }

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
