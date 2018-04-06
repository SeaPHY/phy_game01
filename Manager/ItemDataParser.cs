using System.IO;    // 파일 파싱
using System.Xml.Serialization; // XML
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;  // LitJson
using System.Xml;


public class ItemDataParser : MonoBehaviour
{
    public void MySqlJsonDataParser ()
    {
        //if (localTureWebFalse)
        //{
        //    StartCoroutine (PhpJsonParser (ampsetupPostion , ampsetupEquippableItem));
        //}
        //else
        //{
        //    StartCoroutine (PhpJsonParser (ampsetupPostion , ampsetupEquippableItem));
        //}
        // 우비 너무 느려서 안되겠다.

        StartCoroutine (PhpJsonParser (ampsetupPostion , ampsetupEquippableItem));
    }

    IEnumerator PhpJsonParser (string postionURL, string equippableItemURL)
    {
        yield return StartCoroutine (PhpJsonPostionData (postionURL));

        yield return StartCoroutine (PhpJsonEquippableItemData (equippableItemURL));
    }

    IEnumerator PhpJsonPostionData (string postionURL)
    {
        WWW webRequest = new WWW (postionURL);
        yield return webRequest;

        string jsonString = webRequest.text;

        PortionData = JsonMapper.ToObject<List<Portion>> (jsonString);

        webRequest.Dispose ();
    }

    IEnumerator PhpJsonEquippableItemData (string equippableItemURL)
    {
        WWW webRequest = new WWW (equippableItemURL);
        yield return webRequest;

        string jsonString = webRequest.text;

        EquippableItemData = JsonMapper.ToObject<List<EquippableItem>> (jsonString);

        webRequest.Dispose ();
    }


    public T[] JsonParser<T> (T[] TParserType , string filePath) where T : class
    {
        if (Application.isEditor)
        {
            //UnityProjects/ProjectsName/Assets/Resources/ + filePath;
            filePath = $"{resourecesFolderPath + filePath}.json";

            if (File.Exists ((filePath)))
            {
                string jsonString = File.ReadAllText (filePath);
                TParserType = JsonMapper.ToObject<T[]> (jsonString);
            }
            else
            {
                Debug.LogError ($"JsonDataConvert.JsonParser Error, filePath: {filePath} Error!");
            }
        }
        else
        {
            TextAsset textAsset = Resources.Load<TextAsset> (filePath);
            TParserType = JsonMapper.ToObject<T[]> (textAsset.text);
        }

        return TParserType;
    }

    public void SaveJson<T> (T[] TsaveList , string filePath) where T : class
    {
        //UnityProjects/ProjectsName/Assets/Resources/ + filePath;
        filePath = $"{resourecesFolderPath + filePath}.json";

        JsonData svaeJson = JsonMapper.ToJson (TsaveList);

        // 지정한 경로에 텍스트 파일 생성.
        // 프로젝트 폴더/Assets/Resource/itemData.json
        File.WriteAllText (filePath , svaeJson.ToString ());
    }

    // 저장할 클래스의 XML 어트리뷰트 설정이 필요... 하진 않더라. 없어도 됨
    // XML 형태로 저장
    // 폴더를 만들어 주진 않는다. 폴더를 만들고 저장 해야 함.
    void XMLSerializeSave<T> (T TSerializeData , string filePath)
    {
        ///UnityProjects/ProjectsName/Assets/Resources/ + filePath;
        filePath = $"{resourecesFolderPath + filePath}.xml";

        // Employee 타입으로 XmlSerializer객체를 생성
        // serializer이 매개변수 emp를 writer에 넘겨줌
        XmlSerializer serializer = new XmlSerializer (typeof (T));
        TextWriter writer = new StreamWriter (filePath);
        serializer.Serialize (writer , TSerializeData);
        writer.Close ();
    }

    // XML을 입력 받은 클래스로 변환
    T[] XMLDeserializeLoad<T> (T[] TParserType , string filePath)
    {

        if (Application.isEditor)
        {
            //UnityProjects/ProjectsName/Assets/Resources/ + filePath;
            filePath = $"{resourecesFolderPath + filePath}.xml";

            if (File.Exists (filePath))
            {
                XmlSerializer serializer = new XmlSerializer (typeof (T[]));

                // 파일에서 데이터를 읽어 옴.
                FileStream stream = new FileStream (filePath , FileMode.Open);

                // XmlSerializer.Deserialize을 이용해서 클래스로 변환
                TParserType = (T[])serializer.Deserialize (stream);
                stream.Close ();
            }
            else
            {
                Debug.LogError ($"XmlDataConvert.XMLDeserializeLoad Error, filePath: {filePath} Error!");
            }
        }
        else
        {
            jsonTureXmlFalse = true;
            ItemDataLoad ();
            return null;
        }

        return TParserType;
    }

    string resourecesFolderPath;
    string jsonPostion = "Json/Postion";
    string jsonEquippableItem = "Json/EquippableItem";
    string xmlPostion = "XML/Postion";
    string xmlEquippableItem = "XML/EquippableItem";
    string ampsetupPostion = "127.0.0.1/Unity/mysqlitemData_Postion.php";
    string ampsetupEquippableItem = "127.0.0.1/Unity/mysqlitemData_EquippableItem.php";

    public bool jsonTureXmlFalse;
    //public bool localTureWebFalse;
    public bool dbDataLoad;

    
    public List<Portion> PortionData = new List<Portion>();
    public List<EquippableItem> EquippableItemData = new List<EquippableItem>();


    [ContextMenu ("Load")]
    public void ItemDataLoad ()
    {

        if (dbDataLoad)
        {
            MySqlJsonDataParser ();

            Debug.Log ("MySqlJsonDataParser");
        }
        else
        {
            if (jsonTureXmlFalse)
            {
                PortionData = new List<Portion> (JsonParser (PortionData.ToArray () , jsonPostion));
                EquippableItemData = new List<EquippableItem> (JsonParser (EquippableItemData.ToArray () , jsonEquippableItem));

                Debug.Log ("Resources Json Load");
            }
            else
            {
                PortionData = new List<Portion> (XMLDeserializeLoad (PortionData.ToArray () , xmlPostion));
                EquippableItemData = new List<EquippableItem> (XMLDeserializeLoad (EquippableItemData.ToArray () , xmlEquippableItem));

                Debug.Log ("Resources XML Load");
            }
        }
        
    }

    [ContextMenu ("Save")]
    public void ItemSave ()
    {
        if (jsonTureXmlFalse)
        {
            SaveJson (PortionData.ToArray () , jsonPostion);
            SaveJson (EquippableItemData.ToArray () , jsonEquippableItem);

            Debug.Log ("Resources Json Save");
        }
        else
        {
            XMLSerializeSave (PortionData.ToArray () , xmlPostion);
            XMLSerializeSave (EquippableItemData.ToArray () , xmlEquippableItem);

            Debug.Log ("Resources XML Save");
        }
    }

    public void Awake ()
    {
        resourecesFolderPath = $"{Application.dataPath}/Resources/";
        ItemDataLoad ();

        DontDestroyOnLoad (this.gameObject);
    }
}
