using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 Health와 화면의 체력 UI를 연결하는 컴포넌트
    /// 체력 변경 이벤트를 표시고 전달받은 값을 Slider와 Text에 표시
    /// </summary>

    [Header("플레이어")]
    [SerializeField] Health health;                                   // 화면에 표시할 플레이어의 Health 컴포넌트

    [Header("체력 UI")]
    [SerializeField] Slider healthSlider;                           // 현재 체력 비율을 표시할 슬라이더 컴포넌트
    [SerializeField] TextMeshProUGUI healthText;         // (현재 체력 / 최대 체력)을 숫자로 표시할 텍스트


    private void OnEnable()
    {
        BindHealthEvents();
        InitializeHealthUI();
    }
    private void OnDisable()
    {
        UnBindHealthEvents();
    }


    // Health의 체력 변경 표시 이벤트를 시작하는 메소드
    void BindHealthEvents()
    {
        if (health == null)
            return;

        health.OnHealthChanged += HandleHealthChanged;
    }

    // 체력 변경을 표시 이벤트를 그만두는 메소드
    void UnBindHealthEvents()
    {
        if (health == null)
            return;

        health.OnHealthChanged -= HandleHealthChanged;
    }

    // Health의 현재 값을 읽고 UI를 초기화 시키는 메소드
    void InitializeHealthUI()
    {
        if (health == null)
            return;

        UpdateHealthUI(health.CurrentHealth, health.MaxHealth);
    }

    // 이벤트 발생시 호출하는 메소드
    void HandleHealthChanged(float curHealth, float maxHealth)
    {
        UpdateHealthUI(curHealth, maxHealth);
    }

    // 현재 체력과 최대 체력을 읽어와 Slider, Text에 적용시킵니다
    void UpdateHealthUI(float curHealth, float maxHealth)
    {
        UpdateHealthSlider(curHealth, maxHealth);
        UpdateHealthText(curHealth, maxHealth);
    }

    // 현재 체력을 비율로 계산해 Slider에 적용합니다
    void UpdateHealthSlider(float curHealth, float maxHealth)
    {
        if (healthSlider == null)
            return;

        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;

        // 최대 체력이 0일 경우 나눗셈 오류 발생할 수 있어 기본 체력 비율을 0으로 설정
        float healthRatio = 0;

        // 최대 체력이 0보다 클 때만 비율을 계산합니다
        if (maxHealth > 0)
            healthRatio = curHealth / maxHealth;

        // 계산이 0보다작거나 1보다 크지않도록 설정
        healthRatio = Mathf.Clamp01(healthRatio);

        // slider값 변경
        healthSlider.SetValueWithoutNotify(healthRatio);
    }

    // 현재 체력과 최대 체력을 Text에 표시하는 메소드
    void UpdateHealthText(float curHealth, float maxHealth)
    {
        if (healthText == null)
            return;

        healthText.text = $"{curHealth: 0} / {maxHealth: 0}";
    }
}
