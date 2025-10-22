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
    bool isRolling;

    [SerializeField] float scoreCalcWaitTime;
    [Tooltip("subtract this time to wait time to speed up calculation")]
    [SerializeField] float scoreCalcSpeedBonus;
    [Tooltip("The fastest the score can be calculated")]
    [SerializeField] float minScoreCalcWaitTime;
    [SerializeField] float FinalWaitTime;

    float waitTime;

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
        if (isRolling)
        {
            return;
        }
        isRolling = true;
        GameManager.Instance.SetRollButton(false);
        diceCount = 0;
        collectedDiceScores.Clear();
        GameManager.Instance.SetRollsLeft(GameManager.Instance.rollsLeft - 1);
        foreach(Dice dice in diceList)
        {
            StartCoroutine(dice.Roll());
        }
    }
    /// <summary>
    /// Report score and property unless not rolling
    /// </summary>
    /// <param name="score"></param>

    public void ReportScore(int score, SideProperties side)
    {
        if (!isRolling)
        {
            return;
        }
        diceCount++;
        CalculateScore(score, side);
        
    }
    /// <summary>
    /// Calculate the score, add to the score counter. If all dice are rolled, add to the round score
    /// and stop rolling logic
    /// </summary>
    void CalculateScore(int score, SideProperties side)
    {
        collectedDiceScores.Add(score);
        switch (side)
        {
            case SideProperties.Bonus1:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        if (collectedScore == 1)
                        {
                            sum += UpgradeShop.BONUS1SCORE;
                        }
                    }
                    collectedDiceScores.Add(sum);
                    break;
                }

            case SideProperties.BonusOdds:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        if ((collectedScore % 2) == 1)
                        {
                            sum += UpgradeShop.BONUSODDSCORE;
                        }
                    }
                    collectedDiceScores.Add(sum);
                    break;
                }

            case SideProperties.BonusEvens:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        if ((collectedScore % 2) == 0)
                        {
                            sum += UpgradeShop.BONUSEVENSSCORE;
                        }
                    }
                    collectedDiceScores.Add(sum);
                    break;
                }

            case SideProperties.Multiply2:
                {
                    int sum = 0;
                    foreach (int collectedScore in collectedDiceScores)
                    {
                        sum += collectedScore;
                    }
                    collectedDiceScores.Add(sum);
                    break;
                }

            case SideProperties.ReRoll:
                GameManager.Instance.SetRollsLeft(GameManager.Instance.rollsLeft + 1);
                break;
        }
        if (diceCount >= diceList.Count)
        {
            StartCoroutine(ReportScoreRound());
        }
    }
    /// <summary>
    /// Reports the score round by going through all the calculated scores and adding them to the score
    /// counter, then finally adding that sum to the round score and ending the rolling phase
    /// </summary>
    IEnumerator ReportScoreRound()
    {
        isRolling = false;
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
