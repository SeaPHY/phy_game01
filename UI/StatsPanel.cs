using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] StatDisplay[] statDisplay;
    public string[] statNames;

    private Stat[] stats;

    private void Awake ()
    {
        statDisplay = GetComponentsInChildren<StatDisplay> ();
    }

    private void Start ()
    {
        UpdateStatNames ();
        UpdateStatValues ();
    }

    // params 함수의 인수로 배열을 사용하기 위해 앞에 선언함.
    public void SetStats (params Stat[] characterStat)
    {
        stats = characterStat;

        if (stats.Length > statDisplay.Length)
        {
            // 스텟 디스플레이가 부족함
            Debug.LogError ("Not Enough Stat Displays!");
            return;
        }

        // 캐릭터 스텟의 숫자 만큼 statDisplay를 활성화 함.
        for (int i = 0 ; i < statDisplay.Length ; i++)
        {
            statDisplay[i].gameObject.SetActive (i < statDisplay.Length);

            if (i < stats.Length)
            {
                statDisplay[i].Stat = stats[i];
            }
        }
    }

    public void UpdateStatValues ()
    {
        for (int i = 0 ; i < stats.Length ; i++)
        {
            statDisplay[i].UpdateStatValue ();
        }
    }

    public void UpdateStatNames ()
    {
        for (int i = 0 ; i < stats.Length ; i++)
        {
            statDisplay[i].Name = statNames[i];
        }
    }
}
