using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 좌표 x(가로), y(세로)로 구성
/// </summary>
[System.Serializable]
public class Coordinates
{
    public int x;
    public int y;

    public Coordinates (int _x , int _y)
    {
        x = _x;
        y = _y;
    }

    public Coordinates (Coordinates coordinates)
    {
        x = coordinates.x;
        y = coordinates.y;
    }

    public Coordinates Left ()
    {
        return new Coordinates (x - 1 , y);
    }

    public Coordinates Right ()
    {
        return new Coordinates (x + 1 , y);
    }

    public Coordinates Down ()
    {
        return new Coordinates (x , y - 1);
    }

    public Coordinates Up ()
    {
        return new Coordinates (x , y + 1);
    }

    public Coordinates Zero ()
    {
        return new Coordinates (0 , 0);
    }

    public static bool operator == (Coordinates c1, Coordinates c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator != (Coordinates c1 , Coordinates c2)
    {
        return !(c1 == c2);
    }

    // 해쉬코드가 같은 두 객체가 같은 자료형인지를 확인 합니다.
    public override bool Equals (object c1)
    {
        if (!(c1 is Coordinates))
            return false;

        return Equals ((Coordinates)c1);
    }

    // 해쉬코드가 같은 두 객체의 값을 비교 합니다.
    public bool Equals (Coordinates other)
    {
        if (x != other.x)
            return false;

        return y == other.y;
    }

    // 해시코드는 서로 다른 두 객체를 간단하게 비교하기 위한 용도 입니다.
    // 해시코드가 같더라도 두 객체가 동일하다고 의미 하는건 아닙니다, 다만 동일한 객체의 해쉬코드는 같아야 합니다.
    // 해시코드가 같다 -> Equals
    public override int GetHashCode ()
    {
        return x ^ y;
    }
}