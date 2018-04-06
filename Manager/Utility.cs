using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    // The Fisher-Yates Shuffle 방식 셔플
    // T, <T> - 제네릭(Generic) 형
    public static T[] ShuffleArray<T> (T[] array , int seed)
    {
        // prng 뜻 - 유사난수 생성기(pseudorandom number generator, PRNG)
        // System.Random은 랜덤 처럼 보이도록 수식을 만든 것.
        // 시드값이 같으면 같은 값이 나타난다.
        System.Random prng = new System.Random (seed);

        // The Fisher-Yates Shuffle 방식은 마지막 루프는 생략해도 됨.
        for (int i = 0 ; i < array.Length - 1 ; i++)
        {
            int randomIndex = prng.Next (i , array.Length);   // 지정한 범위 내의 무작위의 정수를 반환.

            // i와 randomIndex의 값을 변경함
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }
        return array;
    }

}
