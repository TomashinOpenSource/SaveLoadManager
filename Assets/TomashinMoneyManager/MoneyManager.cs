using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public SaveLoadType SaveLoadType;
    public Money[] money;

    void Start()
    {
        if (!GetComponent<SaveLoadManager>()) gameObject.AddComponent<SaveLoadManager>();
        SaveLoadManager.SaveLoadType = SaveLoadType;

        foreach (var item in money)
        {
            item.Value = SaveLoadManager.LoadValue(item);
        }
    }

    void Update()
    {
        
    }
    
}

[Serializable]
public class Money
{
    public MoneyType moneyType;
    public int value;
    public Text valueField; // Лучше всего использовать TMP_Text, но чтобы не подключать еще один плагин, использую дефолтный от Unity

    public int Value
    {
        get
        {
            //value = SaveLoadManager.LoadValue(this);
            return value;
        }
        set
        {
            this.value = value;
            valueField.text = moneyType.ToString() + ": " + value.ToString();
            SaveLoadManager.SaveValue(this, value);
        }
        // Загрузка и Сохранение стоит внутри внутри свойства значения валюты для упрощения работы.
        // Данный способ надежнее по защите - изменение происходит только после загрузки актуального значения
        // Но добавляется нагрузка из-за частой работы с системой
    }
}

public enum MoneyType
{
    Coins, Crystals
}