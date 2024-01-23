using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health = 3;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private int lastHealth; // 마지막으로 저장된 건강 상태를 추적합니다.

    void Start()
    {
        lastHealth = health; // 초기 건강 상태를 설정합니다.
        UpdateHearts(); // 초기 하트 UI를 설정합니다.
    }

    void Update()
    {
        // 건강 상태가 변경되었는지 확인합니다.
        if (lastHealth != health)
        {
            UpdateHearts();
            lastHealth = health; // 최신 건강 상태를 저장합니다.
        }
    }

    void UpdateHearts()
    {
        // health 값이 hearts 배열의 길이를 초과하는지 확인합니다.
        if (health > hearts.Length)
        {
            Debug.LogWarning("Health exceeds the number of hearts available.");
            health = hearts.Length; // health 값을 hearts 배열의 길이로 제한합니다.
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < health) ? fullHeart : emptyHeart;
        }
    }

     public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(0, health); // 건강 상태가 음수가 되지 않도록 합니다.
        UpdateHearts(); // 하트 UI 업데이트
    }
}
