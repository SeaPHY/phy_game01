using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[System.Serializable]
public class Stat
{
    public float BaseValue;

    public virtual float Value
    {
        get
        {
            if (isDirty || BaseValue != lastBaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue ();
                isDirty = false;
            }
            return _value;
        }
    }

    protected bool isDirty = true;    // CalculateFinalValue 계산 할지 체크
    protected float _value;
    protected float lastBaseValue = float.MinValue;   // BaseValue 변동 되었는지 판단하기 위한 변수.

    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public Stat ()
    {
        statModifiers = new List<StatModifier> ();
        StatModifiers = statModifiers.AsReadOnly ();
    }

    public Stat (float baseValue) : this ()
    {
        BaseValue = baseValue;

    }

    public virtual void AddModifier (StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add (mod);
        statModifiers.Sort (CompareModifierOrder);  // 리스트 정렬
    }

    // 리스트 Sort에 필요함
    protected virtual int CompareModifierOrder (StatModifier a , StatModifier b)
    {
        if (a.Order < b.Order)
        {
            return -1;
        }
        else if (a.Order > b.Order)
        {
            return 1;
        }
        return 0;   // if(a.Order == b.Order)
    }

    public virtual bool RemoveModifier (StatModifier mod)
    {
        if (statModifiers.Remove (mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiersFromSource (object source)
    {
        bool didRemove = false;

        // 리스트는 삭제되면 자동으로 당겨지기 떄문에, 뒤에서 부터 역으로 검출 한다.
        for (int i = statModifiers.Count - 1 ; i >= 0 ; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt (i);
            }
        }
        return didRemove;
    }

    // 기본값 + statModifiers의 총합
    protected virtual float CalculateFinalValue ()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0;

        for (int i = 0 ; i < statModifiers.Count ; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.Value;

                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.Type == StatModType.PercentMult)
            {
                // 0.1일 경우 -> 1.1이 되어서 finalValue값은 110%가 됨.
                finalValue *= 1 + mod.Value;
            }
        }

        // 12.0001f != 12f Math.Round - 4자릿수 반올림
        return (float)System.Math.Round (finalValue , 4);
    }
}

