using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    public Image[] image;

    public GameObject gameOverpanel;
    public Button restartButton;
    public TextMeshProUGUI scoreText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverpanel != null)
        {
            gameOverpanel.SetActive(false);
        }

        UpdateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        // Die 버튼 없이 테스트할 때는 K 키로 같은 동작을 확인할 수 있어요.
        if (Input.GetKeyDown(KeyCode.K))
        {
            OnDieButtonClicked();
        }
    }

    void OnDieButtonClicked()
    {
        LoseLife();
    }

    public bool LoseLife()
    {
        int activeLifeCount = 0;

        for (int i = 0; i < image.Length; i++)
        {
            if (image[i] != null && image[i].gameObject.activeSelf)
            {
                activeLifeCount++;
            }
        }

        // 더이상 목숨이 없으면 즉시 게임오버 처리
        if (activeLifeCount <= 0)
        {
            ShowGameOverPanel();
            return true;
        }

        // 화면에 보이는 마지막 life 이미지를 하나 숨김
        for (int i = image.Length - 1; i >= 0; i--)
        {
            if (image[i] != null && image[i].gameObject.activeSelf)
            {
                image[i].gameObject.SetActive(false);
                break;
            }
        }

        // 죽기 직전 목숨이 1개였다면 지금 0개가 되므로 게임오버 처리
        if (activeLifeCount == 1)
        {
            ShowGameOverPanel();
            return true;
        }

        return false;
    }

    void ShowGameOverPanel()
    {
        if (gameOverpanel != null)
        {
            gameOverpanel.SetActive(true);
        }
    }

    public void AddScore(int amount)
    {
        int currentScore = 0;

        if (scoreText != null)
        {
            string textValue = scoreText.text.Replace(",", "");
            int.TryParse(textValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out currentScore);
        }

        currentScore += amount;
        if (currentScore < 0)
        {
            currentScore = 0;
        }

        UpdateScoreText(currentScore);
    }

    void UpdateScoreText()
    {
        UpdateScoreText(0);
    }

    void UpdateScoreText(int value)
    {
        if (scoreText != null)
        {
            scoreText.text = value.ToString("N0"); //세자리수 콤마 적용
        }
    }
}
