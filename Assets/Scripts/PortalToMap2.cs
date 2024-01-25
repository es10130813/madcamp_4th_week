using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : MonoBehaviour
{
    public Transform player; // 플레이어 캐릭터의 Transform
    public float interactionDistance = 3.0f; // 상호작용 가능한 최대 거리
    public string sceneToLoad = "Map2"; // 이동할 씬의 이름

    void Update()
    {
        // 플레이어와 오브젝트 간의 거리를 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 거리가 interactionDistance 이내이고 'F' 키가 눌렸을 때
        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.F))
        {
            // 지정된 씬으로 이동
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
