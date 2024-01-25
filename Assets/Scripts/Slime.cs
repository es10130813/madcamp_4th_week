using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Slime : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Tilemap> tilemaps;
    [SerializeField] private float knockbackForce = 0.2f;
    [SerializeField] private float clickDuration = 0.5f;

    [SerializeField] private float knockbackDuration = 0.5f; // 넉백 지속 시간
    private bool isKnockedBack = false;

    public Rigidbody2D rb;
    private bool playerClicked = false; 
    public SpriteRenderer spriteRenderer;
    public Transform child;
    public float jumpHeight = 2f;
    public float jumpDuration = 0.5f;
    public float fallThreshold = -10f;
    private bool isJumping = false;
    private bool isFalling = false;
    private Vector3 startPosition;
    private float movementChangeInterval = 2f;
    private float movementTimer = 0f;
    private Vector2 movementDirection;
    private float fallDelay = 0.5f;
    private float edgeDistanceThreshold = 1f;
    private float boundaryApproachThreshold = 0.5f;
    private float jumpCheckInterval = 0.1f; // 점프 체크 간격
    private float jumpCheckTimer = 0f;
    private CircleCollider2D circleCollider;
    private void Start()
    {
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        PickRandomDirection();
        circleCollider = GetComponent<CircleCollider2D>();

        string currentSword = PlayerPrefs.GetString("DefaultAnimation", "right_hand_sword");

        if(currentSword=="sword2"){
            knockbackForce = 1f;
        }
        else if(currentSword=="sword3"){
            knockbackForce = 5f;
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 클릭 감지
    {
        playerClicked = true;
        Invoke("ResetPlayerClicked", clickDuration); // 일정 시간 후 playerClicked를 리셋
    }

        movementTimer += Time.deltaTime;

        if (movementTimer >= movementChangeInterval)
        {
            PickRandomDirection();
            movementTimer = 0f;
        }

        // 이동 전의 위치를 저장
        Vector3 prevPosition = transform.position;

        // 점프 중일 때만 움직임을 처리
        if (isJumping)
        {
            MoveSlime();
        }

        // 이동 후의 위치를 계산
        Vector3 newPosition = transform.position;

        // 이동한 거리를 계산
        Vector3 moveDelta = newPosition - prevPosition;

        // 타일맵 경계 체크
        CheckForTilemapBounds(prevPosition, moveDelta);

        // 점프 체크 타이머 업데이트
        jumpCheckTimer += Time.deltaTime;

        // 점프 상태에서만 랜덤 방향으로 움직이도록 수정
        if (!isJumping && !isFalling && jumpCheckTimer >= jumpCheckInterval)
        {
            if (Random.Range(0, 100) < 10)
            {
                StartCoroutine(Jump());

                // 점프 시 랜덤 방향 설정
                PickRandomDirection();
            }

            jumpCheckTimer = 0f;
        }

        CheckFallThreshold();
    }

   private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && !player.isKnockedBack && playerClicked)
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            Knockback(knockbackDirection);
        }
    }
    else if (collision.gameObject.CompareTag("Slime"))
    {
        Slime otherSlime = collision.gameObject.GetComponent<Slime>();
        // 슬라임과 슬라임의 충돌에서만 넉백 중인 슬라임을 무시
        if (otherSlime != null && otherSlime.isKnockedBack)
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
        }
    }
}

private void Knockback(Vector2 direction)
{
    isKnockedBack = true;
    rb.velocity = Vector2.zero; // 현재 속도 초기화
    rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    GetComponent<Collider2D>().isTrigger = true; // 넉백 중에는 Collider를 Trigger로 설정
    StartCoroutine(EndKnockback());
}

IEnumerator EndKnockback()
{
    yield return new WaitForSeconds(knockbackDuration);

    rb.velocity = Vector2.zero;
    isKnockedBack = false;
    GetComponent<Collider2D>().isTrigger = false; // 넉백이 끝나면 Collider를 다시 Trigger가 아닌 상태로 설정
}



    void MoveSlime()
    {
        Vector3 moveDelta = new Vector3(movementDirection.x * moveSpeed * Time.deltaTime, movementDirection.y * moveSpeed * Time.deltaTime, 0);
        transform.position += moveDelta;
        spriteRenderer.flipX = movementDirection.x < 0;
    }

    private void ResetPlayerClicked()
{
    playerClicked = false;
}

    void PickRandomDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        Vector2 newDirection = new Vector2(randomX, randomY).normalized;

        movementDirection = newDirection;
    }

    IEnumerator Jump()
    {
        if (isFalling)
            yield break;

        isJumping = true;

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

        elapsedTime = 0;
        while (elapsedTime < jumpDuration)
        {
            child.localPosition = Vector3.Lerp(targetPosition, originalPosition, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        child.localPosition = originalPosition;
        isJumping = false;
    }

    void CheckForTilemapBounds(Vector3 prevPosition, Vector3 moveDelta)
    {
        float step = 0.1f;
        for (float t = 0; t < 1; t += step)
        {
            Vector3 currentPos = Vector3.Lerp(prevPosition, transform.position, t);

            if (!IsTileAtSlimePosition(currentPos, tilemaps) && !isJumping)
            {
                Vector3 newPosition = transform.position - moveDelta.normalized * edgeDistanceThreshold * step;
                transform.position = newPosition;
                if (circleCollider != null)
            {
                circleCollider.isTrigger = true;
            }

                // 슬라임이 밖으로 떨어질 때 중력을 적용하여 아래로 떨어지도록 설정
                rb.gravityScale = 4f;

                FallAndDisappear();
                break;
            }
        }
    }

    public bool IsTileAtSlimePosition(Vector3 position, List<Tilemap> tilemaps)
    {
        foreach (var tilemap in tilemaps)
        {
            Vector3Int cellPosition = tilemap.WorldToCell(position);
            if (tilemap.HasTile(cellPosition))
            {
                return true;
            }
        }
        return false;
    }

    void CheckFallThreshold()
    {
        if (transform.position.y < fallThreshold && !isJumping && !isFalling)
        {
            // 슬라임이 밖으로 떨어질 때 중력을 적용하여 아래로 떨어지도록 설정
            rb.gravityScale = 4f;

            FallAndDisappear();
        }
    }

    void FallAndDisappear()
    {
        isFalling = true;
        spriteRenderer.sortingOrder = 1;
        child.GetComponent<SpriteRenderer>().sortingOrder = 1;
        StartCoroutine(DeactivateAfterDelay(fallDelay));
    }

    IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
