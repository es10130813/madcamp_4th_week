using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Slime : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private List<Tilemap> tilemaps;
    public Rigidbody2D rb;
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

    private void Start()
    {
        rb.gravityScale = 0f;
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        PickRandomDirection();
    }

    private void Update()
    {
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
            if (Random.Range(0, 100) < 15)
            {
                StartCoroutine(Jump());

                // 점프 시 랜덤 방향 설정
                PickRandomDirection();
            }

            jumpCheckTimer = 0f;
        }

        CheckFallThreshold();
    }

    void MoveSlime()
    {
        Vector3 moveDelta = new Vector3(movementDirection.x * moveSpeed * Time.deltaTime, movementDirection.y * moveSpeed * Time.deltaTime, 0);
        transform.position += moveDelta;
        spriteRenderer.flipX = movementDirection.x < 0;
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
