using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private Button m_Button;
    #endregion

    private void Awake()
    {
        m_Button.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("InGame");
    }
}
