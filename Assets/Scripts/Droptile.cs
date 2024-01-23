using UnityEngine;

public class DisappearingTile : MonoBehaviour
{
    public float dropSpeed = 5f;
    public float disappearHeight = -10f;
    public float playerOnTileTime = 2f;
    public float respawnTime = 2f;

    private Vector3 originalPosition;
    private Transform playerTransform;
    private bool isPlayerOnTile = false;
    private bool isDropping = false;
    private float dropTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // 타일의 초기 위치 저장
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerOnTile)
        {
            dropTimer += Time.deltaTime;

            if (dropTimer >= playerOnTileTime)
            {
                StartDropping();
            }
        }

        if (isDropping)
        {
            transform.position -= new Vector3(0, dropSpeed * Time.deltaTime, 0);

            if (transform.position.y <= disappearHeight)
            {
                isDropping = false;
                gameObject.SetActive(false);
                Invoke("RespawnTile", respawnTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform;
            isPlayerOnTile = true;
            dropTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnTile = false;
            dropTimer = 0f;
        }
    }

    private void StartDropping()
    {
        isDropping = true;
        isPlayerOnTile = false;
        if (playerTransform != null)
        {
            playerTransform.SetParent(null);
        }
    }

    private void RespawnTile()
    {
        // 타일을 원래 위치로 되돌림
        transform.position = originalPosition;
        gameObject.SetActive(true);
    }
}
