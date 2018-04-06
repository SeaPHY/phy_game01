using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 타일 맵 생성.
// 생성한 타일맵의 정보를  Transform[,]로 가지고 있음.
// 타일맵은 왼쪽 하단 0,0 좌표부터 시작함.

public class TileMapManager : MonoBehaviour
{

    public enum ObjectType
    {
        obstacle, enemy, item
    };

    public Map[] tileMaps;
    public Map lastMap;     // tileMaps 범위를 벗어나면 만들어질 최종 스테이지.

    [Header ("Tile Attribute")]
    public Transform tilePrefab;
    public float tileSize;
    [Range (0 , 1)]
    public float outlinePercent;

    [Header ("Current Map")]
    public int currentMapIndex;
    public Map currentMap;
    public Transform[,] currentTileTransform;
    //List<Coordinates> currentObject;
    List<Coordinates> currentCoordinatesList;
    Queue<Coordinates> shuffledCoordinates;

    public Coordinates playerCoordinates;
    public Dictionary<Coordinates , ObjectType> currentObject;
    public Dictionary<Coordinates , Enemy> currentEnemys;
    public Dictionary<Coordinates , ItemObject> currentItems;

    Transform mapHolder;
    string holderName = "Generated Map";

    public void GeneratorMap (int index)
    {
        currentMapIndex = index;
        GeneratorMap ();
    }

    // 맵 생성 부분.
    public void GeneratorMap ()
    {
        // 필요한 리스트들을 선언 합니다.
        if (currentMapIndex < tileMaps.Length)
        {
            currentMap = tileMaps[currentMapIndex];
        }
        else
        {
            currentMap = lastMap;
        }

        // 매번 초기화를 하지 않는다.
        if (currentTileTransform != null)
        {
            System.Array.Clear (currentTileTransform , 0 , currentTileTransform.Length);
        }
        else
        {
            currentObject = new Dictionary<Coordinates , ObjectType> ();
            currentEnemys = new Dictionary<Coordinates , Enemy> ();
            currentItems = new Dictionary<Coordinates , ItemObject> ();
        }
        
        currentCoordinatesList?.Clear ();

        currentTileTransform = new Transform[currentMap.mapSize.x , currentMap.mapSize.y];
        currentCoordinatesList = new List<Coordinates> ();
        

        for (int x = 0 ; x < currentMap.mapSize.x ; x++)
        {
            for (int y = 0 ; y < currentMap.mapSize.y ; y++)
            {
                currentCoordinatesList.Add (new Coordinates (x , y));
            }
        }

        shuffledCoordinates = new Queue<Coordinates> (Utility.ShuffleArray (currentCoordinatesList.ToArray () , GameManager.Instance.randomSeed));

        // 맵 홀더 생성
        if (mapHolder != null)
        {
            Destroy (mapHolder.gameObject);
            mapHolder = new GameObject (holderName).transform;
            mapHolder.parent = transform;
        }
        else
        {
            mapHolder = new GameObject (holderName).transform;
            mapHolder.parent = transform;
        }

        // 타일맵 생성
        for (int x = 0 ; x < currentMap.mapSize.x ; x++)
        {
            for (int y = 0 ; y < currentMap.mapSize.y ; y++)
            {
                Vector3 tilePostion = CoordinatesToPostion (x , y);
                Transform newTile = Instantiate (tilePrefab , tilePostion , Quaternion.Euler (Vector3.right * 90) , mapHolder) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;  // outlinePercent 만큼 타일의 크기를 줄여줌
                currentTileTransform[x , y] = newTile;   // tileMap에 타일맵 저장.
            }
        }

        playerCoordinates = currentMap.MapCenter;
    }

    // 타일맵
    // 필요한것 - 좌표, 타일의 속성 - 이동가능 여부
    [System.Serializable]
    public class Map
    {
        public Coordinates mapSize;

        public Coordinates MapCenter
        {
            get { return new Coordinates ((mapSize.x - 1) / 2 , (mapSize.y - 1) / 2); }
        }
    }

