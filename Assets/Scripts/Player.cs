using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Tilemap> tilemaps; // Tilemap 컴포넌트 참조

    [SerializeField] private float knockbackDuration = 0.5f; // 넉백 지속 시간
    private bool isKnockedBack = false;

    [SerializeField] private float knockbackForce = 0.2f;

    private bool playerClicked = false;
    [SerializeField] private float clickDuration = 0.5f;

    public Rigidbody2D rb;
    public SpriteRenderer playerSpriteRenderer;
    public Transform child; // 자식 오브젝트 참조
    public float jumpHeight = 2f; // 점프 높이
    public float jumpDuration = 0.5f; // 점프 시간
    private bool isJumping = false; // 점프 상태 추적 변수
    public bool isFalling = false;
    private Vector3 startPosition;
    private Vector3 checkPointPosition;
    public float fallThreshold = -15f;
    private int originalPlayerSortingOrder; // 플레이어의 원래 Order in Layer 값
    private Dictionary<Transform, int> originalChildSortingOrders;
    private Animator childAnimator;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        rb.gravityScale = 0f; 
    rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 시작 시 중력 영향을 받지 않도록 설정
        startPosition = transform.position;
        checkPointPosition = startPosition;
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");

        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            originalPlayerSortingOrder = playerSpriteRenderer.sortingOrder;
        }

        // 자식 오브젝트의 원래 Order in Layer 값 저장
        originalChildSortingOrders = new Dictionary<Transform, int>();
        foreach (Transform child in transform)
        {
            SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
            if (childSpriteRenderer != null)
            {
                originalChildSortingOrders[child] = childSpriteRenderer.sortingOrder;
            }
        }
        if (child != null)
        {
            childAnimator = child.GetComponent<Animator>();
        }
        circleCollider = GetComponent<CircleCollider2D>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭 감지
    {
        playerClicked = true;
        Invoke("ResetPlayerClicked", clickDuration); // 일정 시간 후 playerClicked를 리셋
    }

        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        if (childAnimator != null)
        {
            childAnimator.SetInteger("AnimationState", isMoving ? 1 : 0);
        }
        if (rb.gravityScale == 0&&!isKnockedBack)
        {
            Vector3 moveHorizontal = new Vector3(moveSpeed * Time.deltaTime, 0, 0);
            Vector3 moveVertical = new Vector3(0, moveSpeed * Time.deltaTime, 0);

            bool isDiagonal = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
            if (isDiagonal)
            {
                moveVertical *= 0.5f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position -= moveHorizontal;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += moveHorizontal;
            }

            if (Input.GetKey(KeyCode.W))
            {
                transform.position += moveVertical;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= moveVertical;
            }

            if (Input.GetKeyDown(KeyCode.Space) && !isJumping && rb.gravityScale == 0) // 점프 중이 아닐 때만 점프 가능
            {
                StartCoroutine(Jump());
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                FlipChild(-1);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                FlipChild(1);
            }
            CheckForTilemapBounds();

        }
        CheckFallThreshold();

    }


    void FlipChild(int direction)
    {
        if (child != null)
        {
            SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
            if (childSpriteRenderer != null)
            {
                // direction에 따라 flipX 값을 설정
                childSpriteRenderer.flipX = direction < 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Slime"))
    {
        // 부딪힌 반대방향으로 넉백 계산
        Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;

        if (!playerClicked) // 클릭하지 않은 상태에서만 플레이어가 넉백됨
        {
            Knockback(knockbackDirection);
        }
    }
}

private void Knockback(Vector2 direction)
{
    isKnockedBack = true;
    rb.velocity = Vector2.zero; // 현재 속도 초기화
    rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    StartCoroutine(EndKnockback());
}

IEnumerator EndKnockback()
{
    yield return new WaitForSeconds(knockbackDuration);

    // 넉백 종료 후 속도 초기화
    rb.velocity = Vector2.zero;
    isKnockedBack = false;
}



    public HealthManager healthManager; // HealthManager 참조 추가

    void CheckFallThreshold()
{
    if (transform.position.y < fallThreshold)
    {
        // healthManager가 null이 아닌 경우에만 접근합니다.
        if (healthManager != null)
        {
            healthManager.TakeDamage(1); // 하트 하나를 잃습니다.
            if (healthManager.GetHealthCount() == 0)
            {
                checkPointPosition = startPosition;
                healthManager.SetHealth(3);
                PlayerPrefs.DeleteKey("CheckpointX");
                PlayerPrefs.DeleteKey("CheckpointY");
            }
            else
            {
                checkPointPosition = new Vector3(PlayerPrefs.GetFloat("CheckpointX", startPosition.x), PlayerPrefs.GetFloat("CheckpointY", startPosition.y), 0);
            }
        }
        else
        {
            Debug.LogError("HealthManager 참조가 없습니다.");
        }

        ResetPlayer();
    }
}

    void ResetPlayer()
    {
        transform.position = checkPointPosition; // 원래 위치로 돌아감
        rb.velocity = Vector2.zero; // 속도 초기화
        isJumping = false; // 점프 상태 초기화
        isFalling = false;
        if (circleCollider != null)
        {
            circleCollider.isTrigger = false;
        }
        rb.gravityScale = 0f;
        ResetLayerOrder();
        // 중력 스케일 초기화
        // 필요한 경우 다른 초기화 로직 추가
    }

    void CheckForTilemapBounds()
    {
        if (!IsTileAtPlayerPosition(transform.position, tilemaps) && !isJumping) // 추가된 점프 상태 확인
        {
            isFalling = true;
            if (circleCollider != null)
            {
                circleCollider.isTrigger = true;
            }
            StartCoroutine(ApplyGravityAndLayerOrder());
        }
        else
        {
            rb.gravityScale = 0f; // 타일이 있거나 점프 중이면 중력을 0으로 설정
        }
    }

    void UpdateLayerOrder()
    {
        // 플레이어의 SpriteRenderer 찾기
        SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            // 플레이어의 Order in Layer 값 변경
            playerSpriteRenderer.sortingOrder -= 6;
        }

        // 자식 오브젝트의 SpriteRenderer 찾아서 Order in Layer 값 변경
        foreach (Transform child in transform)
        {
            SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
            if (childSpriteRenderer != null)
            {
                childSpriteRenderer.sortingOrder -= 10;
            }
        }
    }

    private void ResetPlayerClicked()
    {
        playerClicked = false;
    }

    void ResetLayerOrder()
    {
        // 플레이어의 Order in Layer 값을 원래대로 복원
        SpriteRenderer playerSpriteRenderer = GetComponent<SpriteRenderer>();
        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.sortingOrder = originalPlayerSortingOrder;
        }

        // 자식 오브젝트의 Order in Layer 값을 원래대로 복원
        foreach (Transform child in transform)
        {
            if (originalChildSortingOrders.TryGetValue(child, out int originalSortingOrder))
            {
                SpriteRenderer childSpriteRenderer = child.GetComponent<SpriteRenderer>();
                if (childSpriteRenderer != null)
                {
                    childSpriteRenderer.sortingOrder = originalSortingOrder;
                }
            }
        }
    }

    public bool IsTileAtPlayerPosition(Vector3 playerPosition, List<Tilemap> tilemaps)
    {
        foreach (var tilemap in tilemaps)
        {
            Vector3Int playerCell = tilemap.WorldToCell(playerPosition);
            Vector3[] offsets = {
            new Vector3(0, 0.15f, 0), // 위쪽
            new Vector3(0, -0.15f, 0), // 아래쪽
            new Vector3(0, 0.1f, 0), // 오른쪽
            new Vector3(0, -0.1f, 0) // 왼쪽
        };

            foreach (var offset in offsets)
            {
                Vector3Int checkPos = tilemap.WorldToCell(playerPosition + offset);
                if (tilemap.HasTile(checkPos))
                {
                    return true;
                }
            }
        }

        return false;
    }
    IEnumerator ApplyGravityAndLayerOrder()
    {
        rb.gravityScale = 4f; // 중력 적용
        yield return new WaitForSeconds(0.175f); // 0.175초 기다림
        UpdateLayerOrder(); // Layer Order 업데이트
    }

    IEnumerator Jump()
    {
        isJumping = true; // 점프 시작을 표시

        float elapsedTime = 0;
        Vector3 originalPosition = child.localPosition;
        Vector3 targetPosition = originalPosition + new Vector3(0, jumpHeight, 0);

        while (elapsedTime < jumpDuration)
        {
            child.localPosition = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        child.localPosition = targetPosition;

        // 다시 내려오는 부분
        elapsedTime = 0;
        while (elapsedTime < jumpDuration)
        {
            child.localPosition = Vector3.Lerp(targetPosition, originalPosition, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        child.localPosition = originalPosition;
        isJumping = false; // 점프가 끝났으므로 상태를 변경

    }
}