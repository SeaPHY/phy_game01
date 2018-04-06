using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

// 게임 매니저
// 전체적인 게임 진행을 관리함, 턴 진행 등
public class GameManager : MonoBehaviour
{
    private int stageCount;
    private int turnCount;
    public int randomSeed;      // 셔플용 시드.

    // 게임 시작할때 등급별로 아이템을 저장할 변수
    List<Portion> PortionList = new List<Portion> ();
    [SerializeField] List<EquippableItem> CommonItemList = new List<EquippableItem> ();
    [SerializeField] List<EquippableItem> UncommonItemList = new List<EquippableItem> ();
    [SerializeField] List<EquippableItem> RareItemList = new List<EquippableItem> ();
    [SerializeField] List<EquippableItem> EpicItemList = new List<EquippableItem> ();

    public Dictionary<string , Sprite> itemIconDictionaty = new Dictionary<string , Sprite> ();

    // 실제 게임에서 사용할 아이템 리스트
    // 장비 아이템은 중복해서 떨어지지 않는다.
    public List<Portion> CurrentPortionList;
    public Queue<EquippableItem> CurrentCommonItemList;
    public Queue<EquippableItem> CurrentUncommonItemList;
    public Queue<EquippableItem> CurrentRareItemList;
    public Queue<EquippableItem> CurrentEpicItemList;
    public ItemDataParser itemDataBase;

    // 등급별 아이템 등장 확률 0 ~ 1
    [Range (0 , 1)] public float uncommonProbability = .3f;
    [Range (0 , 1)] public float rareProbability = .01f;
    [Range (0 , 1)] public float epicProbability = .05f;
    [Range (0 , 1)] public float portionProbability = .5f;

    public Inventory inventory;

    public Text pauseText;

    bool isItemDataGet = false;
    bool fastGame = false;

    public int StageCount
    {
        get { return stageCount; }
        set
        {
            stageCount = value;
            stageCountText.text = $"Stage: {stageCount}";
        }
    }

    public int TurnCount
    {
        get { return turnCount; }
        set
        {
            turnCount = value;
            turnCountText.text = $"Turn: {turnCount}";
        }
    }

    public Text turnCountText;
    public Text stageCountText;

