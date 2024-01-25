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
    if (collision.gameObject.CompareTag("Player"))
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && !player.isKnockedBack && playerClicked)
        {
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            Knockback(knockbackDirection);
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
    private void ResetPlayerClicked()
    {
        playerClicked = false;
    }

    void CheckForTilemapBounds()
    {
        if (!IsTileAtScarecrowPosition(transform.position, tilemaps) && !isFalling)
        {
            circleCollider.isTrigger = true;
            child.GetComponent<SpriteRenderer>().sortingOrder = 1;
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
             child.GetComponent<SpriteRenderer>().sortingOrder = 10;
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
