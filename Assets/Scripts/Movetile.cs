using UnityEngine;

public class MovingTile : MonoBehaviour
{
    public Transform desPos;
    public Transform startPos;
    public Transform endPos;
    public float speed;

    private Transform playerTransform;
    private bool isPlayerOnTile = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPos.position;
        desPos = endPos;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 현재 위치에서 목표 위치로의 방향 벡터 계산
        Vector3 direction = (desPos.position - transform.position).normalized;

        // 현재 위치에서 목표 위치로 이동
        transform.position += direction * Time.deltaTime * speed;

        // 목표 위치에 도달했을 경우 목표 위치 변경
        if (Vector3.Distance(transform.position, desPos.position) <= 0.05f)
        {
            desPos = desPos == endPos ? startPos : endPos;
        }

        // 플레이어와 움직이는 타일 간의 겹치는 영역을 검출
        //CheckPlayerOnTile();
    }

    // void CheckPlayerOnTile()
    // {
    //     if (playerTransform == null)
    //     {
    //         // "Player" 태그를 가진 오브젝트를 찾아서 플레이어 Transform을 가져옵니다.
    //         GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
    //         if (playerObject != null)
    //         {
    //             playerTransform = playerObject.transform;
    //         }
    //     }

    //     if (playerTransform != null)
    //     {
    //         Collider2D playerCollider = playerTransform.GetComponent<Collider2D>();
    //         Collider2D tileCollider = GetComponent<Collider2D>();

    //         // 플레이어가 타일 위에 있을 경우, 플레이어를 타일의 자식으로 설정
    //         if (tileCollider.bounds.Intersects(playerCollider.bounds))
    //         {
    //             playerTransform.SetParent(transform);
    //             isPlayerOnTile = true;
    //         }
    //         else if (isPlayerOnTile)
    //         {
    //             // 플레이어가 타일에서 내려왔을 경우, 부모-자식 관계 해제
    //             playerTransform.SetParent(null);
    //             isPlayerOnTile = false;
    //         }
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerTransform.SetParent(transform);
            isPlayerOnTile = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform.SetParent(null);
            isPlayerOnTile = false;
        }
    }



}