    public TileMapManager tileMapManager;
    Generator generator;
    public event System.Action ActionOnTurn;
    public event System.Action ActionNewGame;
    CameraMovement cameraMovement;
    UIManager uiManager;

    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager ();
            }

            return instance;
        }
    }

    private void Awake ()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy (gameObject);
        }

        tileMapManager = GetComponent<TileMapManager> ();
        generator = GetComponent<Generator> ();
        cameraMovement = gameObject.GetComponent<CameraMovement> ();
        uiManager = gameObject.GetComponent<UIManager> ();

        if (GameObject.FindGameObjectWithTag ("ItemDataBase") == null)
        {
            itemDataBase = Instantiate (itemDataBase , Vector3.zero , Quaternion.identity) as ItemDataParser;
        }
        else
        {
            itemDataBase = GameObject.FindGameObjectWithTag ("ItemDataBase").GetComponent<ItemDataParser> ();
        }

    }

    private void Start ()
    {
        fastGame = true;
        StartCoroutine (GaneSeting ());
    }

    IEnumerator GaneSeting ()
    {
        while (itemDataBase.PortionData.Count < 1 && itemDataBase.EquippableItemData.Count < 1)
        {
            yield return new WaitForSeconds (.1f);
        }

        yield return StartCoroutine (ItemShuffle ());

        NewGame ();
    }

    // 생성 시작
    void InitGenerator ()
    {
        // 맵 생성, 플레이어, 몬스터 배치
        tileMapManager.GeneratorMap (StageCount - 1);
        generator.EnemySpawn ();
        cameraMovement.enabled = true;
    }

    // 다음 턴
    public void NextTurn ()
    {
        TurnCount++;

        if (tileMapManager.currentEnemys.Count == 0)
        {
            NextStage ();
        }

        ActionOnTurn?.Invoke ();
    }

    void NextStage ()
    {

        if (tileMapManager.currentItems.Count >= 1)
        {
            foreach (var item in FindObjectsOfType<ItemObject> ())
            {
                item.GetItemPlayer ();
            }
        }

        generator.NewStage ();
        StageCount++;
        TurnCount = 0;

        if (tileMapManager.tileMaps.Length >= stageCount)
        {
            tileMapManager.GeneratorMap (stageCount);
        }

        generator.EnemySpawn ();
        cameraMovement.enabled = true;
    }

    // 게임 시작, 재시작할떄 호출
    public void NewGame ()
    {
        randomSeed = (int)Random.Range (1 , 1000);

        StartItemShuffle ();

        StageCount = 1;
        TurnCount = 0;

        if (ActionNewGame != null && !fastGame)
        {

            if (tileMapManager.currentEnemys.Count >= 1)
            {
                foreach (var enemey in FindObjectsOfType<Enemy> ())
                {
                    enemey.Die ();
                }
            }

            if (tileMapManager.currentItems.Count >= 1)
            {
                foreach (var item in FindObjectsOfType<ItemObject> ())
                {
                    item.RemoveItemObject ();
                }
            }

            ActionNewGame?.Invoke ();
        }
        else
        {
            fastGame = false;
        }

        InitGenerator ();
    }

    public void GameOver ()
    {
        uiManager.GameOver ();
    }

    #region GameManager ItemList

    // ItemDataParser에서 데이터를 받아오고, 아이콘을 이미지를 로드함.
    IEnumerator GetItemList ()
    {
        Sprite[] ItemIconSprite = Resources.LoadAll<Sprite> ("icons"); ;

        if (itemDataBase.EquippableItemData?.Count > 0 || !isItemDataGet)
        {
            for (int i = 0 ; i < itemDataBase.EquippableItemData.Count ; i++)
            {
                string iconNumber = Regex.Replace (itemDataBase.EquippableItemData[i].Icon , @"\D" , "");
                itemIconDictionaty.Add (itemDataBase.EquippableItemData[i].Icon , ItemIconSprite[int.Parse (iconNumber)]);

                switch (itemDataBase.EquippableItemData[i].Rank)
                {
                    case ItemRank.Common:
                        CommonItemList.Add (itemDataBase.EquippableItemData[i]);
                        break;
                    case ItemRank.Uncommon:
                        UncommonItemList.Add (itemDataBase.EquippableItemData[i]);
                        break;
                    case ItemRank.Rare:
                        RareItemList.Add (itemDataBase.EquippableItemData[i]);
                        break;
                    case ItemRank.Epic:
                        EpicItemList.Add (itemDataBase.EquippableItemData[i]);
                        break;
                    default:
                        break;
                }
            }

            PortionList = itemDataBase.PortionData;

            for (int i = 0 ; i < PortionList.Count ; i++)
            {
                string iconNumber = Regex.Replace (PortionList[i].Icon , @"\D" , "");
                itemIconDictionaty.Add (PortionList[i].Icon , ItemIconSprite[int.Parse (iconNumber)]);
            }

            isItemDataGet = true;
        }

        yield return null;
    }

    // 등급별로 아이템의 등장 순서를 셔플 한다.
    IEnumerator ShuffleItemArray ()
    {
        // 아이템 데이터가 있을 때만.
        if (isItemDataGet)
        {
            CurrentPortionList = new List<Portion> (Utility.ShuffleArray (PortionList.ToArray () , randomSeed));
            CurrentCommonItemList = new Queue<EquippableItem> (Utility.ShuffleArray (CommonItemList.ToArray () , randomSeed));
            CurrentUncommonItemList = new Queue<EquippableItem> (Utility.ShuffleArray (UncommonItemList.ToArray () , randomSeed));
            CurrentRareItemList = new Queue<EquippableItem> (Utility.ShuffleArray (RareItemList.ToArray () , randomSeed));
            CurrentEpicItemList = new Queue<EquippableItem> (Utility.ShuffleArray (EpicItemList.ToArray () , randomSeed));
        }

        yield return null;
    }

    public IEnumerator ItemShuffle ()
    {
        if (!isItemDataGet)
        {
            if (itemDataBase.EquippableItemData.Count > 0)
            {
                yield return StartCoroutine (GetItemList ());
            }
        }

        yield return StartCoroutine (ShuffleItemArray ());

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory> ();

        }
    }

    public Item GetRandomEquippableItem ()
    {
        Item item;

        // 포션 or 장비 아이템 랜덤
        float random = Random.Range (0.0f , 1.0f);

        Debug.Log (string.Format ("포션 랜덤" + random));

        item = CurrentPortionList[Mathf.RoundToInt (Mathf.Clamp ((random * CurrentPortionList.Count - 1) , 0 , CurrentPortionList.Count - 1))];

        if (random > portionProbability)
        {
            return item;
        }

        // 장비 아이템 레어도 랜덤
        random = Random.Range (0.0f , 1.0f);

        Debug.Log (string.Format ("장비 랜덤" + random));

        if (random < epicProbability)
        {
            if (CurrentEpicItemList.Count > 0)
            {
                item = CurrentEpicItemList.Dequeue ();
            }
        }
        else if (random < rareProbability)
        {
            if (CurrentRareItemList.Count > 0)
            {
                item = CurrentRareItemList.Dequeue ();
            }
        }
        else if (random < uncommonProbability)
        {
            if (CurrentUncommonItemList.Count > 0)
            {
                item = CurrentUncommonItemList.Dequeue ();
            }
        }
        else
        {
            if (CurrentCommonItemList.Count > 0)
            {
                item = CurrentCommonItemList.Dequeue ();
            }
        }

        return item;
    }

    
    void StartItemShuffle ()
    {
        StartCoroutine (ItemShuffle ());
    }

    #endregion


}

