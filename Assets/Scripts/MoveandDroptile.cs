using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveandDroptile : MonoBehaviour
{
    public float dropSpeed = 5f;
    public float disappearHeight = -10f;
    public float playerOnTileTime = 2f;
    public float respawnTime = 2f;
    public Transform startPos, endPos; // 이동을 위한 시작 및 끝 위치
    public float speed = 5f; // 이동 속도

    private Vector3 originalPosition;
    private Transform playerTransform;
    private bool isPlayerOnTile = false;
    private bool isDropping = false;
    private float dropTimer = 0f;
    private Transform desPos; // 현재 목적지

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        desPos = startPos; // 초기 목적지를 시작 위치로 설정
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

    void FixedUpdate()
    {
        // 타일이 떨어지고 있지 않을 때만 이동
        if (!isDropping)
        {
            MoveTile();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerTransform.SetParent(transform);
            isPlayerOnTile = true;
            dropTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform.SetParent(null);
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
        transform.position = originalPosition;
        gameObject.SetActive(true);
    }

    private void MoveTile()
    {
        Vector3 direction = (desPos.position - transform.position).normalized;
        transform.position += direction * Time.deltaTime * speed;

        if (Vector3.Distance(transform.position, desPos.position) <= 0.05f)
        {
            desPos = desPos == endPos ? startPos : endPos;
        }
    }
}
