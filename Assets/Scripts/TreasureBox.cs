using UnityEngine;

public class InteractionScript : MonoBehaviour
{
    public GameObject alertPanel; // 알림창에 해당하는 UI 오브젝트
    public GameObject objectToAnimate; // 애니메이션을 변경할 오브젝트

    public string newAnimationState; // 변경할 새로운 애니메이션 상태의 이름

    private bool isPlayerNear = false; // 플레이어가 오브젝트 근처에 있는지 여부
    private int hasToggled = 0; // 알림창을 이미 한 번 토글했는지 여부

    
    void Start()
    {
        LoadDefaultAnimation();
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F) && hasToggled < 2)
        {
            alertPanel.SetActive(!alertPanel.activeSelf);
            hasToggled++;

            // 여기서 다른 오브젝트의 애니메이션 변경
            ChangeAnimation();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void ChangeAnimation()
    {
        // Animator 컴포넌트를 가져오고 새로운 애니메이션 상태로 변경
        Animator animator = objectToAnimate.GetComponent<Animator>();
        Debug.Log("ChangeAnimation: "+newAnimationState);

        if (animator != null)
        {
            animator.Play(newAnimationState);
            PlayerPrefs.SetString("DefaultAnimation", newAnimationState);
            PlayerPrefs.Save();
        }
    }

       private void LoadDefaultAnimation()
    {
        Debug.Log("LoadDefaultAnimation(start)");
        string defaultAnimation = PlayerPrefs.GetString("DefaultAnimation", "right_hand_sword"); // DefaultState는 기본 애니메이션 이름

        // Animator 컴포넌트를 가져와서 저장된 애니메이션으로 설정
        Animator animator = objectToAnimate.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(defaultAnimation);
        }
    }
}