    // 받아온 Vector3을 tileMap으로 변환.
    public Transform GetTileFromPosition (Vector3 posititon)
    {
        // Vector3을 변환 해줌.
        // x 구하는 식. CoordToPostion의 역으로 계산하면 됨.
        // posititon = (-posititon.x / 2f + 0.5f + x) * tileSize
        // posititon/tileSize = -mapSize.x + 1 / 2f + x
        // posititon/tileSize + (mapSize.x + 1) / 2 = x 
        // Mathf.RoundToInt 반올림 해서 형변환 해줌.
        int x = Mathf.RoundToInt (posititon.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt (posititon.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

        if (x > currentTileTransform.GetLength (0) || y > currentTileTransform.GetLength (1) || x < 0 || y < 0)
        {
            return null;
        }

        // 타일맵 범위 안의 값을 가지도록 범위 제한
        x = Mathf.Clamp (x , 0 , currentTileTransform.GetLength (0) - 1);
        y = Mathf.Clamp (y , 0 , currentTileTransform.GetLength (1) - 1);

        return currentTileTransform[x , y];
    }

    public Coordinates GetRandomCoordinates ()
    {
        Coordinates randomCoordinates = shuffledCoordinates.Dequeue ();
        shuffledCoordinates.Enqueue (randomCoordinates);

        return randomCoordinates;
    }

    public Coordinates GetRandomNomalCoordinates ()
    {
        Coordinates randomCoordinates;

        do
        {
            randomCoordinates = shuffledCoordinates.Dequeue ();
            shuffledCoordinates.Enqueue (randomCoordinates);

        } while (currentObject.ContainsKey (randomCoordinates) || randomCoordinates == playerCoordinates);

        return randomCoordinates;
    }

    public Transform GetRandomNomalTileTransform ()
    {
        Coordinates randomCoordinates = GetRandomCoordinates ();
        return currentTileTransform[randomCoordinates.x , randomCoordinates.y];
    }

    public Vector3 GetRandomTilePosition ()
    {
        Coordinates randomCoordinates = GetRandomCoordinates ();
        return CoordinatesToPostion (randomCoordinates.x , randomCoordinates.y);
    }


    public Vector3 GetRandomNomalTilePosition ()
    {
        Coordinates randomCoordinates = GetRandomNomalCoordinates ();

        return CoordinatesToPostion (randomCoordinates.x , randomCoordinates.y);
    }

    public Coordinates GetCoordFromPosition (Vector3 posititon)
    {
        // Vector3을 변환 해줌.
        // x 구하는 식. CoordToPostion의 역으로 계산하면 됨.
        // posititon = (-posititon.x / 2f + 0.5f + x) * tileSize
        // posititon/tileSize = -mapSize.x + 1 / 2f + x
        // posititon/tileSize + (mapSize.x + 1) / 2 = x 
        // Mathf.RoundToInt 반올림 해서 형변환 해줌.
        int x = Mathf.RoundToInt (posititon.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt (posititon.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

        if (x > currentTileTransform.GetLength (0) || y > currentTileTransform.GetLength (1) || x < 0 || y < 0)
        {
            return null;
        }

        // 타일맵 범위 안의 값을 가지도록 범위 제한
        x = Mathf.Clamp (x , 0 , currentTileTransform.GetLength (0) - 1);
        y = Mathf.Clamp (y , 0 , currentTileTransform.GetLength (1) - 1);

        return new Coordinates (x , y);
    }

    // Coordinates 를 실제 좌표 값으로 변환.
    public Vector3 CoordinatesToPostion (int x , int y)
    {
        return new Vector3 (-currentMap.mapSize.x / 2f + 0.5f + x , 0 , -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Vector3 CoordinatesToPostion (Coordinates coordinates)
    {
        return new Vector3 (-currentMap.mapSize.x / 2f + 0.5f + coordinates.x , 0 , -currentMap.mapSize.y / 2f + 0.5f + coordinates.y) * tileSize;
    }

    // 맵 중앙 포지션.
    public Vector3 MapCenterPostion ()
    {
        return CoordinatesToPostion (currentMap.MapCenter.x , currentMap.MapCenter.y);
    }

    // 범위 체크.
    public List<Coordinates> RangeCheck (Coordinates nowCoordinates , int range = 1 , bool obstacleCheck = false)
    {
        List<Coordinates> rangeCoordinates;
        rangeCoordinates = new List<Coordinates> ();

        // 맵 범위 안의 좌표를 받아옴
        List<Coordinates> isCoordinates = CrossRange (nowCoordinates , range);

        if (isCoordinates.Count == 0)
        {
            return null;
        }
        for (int i = 0 ; i < isCoordinates.Count ; i++)
        {
            rangeCoordinates.Add (isCoordinates[i]);
        }
        return rangeCoordinates;
    }

    public bool RangeCheck (Coordinates standardCoordinates , Coordinates checkCoordinates , int range)
    {
        if (range >= Mathf.Abs (standardCoordinates.x - checkCoordinates.x) + Mathf.Abs (standardCoordinates.y - checkCoordinates.y))
        {
            return true;
        }
        return false;
    }

    // 입력받은 좌표에 플레이어가 있는지 확인.
    public bool PlayerCheck (Coordinates checkCoordinates)
    {
        if (checkCoordinates.x == playerCoordinates.x && checkCoordinates.y == playerCoordinates.y)
        {
            return true;
        }

        return false;
    }

    // 입력받은 좌표리스트에 플레이어가 있는지 확인.
    public bool PlayerCheck (List<Coordinates> checkCoordinates)
    {
        for (int i = 0 ; i < checkCoordinates.Count ; i++)
        {
            if (checkCoordinates[i].x == playerCoordinates.x && checkCoordinates[i].y == playerCoordinates.y)
            {
                return true;
            }
        }
        return false;
    }

    // 입력받은 centerCoord 기준으로 십자 모양으로 range 만큼의 타일들을 구해서 리스트로 반환 합니다.
    public List<Coordinates> CrossRange (Coordinates centerCoordinates , int range)
    {
        Coordinates mapSize = GameManager.Instance.tileMapManager.currentMap.mapSize;
        // centerCoord가 존재하는 좌표인지 체크
        if (!TileCheck (centerCoordinates))
        {
            return null;
        }

        List<Coordinates> rangeCoordinates;
        rangeCoordinates = new List<Coordinates> ();

        for (int i = -1 * range ; i <= range ; i++)
        {
            for (int j = -1 * range ; j <= range ; j++)
            {
                if (Mathf.Abs (i) != Mathf.Abs (j) && (centerCoordinates.x + i) >= 0 && (centerCoordinates.x + i) < mapSize.x && (centerCoordinates.y + j) >= 0 && (centerCoordinates.y + j) < mapSize.y)
                {
                    rangeCoordinates.Add (new Coordinates (centerCoordinates.x + i , centerCoordinates.y + j));
                }
            }
        }

        return rangeCoordinates.Count == 0 ? null : rangeCoordinates;
    }

    // 현재 맵사이즈와 입력된 coord를 비교합니다.
    public bool TileCheck (Coordinates coordinates)
    {
        return (coordinates.x >= 0 && coordinates.x < currentMap.mapSize.x && coordinates.y >= 0 && coordinates.y < currentMap.mapSize.y) ? true : false;
    }

    public void OvjectList ()
    {
        //currentObject[0] = new Coordinates()
    }

}
