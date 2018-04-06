using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지가 바뀔때 마다 맵 크기를 확인해서 활성화, 비활성화 됨
/// </summary>
public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    Vector3 offset = new Vector3 (0f , 4.5f , -3f);
    Camera mainCamera;

    //float zMaxValue = 0;
    //float zMinValue = 0;
    //float xMaxValue = 0;
    //float xMinValue = 0;
    float tileSize;
    TileMapManager tileMapManager;

    Vector3 targetPosition;
    Vector3 smoothedPosition;
    Vector3 desiredPosition;

    private void OnEnable ()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = offset;
        tileMapManager = GameManager.Instance.tileMapManager;
        tileSize = tileMapManager.tileSize;
        mainCamera.orthographicSize = 6 * tileSize;
        //GetMinMax ();
    }

    private void LateUpdate ()
    {
        /*
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        targetPosition = target.position;
        targetPosition.x = Mathf.Clamp (target.position.x , xMinValue , xMaxValue);
        targetPosition.z = Mathf.Clamp (target.position.z , zMinValue , zMaxValue);
        desiredPosition = targetPosition + offset;
        if (mainCamera.transform.position != desiredPosition)
        {
            smoothedPosition = Vector3.Lerp (mainCamera.transform.position , desiredPosition , smoothSpeed);
            mainCamera.transform.position = smoothedPosition;
        }
        */

        targetPosition = target.position;
        targetPosition.y = 4.5f;
        smoothedPosition = Vector3.Lerp (mainCamera.transform.position , targetPosition , smoothSpeed);
        mainCamera.transform.position = smoothedPosition;
    }

    /*
    public void GetMinMax ()
    {
        Vector3 minVector = tileMapManager.CoordinatesToPostion (0 , 0);
        Vector3 maxVector = tileMapManager.CoordinatesToPostion (tileMapManager.currentMap.mapSize);
        zMaxValue = Mathf.Max(maxVector.z - mainCamera.orthographicSize - tileSize * 0.8f, 0);
        zMinValue = Mathf.Min(minVector.z + mainCamera.orthographicSize + tileSize * 0.8f, 0);
        xMaxValue = Mathf.Max(maxVector.x - (int)(mainCamera.orthographicSize * 1.5f) - tileSize, 0);
        xMinValue = Mathf.Min(minVector.x + (int)(mainCamera.orthographicSize * 1.5f), 0);

        if (zMaxValue + xMaxValue == 0 && zMinValue + xMinValue == 0)
        {
            gameObject.GetComponent<CameraMovement> ().enabled = false;
        }
    }

    */

}
