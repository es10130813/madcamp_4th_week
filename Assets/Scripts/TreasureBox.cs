using UnityEngine;

public class InteractionScript : MonoBehaviour
{
    public GameObject alertPanel; // 알림창에 해당하는 UI 오브젝트

    private bool isPlayerNear = false; // 플레이어가 오브젝트 근처에 있는지 여부
    private int hasToggled = 0; // 알림창을 이미 한 번 토글했는지 여부

    void Update()
    {
        // 플레이어가 오브젝트 근처에 있고 F 키를 누르며 알림창을 아직 토글하지 않았다면
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F) && hasToggled<2)
        {
            // 알림창의 활성화 상태를 토글하고 토글 플래그를 설정
            alertPanel.SetActive(!alertPanel.activeSelf);
            hasToggled ++;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어 태그를 사용
        {
            isPlayerNear = true;
        }
    }
}
