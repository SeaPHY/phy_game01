using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyData
{
    public Enemy prefeb;
    public int enemyCount;

    [Header ("에너미 스테이터스")]
    public int healt = 2;
    public int dameg = 1;
    public int attackRange = 1;
    public int turnCounter = 2; // 턴 카운터
    public bool firstAttack = false; // 선공 설정

    [Header ("도주 설정")]
    public bool escape = false;
    [Range (0 , 1)]
    public float escapeHp = 0.3f;

    [Header ("인식 거리 - 0은 무제한")]
    public int seeRange = 0;

}
