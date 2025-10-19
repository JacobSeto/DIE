using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Tooltip("The number of dice the player owns")]
    public List<Dice> dice;

    int diceCount;

    [SerializeField] float scoreCalcWaitTime;
    [SerializeField] float scoreCalcEndTime;


    List<int> collectedDiceScores = new List<int>();
    List<SpecialSides> collectedSpecialSides = new List<SpecialSides>();

    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// Rolls all dice
    /// </summary>
    public void RollDice()
    {
        diceCount = 0;
        GameManager.Instance.rollsLeft--;
    }

    public void ReportScore(int score)
    {
        collectedDiceScores.Add(score);
        diceCount++;
        if(diceCount >= dice.Count)
        {
            CalculateScore();
        }

    }

    public void ReportScore(SpecialSides side)
    {
        collectedSpecialSides.Add(side);
        diceCount++;
        if (diceCount >= dice.Count)
        {
            StartCoroutine(CalculateScore());
        }
    }
    /// <summary>
    /// Calculate the score, add to the score counter, then end the round
    /// </summary>
    IEnumerator CalculateScore()
    {
        foreach(SpecialSides side in collectedSpecialSides)
        {
            switch (side)
            {
                case SpecialSides.Bonus1:
                    foreach(int score in collectedDiceScores)
                    {
                        if (score == 1)
                        {
                            collectedDiceScores.Add(Upgrade.BONUS1SCORE);
                        }
                    }
                    break;
                case SpecialSides.BonusOdds:
                    foreach (int score in collectedDiceScores)
                    {
                        if (score % 2 == 1)
                        {
                            collectedDiceScores.Add(Upgrade.BONUSODDSCORE);
                        }
                    }
                    break;
                case SpecialSides.BonusEvens:
                    foreach (int score in collectedDiceScores)
                    {
                        if (score % 2 == 0)
                        {
                            collectedDiceScores.Add(Upgrade.BONUSEVENSSCORE);
                        }
                    }
                    break;
                case SpecialSides.Multiply2:
                    int sum = 0;
                    foreach (int score in collectedDiceScores)
                    {
                        sum += score;
                    }
                    collectedDiceScores.Add(sum);
                    break;
                case SpecialSides.ReRoll:
                    GameManager.Instance.SetRollsLeft(GameManager.Instance.rollsLeft + 1);
                    break;
            }
        }
        foreach(int score in collectedDiceScores)
        {
            yield return new WaitForSeconds(scoreCalcWaitTime);
            GameManager.Instance.AddScoreCounter(score);
        }
        yield return new WaitForSeconds(scoreCalcEndTime);
        GameManager.Instance.AddScore();
    }
}
