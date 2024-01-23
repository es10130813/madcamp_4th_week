using UnityEngine;
using System.Collections; 

public class DropingTile : MonoBehaviour
{
    public float disappearDelay = 2.0f; // 타일이 사라지기까지의 지연 시간
    public float reappearDelay = 5.0f; // 타일이 다시 나타나기까지의 지연 시간

    private Transform playerTransform;
    private bool isPlayerOnTile = false;

    // Start는 첫 번째 프레임 업데이트 전에 호출됩니다
    void Start()
    {
        // 초기 설정이 필요한 경우 여기에 추가
    }

    // OnTriggerEnter2D는 Collider2D가 트리거와 충돌할 때 호출됩니다
    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.gameObject.CompareTag("Player"))
    {
        playerTransform = other.transform;
        playerTransform.SetParent(transform);
        isPlayerOnTile = true;
        StartCoroutine(DisappearAndReappear());
    }
}

private void OnTriggerExit2D(Collider2D other)
{
    if (other.gameObject.CompareTag("Player"))
    {
        // 타일이 활성화 상태일 때만 부모 설정 변경
        if (gameObject.activeInHierarchy)
        {
            playerTransform.SetParent(null);
        }
        isPlayerOnTile = false;
    }
}

IEnumerator DisappearAndReappear()
{
    // 지정된 지연 시간 후에 타일을 비활성화
    yield return new WaitForSeconds(disappearDelay);

    // 타일이 비활성화되기 전에 플레이어가 타일 위에 있는지 확인
    if (isPlayerOnTile && playerTransform != null)
    {
        Rigidbody2D playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
        if (playerRigidbody != null)
        {
            playerRigidbody.gravityScale = 4f; // 중력 적용
            playerTransform.SetParent(null);
        }
    }

    gameObject.SetActive(false);

    // 추가적인 지연 시간 후에 타일을 다시 활성화
    yield return new WaitForSeconds(reappearDelay);
    gameObject.SetActive(true);

    // 타일이 다시 활성화되면 플레이어의 중력을 다시 0으로 설정
    if (playerTransform != null)
    {
        Rigidbody2D playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
        if (playerRigidbody != null)
        {
            playerRigidbody.gravityScale = 0f; // 중력 해제
        }
    }
}


}
