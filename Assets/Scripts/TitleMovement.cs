using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMovement : MonoBehaviour
{
    public Transform titleTransform;
    public Animator monsterAnimator;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float moveDuration = 0.1f;
    private float moveTime = 0.0f;
    private bool isMoving = false;
    private float waitTime = 0.5f; // 대기 시간 추가
    private bool isWaiting = false; // 대기 중인지 확인하는 플래그

    void Start()
    {
        titleTransform.position = startPosition;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = monsterAnimator.GetCurrentAnimatorStateInfo(0);

        // 몬스터 애니메이션이 끝났고, 아직 이동 또는 대기를 시작하지 않았을 때
        if (stateInfo.IsName("opening_attack") && stateInfo.normalizedTime >= 1.0f && !isMoving && !isWaiting)
        {
            isWaiting = true; // 대기 시작
        }

        // 대기 중일 때
        if (isWaiting)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                isWaiting = false;
                isMoving = true; // 대기 후 이동 시작
            }
        }

        // 이동 로직
        if (isMoving)
        {
            moveTime += Time.deltaTime;
            titleTransform.position = Vector3.Lerp(startPosition, endPosition, moveTime / moveDuration);

            if (moveTime >= moveDuration)
            {
                isMoving = false;
                titleTransform.position = endPosition;
            }
        }
    }
}
