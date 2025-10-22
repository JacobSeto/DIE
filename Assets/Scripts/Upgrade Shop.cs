using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The shop that allows the user to choose a random upgrade from a number of
/// upgrades
/// </summary>
public class UpgradeShop : MonoBehaviour
{

    [SerializeField] GameObject shopUI;
    [SerializeField] Transform UpgradeContainer;
    List<GameObject> UpgradeButtons = new List<GameObject>();
    [Tooltip("The number of upgrades that are revealed in a shop")]
    [SerializeField] int numUpgrades;

    [SerializeField] Dice d6Prefab;
    [SerializeField] Dice coinPrefab;
    [SerializeField] Dice doubleCoinPrefab;
    [SerializeField] Dice oddEvenCoinPrefab;
    [SerializeField] Dice onesCoinPrefab;
    [SerializeField] Dice sillyDiePrefab;
    [SerializeField] Dice nearbyDiePrefab;

    private void Start()
    {
        for (int i = 0; i < UpgradeContainer.childCount; i++)
        {
            UpgradeButtons.Add(UpgradeContainer.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Randomly chooses a number of upgrades to reveal in the shop
    /// </summary>
    public void RandomizeUpgrades()
    {
        foreach(GameObject button in UpgradeButtons)
        {
            button.SetActive(false);
        }
        List<GameObject> temp = new List<GameObject>();
        temp.AddRange(UpgradeButtons);
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

    /// <summary>
    /// Adds a silly die coin that could give a reroll
    /// </summary>
    public void AddSillyDie()
    {
        DiceManager.Instance.AddDice(sillyDiePrefab);
    }

    /// <summary>
    /// Adds a nearby die coin that scales its roll value based on nearby dice
    /// </summary>
    public void AddNearbyDie()
    {
        DiceManager.Instance.AddDice(nearbyDiePrefab);
    }
}
