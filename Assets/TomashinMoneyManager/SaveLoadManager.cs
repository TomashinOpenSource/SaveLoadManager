using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadType SaveLoadType;

    private const string fileEnd = ".save";
    private static string filePath;

    private void Start()
    {

    }

    public static void SaveValue(Money field, int value)
    {
        filePath = Application.persistentDataPath + "/" + field.moneyType.ToString() + fileEnd;
        switch (SaveLoadType)
        {
            case SaveLoadType.None:
                break;
            case SaveLoadType.PlayerPrefs:
                PlayerPrefs.SetInt(field.moneyType.ToString(), value);
                break;
            case SaveLoadType.Text:
                break;
            case SaveLoadType.Binary:
                BinaryFormatter bf = new BinaryFormatter();
                Money money = new Money();
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        money.value = value;
                        bf.Serialize(fs, money);
                    }
                }
                catch (Exception ioEx)
                {
                    Debug.Log(String.Format("Сохранение {0} завершилось: {1}", field.moneyType, ioEx.Message));
                }
                break;
            case SaveLoadType.Database:
                break;
            default:
                break;
        }
        Debug.Log(string.Format("Сохранение {0} = {1}", field.moneyType, value));
    }

    public static int LoadValue(Money field)
    {
        filePath = Application.persistentDataPath + "/" + field.moneyType.ToString() + fileEnd;
        int value = 0;
        switch (SaveLoadType)
        {
            case SaveLoadType.None:
                break;
            case SaveLoadType.PlayerPrefs:
                value = PlayerPrefs.GetInt(field.moneyType.ToString());
                break;
            case SaveLoadType.Text:
                break;
            case SaveLoadType.Binary:
                BinaryFormatter bf = new BinaryFormatter();
                Money money;
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        money = (Money)bf.Deserialize(fs);
                        value = money.value;
                    }
                }
                catch (Exception ioEx)
                {
                    Debug.Log(String.Format("Загрузка {0} завершилась: {1}", "0", ioEx.Message));
                }
                break;
            case SaveLoadType.Database:
                break;
            default:
                
                break;
        }
        Debug.Log(string.Format("Получение {0} = {1}", field.moneyType, value));
        return value;
    }
}

public enum SaveLoadType
{
    None,
    PlayerPrefs,
    Text,
    Binary,
    Database
}
