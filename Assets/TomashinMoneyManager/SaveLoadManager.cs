using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadType SaveLoadType;

    private const string fileBinaryEnd = ".save";
    private const string fileJsonEnd = ".json";
    private static string filePath;

    private static Save save = new Save();

    private void Start()
    {

    }

    public static void SaveValue(Money field, int value)
    {
        switch (SaveLoadType)
        {
            case SaveLoadType.None:
                break;
            case SaveLoadType.PlayerPrefs:
                PlayerPrefs.SetInt(field.moneyType.ToString(), value);
                break;
            case SaveLoadType.Text:
                filePath = Application.persistentDataPath + "/" + field.moneyType.ToString() + fileJsonEnd;
                save.value = value;
                File.WriteAllText(filePath, JsonUtility.ToJson(save));
                break;
            case SaveLoadType.Binary:
                filePath = Application.persistentDataPath + "/" + field.moneyType.ToString() + fileBinaryEnd;
                BinaryFormatter bf = new BinaryFormatter();
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        save.value = value;
                        bf.Serialize(fs, save);
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
        int value = 0;
        switch (SaveLoadType)
        {
            case SaveLoadType.None:
                break;
            case SaveLoadType.PlayerPrefs:
                value = PlayerPrefs.GetInt(field.moneyType.ToString());
                break;
            case SaveLoadType.Text:
                filePath = Application.persistentDataPath + "/" + field.moneyType.ToString() + fileJsonEnd;
                if (File.Exists(filePath))
                {
                    save = JsonUtility.FromJson<Save>(File.ReadAllText(filePath));
                    value = save.value;
                }
                break;
            case SaveLoadType.Binary:
                filePath = Application.persistentDataPath + "/" + field.moneyType.ToString() + fileBinaryEnd;
                BinaryFormatter bf = new BinaryFormatter();
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        save = (Save)bf.Deserialize(fs);
                        value = save.value;
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

[Serializable]
public class Save
{
    public int value;
}
// Изначально планировал сделать сохранение всех валют в один файл, но это было не удобно тем,
// что либо прописывать в классе выше все виды вручную (неудобно),
// либо через массив/лист/словарь (но сложность в доступе, часто приходится объявлять = обнулять)
// как я это делал могу показать, возможно сможете подсказать как надо правильнее, но пока что только так(