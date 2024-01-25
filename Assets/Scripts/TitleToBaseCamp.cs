using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요함
using UnityEngine.UI; // UI 버튼을 위해 필요함

public class SceneLoader : MonoBehaviour
{
    // 씬 이동 함수
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void Start()
    {
        // 버튼 컴포넌트 찾기
        Button btn = GetComponent<Button>();
        // 버튼에 이벤트 리스너 추가
        btn.onClick.AddListener(() => LoadScene("BaseCamp"));
    }
}
