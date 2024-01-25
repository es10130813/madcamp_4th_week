using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    private Animator animator;
    private bool isPlayerInRange = false;
    private Vector3 checkpointPosition; // 체크포인트의 위치를 저장하는 변수
    public HealthManager healthManager; // HealthManager 참조 추가

    void Start()
    { 
        animator = GetComponent<Animator>();
        checkpointPosition = transform.position; // 체크포인트의 초기 위치를 설정
    }

    void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetBool("CheckPointState", true);
                SaveCheckpointPosition(); // 위치 저장 함수 호출
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void SaveCheckpointPosition()
    {
        PlayerPrefs.SetFloat("CheckpointX", checkpointPosition.x);
        PlayerPrefs.SetFloat("CheckpointY", checkpointPosition.y);
        PlayerPrefs.Save();
        healthManager.SetHealth(3);
    }
}
