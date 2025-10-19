using UnityEngine;

/// <summary>
/// Special dice sides that have unique properties
/// </summary>
public enum SpecialSides
{
    Bonus1 = 0,
    BonusOdds,
    BonusEvens,
    Multiply2,
    ReRoll
}
public class Upgrade : MonoBehaviour
{
    public static int BONUS1SCORE;
    public static int BONUSODDSCORE;
    public static int BONUSEVENSSCORE;
    public static int MULTIPLY2AMOUNT;

    [SerializeField] Dice d6Prefab;
    
    /// <summary>
    /// Adds a D6
    /// </summary>
    public void AddD6()
    {
        DiceManager.Instance.dice.Add(d6Prefab);
    }
}
