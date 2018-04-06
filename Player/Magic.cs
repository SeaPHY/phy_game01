using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu]
public class Magic : ScriptableObject
{
    [Header ("Base")]
    public string magicName;         // 마법 이름
    public string magicText;    // 마법 발동할때 입력될 텍스트.
    public int range;           // 사거리 0이면 자신에게 사용.
    public int damage;          // 마법 데미지
    public int castingCount;    // 필요한 캐스팅 횟수
    public int magicPointCost;  // 필요한 마나
    public int shieldPoint;     // 쉴드 량
    public bool targeting;
    public Sprite icon;   // 이미지
    public Type type = Type.shoot;     // 마법 유형, 기본은 shoot

    [Header ("Type Chain")]
    public int chainCount;      // 체인할 횟수
    public int chainRange;      // 체인 가능한 사거리

    [Header ("Type Impact")]      // 공격 범위는 폭발범위 + 스플래시 범위 / 폭발범위에는 그대로 데미지가 들어가고, 스플래시 범위엔는 스플래시데미지 퍼센트 만큼의 데미지가 들어감. 
    public int impactRange;       // 폭발 범위
    public int splashRange;       // 스플래시 범위
    [Range (0 , 1)]
    public float splashDamage;    // 스플래시 데미지

    [Header ("Type Random")]
    public int randomCount;     // 랜덤 발동 횟수

    public enum Type
    {
        shoot,  // 발사형 - 해당 방향으로 발사
        move,   // 이동마법 - 지정 좌표로 이동함.
        chain,  // 체인 - 발사후 근처 에너미에 연쇄 데미지 (체인라이트닝)
        impact, // 폭발형 - 목표 좌표 주변 범위 데미지
        random, // 랜덤 좌표. 지정한 카운터 만큼 발동함.
        recovery,
    }
}
