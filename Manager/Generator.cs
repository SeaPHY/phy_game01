using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스테이지 진행 정도, 턴의 진행 정도에 따라서 생성하면 좋을거 같다.
// 스테이지 클래스를 만들어서 스테이지 지날떄 마다 해당 스테이지 정보를 읽어서 적용해도 괜찮을꺼 같음.

public class Generator : MonoBehaviour
{

    TileMapManager tileMapManager;
    public EnemyData[] enemys;
    public ItemObject itemPrefab;
    public int[] generateTurn;
    public int maximumItem;
    int itemIndex;

    private void Start ()
    {
        tileMapManager = GameManager.Instance.tileMapManager;
        GameManager.Instance.ActionOnTurn += ItemSpawn;
        itemIndex = 0;
    }

    public void NewStage ()
    {
        itemIndex = 0;

    }

    // 아이템은 특정한 턴에 생성 된다.
    public void ItemSpawn ()
    {
        if (tileMapManager.currentItems.Count >= maximumItem)
        {
            return;
        }

        if (itemIndex < generateTurn.Length && GameManager.Instance.TurnCount >= generateTurn[itemIndex])
        {
            Vector3 spawnPosition = tileMapManager.GetRandomNomalTilePosition ();
            Coordinates spawnCoordinates = tileMapManager.GetCoordFromPosition (spawnPosition);

            ItemObject item = Instantiate (itemPrefab , spawnPosition + Vector3.up , Quaternion.Euler (90 , 0 , 0));
            item.coordinates = spawnCoordinates;
            tileMapManager.currentItems.Add (spawnCoordinates , item);
            tileMapManager.currentObject.Add (spawnCoordinates , TileMapManager.ObjectType.item);
            itemIndex++;
        }
    }

    // 에너미를 생성하고, TileMapManager.Instance.currentEnemys 리스트에 좌표와 트랜스폼 값을 추가 합니다.
    public void EnemySpawn ()
    {
        for (int i = 0 ; i < enemys.Length ; i++)
        {
            for (int count = 0 ; count < enemys[i].enemyCount ; count++)
            {

                Coordinates spawnCoordinates = tileMapManager.GetRandomNomalCoordinates ();
                Vector3 spawnPosition = tileMapManager.CoordinatesToPostion (spawnCoordinates);
                Enemy enemy = Instantiate (enemys[i].prefeb , spawnPosition , Quaternion.identity);
                enemy.GeneratorSetUp (enemys[i] , spawnCoordinates);
                tileMapManager.currentEnemys.Add (spawnCoordinates , enemy);
                tileMapManager.currentObject.Add (spawnCoordinates , TileMapManager.ObjectType.enemy);
            }
        }
    }
}
