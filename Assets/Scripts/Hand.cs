using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;

    [SerializeField] public float swingDuration = 0.15f; // Duration of the swing motion

    SpriteRenderer player;

    private Player shadow;

    private bool isSwinging = false; // 코루틴 실행 상태 추적을 위한 플래그

    void Awake()
    {
        player = GetComponentsInParent<SpriteRenderer>()[1];
        shadow = FindObjectOfType<Player>();
    }

    void LateUpdate()
    {
        if (shadow != null && shadow.isFalling)
        {
            StartCoroutine(ApplyLayerOrder());
        }
        else{
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
    }


    IEnumerator ApplyLayerOrder()
    { // 중력 적용
        yield return new WaitForSeconds(0.175f); // 0.175초 기다림
        spriter.sortingOrder = 1; // Layer Order 업데이트
    }

    IEnumerator SwingHand(bool isReverse)
{
    isSwinging = true;
    float time = 0;
    Vector3 originalPosition = spriter.transform.localPosition;
    Vector3 originalRotation = spriter.transform.localEulerAngles;

    Vector3 customInitialRotation; // 예시로 30도 설정

    float startAngle, endAngle, radius, rotationSpeed;
    if (isReverse)
    {
        startAngle = 60f;
        endAngle = 270f;
        radius = 0.2f;
        rotationSpeed = 2.0f;
        customInitialRotation = new Vector3(0, 0, 270);
    }
    else
    {
        startAngle = 450f;
        endAngle = 270f;
        radius = 0.2f;
        rotationSpeed = -2.0f;
        customInitialRotation = new Vector3(0, 0, 450);
    }

    while (time < swingDuration)
    {
        time += Time.deltaTime;
        float angle = Mathf.Lerp(startAngle, endAngle, time / swingDuration) * Mathf.Deg2Rad;

        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        // 스프라이트의 위치 업데이트
        spriter.transform.localPosition = originalPosition + new Vector3(x, y, 0);

        // 스프라이트의 회전 업데이트
        float rotationAngle = rotationSpeed * 70f * (time / swingDuration); // 여기에서 회전 속도 적용
        spriter.transform.localEulerAngles = customInitialRotation + new Vector3(0, 0, rotationAngle);

        yield return null;
    }

    spriter.transform.localPosition = originalPosition;
    spriter.transform.localEulerAngles = originalRotation; // 스윙 종료 후 원래 회전 각도로 복귀
    isSwinging = false;
}



}