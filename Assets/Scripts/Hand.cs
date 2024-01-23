using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;

    [SerializeField] public float swingDuration = 0.15f; // Duration of the swing motion

    SpriteRenderer player;
    private bool isSwinging = false; // 코루틴 실행 상태 추적을 위한 플래그

    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;
        if (isLeft)
        {
            spriter.sortingOrder = isReverse ? 12 : 9;
        }
        else
        {
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 9 : 12;
        }

        // 오른손이고, 클릭했으며, 현재 스윙 중이 아닐 때 코루틴 시작
        if (!isLeft && Input.GetMouseButtonDown(0) && !isSwinging)
        {
            StartCoroutine(SwingHand(isReverse));
        }
    }

    IEnumerator SwingHand(bool isReverse)
{
    isSwinging = true;
    float time = 0;
    Vector3 originalPosition = spriter.transform.localPosition;

    // X축 방향으로 이동할 거리를 설정
    float xOffset = 0.1f;

    float startAngle, endAngle, radius;
    if (isReverse)
    {
        startAngle = 60f;
        endAngle = 270f;
        radius = 0.2f;
    }
    else
    {
        startAngle = 450f;
        endAngle = 270f;
        radius = 0.2f;
    }

    while (time < swingDuration)
    {
        time += Time.deltaTime;
        float angle = Mathf.Lerp(startAngle, endAngle, time / swingDuration) * Mathf.Deg2Rad;

        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        spriter.transform.localPosition = originalPosition + new Vector3(x, y, 0);
        yield return null;
    }

    spriter.transform.localPosition = originalPosition;
    isSwinging = false;
}

}
