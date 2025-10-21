using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The shop that allows the user to choose a random upgrade from a number of
/// upgrades
/// </summary>
public class UpgradeShop : MonoBehaviour
{
    public static int BONUS1SCORE = 10;
    public static int BONUSODDSCORE = 3;
    public static int BONUSEVENSSCORE = 3;
    public static int MULTIPLY2AMOUNT = 2;

    [SerializeField] GameObject shopUI;
    [Tooltip("All possible upgrades that can appear in the shop")]
    [SerializeField] List<GameObject> Upgrades = new List<GameObject>();
    [Tooltip("The number of upgrades that are revealed in a shop")]
    [SerializeField] int numUpgrades;

    [SerializeField] Dice d6Prefab;
    [SerializeField] Dice coinPrefab;
    [SerializeField] Dice doubleCoinPrefab;
    [SerializeField] Dice oddEvenCoinPrefab;
    [SerializeField] Dice onesCoinPrefab;

    /// <summary>
    /// Randomly chooses a number of upgrades to reveal in the shop
    /// </summary>
    public void RandomizeUpgrades()
    {
        foreach(GameObject upgrade in Upgrades)
        {
            upgrade.SetActive(false);
        }
        List<GameObject> temp = new List<GameObject>();
        temp.AddRange(Upgrades);
        for(int i = 0; i < numUpgrades; i++)
        {
            int ranIndex = Random.Range(0, temp.Count);
            temp[ranIndex].SetActive(true);
            temp.Remove(temp[ranIndex]);
        }
    }
    /// <summary>
    /// Opens the shop by revealing the UI and revealing random upgrades
    /// </summary>
    public void OpenShop()
    {
        shopUI.SetActive(true);
        RandomizeUpgrades();
    }

    /// <summary>
    /// Hides the shop
    /// </summary>
    public void HideShop()
    {
        shopUI.SetActive(false);
    }

    /// <summary>
    /// Adds a D6
    /// </summary>
    public void AddD6()
    {
        DiceManager.Instance.AddDice(d6Prefab);
    }

    /// <summary>
    /// Adds a coin
    /// </summary>
    public void AddCoin()
    {
        DiceManager.Instance.AddDice(coinPrefab);
    }
    /// <summary>
    /// Adds a coin that doubles the score
    /// </summary>
    public void AddDoubleCoin()
    {
        DiceManager.Instance.AddDice(doubleCoinPrefab);
    }

    /// <summary>
    /// Adds a coin that gives bonuses to odd or even rolls
    /// </summary>
    public void AddOddEvenCoin()
    {
        DiceManager.Instance.AddDice(oddEvenCoinPrefab);
    }

    /// <summary>
    /// Adds a coin that gives bonuses to rolling ones
    /// </summary>
    public void AddOnesCoin()
    {
        DiceManager.Instance.AddDice(onesCoinPrefab);
    }
}
