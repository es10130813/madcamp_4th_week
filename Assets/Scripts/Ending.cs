using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 추가

public class ToggleObjectActive : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public GameObject objectToToggle; // 토글할 오브젝트
    public float interactionDistance = 3.0f; // 상호작용 가능한 거리
    public Animator animator; // Animator 컴포넌트가 있는 오브젝트
    public string parameterName = "CheckPointState"; // 변경할 애니메이터 파라미터 이름

    
    void Update()
    {
        // 플레이어와 이 오브젝트 사이의 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 거리가 interactionDistance 이내이고 'F' 키가 눌렸을 때
        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(ToggleObjectAfterDelay());
        }
    }

    IEnumerator ToggleObjectAfterDelay()
    {
        // 애니메이터 파라미터 설정
        animator.SetBool(parameterName, true);

        // 1초 동안 기다림
        yield return new WaitForSeconds(1f);

        // 오브젝트의 활성화 상태를 토글
        objectToToggle.SetActive(!objectToToggle.activeSelf);
    }
}