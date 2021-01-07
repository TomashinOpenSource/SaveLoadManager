using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.Networking;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadType SaveLoadType;

    private static Save save = new Save();

    private static string filePath;
    private const string fileBinaryEnd = ".save";
    private const string fileJsonEnd = ".json";

    private const string URL_SaveLoadManagerPHP = "http://194.67.111.214/Database/SaveLoadManager.php";
    private const string dbname = "testtaskzimad";
    private const string table = "testtaskzimad";
    private const int uid = 1;

    #region SaveLoadManager & MoneyManager
    public static SaveLoadManager SLM;
    public static MoneyManager MoneyManager;
    private void Awake()
    {
        SLM = this;
        MoneyManager = GetComponent<MoneyManager>();
    }
    #endregion

    private void Start()
    {

    }

    public static int UpdateValue(UpdateType type, Money field, int value = 0)
    {
        switch (SaveLoadType)
        {
            case SaveLoadType.None:
                break;
            case SaveLoadType.PlayerPrefs:
                if (type == UpdateType.Save) PlayerPrefs.SetInt(field.moneyType.ToString(), value);
                else value = PlayerPrefs.GetInt(field.moneyType.ToString());
                break;
            case SaveLoadType.Text:
                filePath = Application.persistentDataPath + "/" + field.moneyType.ToString() + fileJsonEnd;
                if (type == UpdateType.Save)
                {
                    save.value = value;
                    File.WriteAllText(filePath, JsonUtility.ToJson(save));
                }
                else if (File.Exists(filePath))
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
                        if (type == UpdateType.Save)
                        {
                            save.value = value;
                            bf.Serialize(fs, save);
                        }
                        else
                        {
                            save = (Save)bf.Deserialize(fs);
                            value = save.value;
                        }

                    }
                }
                catch (Exception ioEx)
                {
                    Debug.Log(string.Format("{0} завершилось с значением {1} и ошибкой {2}", type, field.moneyType, ioEx.Message));
                }
                break;
            case SaveLoadType.Database:
                SLM.StartCoroutine(ConnectToServer(type, field, value));
                break;
            default:
                break;
        }
        return value;
    }

    private static IEnumerator ConnectToServer(UpdateType type, Money field, int value = 0)
    {
        WWWForm form = new WWWForm();
        form.AddField("dbname", dbname);
        form.AddField("table", table);
        form.AddField("uid", uid);
        form.AddField("field", field.moneyType.ToString());
        form.AddField("type", type.ToString());
        form.AddField("value", value);

        using (UnityWebRequest www = UnityWebRequest.Post(URL_SaveLoadManagerPHP, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("ERROR: " + www.error);
            }
            else
            {
                if (type == UpdateType.Load)
                {
                    if (www.downloadHandler.text != "")
                    {
                        //Debug.Log(www.downloadHandler.text);
                        foreach (var item in MoneyManager.money)
                        {
                            if (item.moneyType == field.moneyType)
                            {
                                value = Convert.ToInt32(www.downloadHandler.text);
                                item.Value = value;
                                //slm.StartCoroutine(ConnectToServer(UpdateType.Save, field, value));
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("NULL");
                    }
                }
                else
                {
                    //Debug.Log(www.downloadHandler.text);
                }
            }
        }
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

public enum UpdateType
{
    Save, Load
}