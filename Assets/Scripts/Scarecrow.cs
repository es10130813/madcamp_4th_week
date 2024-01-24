using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Scarecrow : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 0.2f;
    [SerializeField] private float clickDuration = 0.5f;
    [SerializeField] private float knockbackDuration = 0.5f;
    [SerializeField] private List<Tilemap> tilemaps;
    [SerializeField] private float fallThreshold = -10f; // 떨어질 수 있는 최대 높이

    private Vector3 startPosition; // 초기 위치 저장
    private bool isKnockedBack = false;
    private bool playerClicked = false;
    private bool isFalling = false;

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Transform child;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position; // 초기 위치 설정
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerClicked = true;
            Invoke("ResetPlayerClicked", clickDuration);
        }

        CheckForTilemapBounds();
        CheckFallThreshold();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerClicked)
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            Knockback(knockbackDirection);
        }
    }

    private void Knockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(EndKnockback());
    }

    IEnumerator EndKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);

        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void ResetPlayerClicked()
    {
        playerClicked = false;
    }

    void CheckForTilemapBounds()
    {
        if (!IsTileAtScarecrowPosition(transform.position, tilemaps) && !isFalling)
        {
            circleCollider.isTrigger = true;
            rb.gravityScale = 4f;
            isFalling = true;
        }
    }

    public bool IsTileAtScarecrowPosition(Vector3 position, List<Tilemap> tilemaps)
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

    private void CheckFallThreshold()
    {
        if (transform.position.y < fallThreshold && isFalling)
        {
            // 초기 위치로 복귀
            transform.position = startPosition; 
            rb.gravityScale = 0f; 
            rb.velocity = Vector2.zero; 
            if (circleCollider != null)
        {
            circleCollider.isTrigger = false;
        }
            isFalling = false;
            gameObject.SetActive(true);
        }
    }
}
