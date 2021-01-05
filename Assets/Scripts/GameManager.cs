using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MoneyManager moneyManager;

    public MoneyManager MoneyManager
    { 
        get => (moneyManager == null) ? MoneyManager = FindObjectOfType<MoneyManager>() : moneyManager;
        set => moneyManager = value; 
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    public void OnChangeButtonPressed()
    {
        foreach (var money in MoneyManager.money)
        {
            money.Value += Random.Range(-5, 6);
        }
    }
    public void OnNullButtonPressed()
    {
        foreach (var money in MoneyManager.money)
        {
            money.Value = 0;
        }
    }
}