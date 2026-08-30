using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    /// <summary>
    /// 체력이 존재하는 오브젝트 혹은 캐릭터라면 필요한 체력 관리 컴포넌트
    /// 현재 체력을 저장 및 감소하거나 0이 되었을 시 사망 처리를 실행합니다
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] PlayerDodgeController dodgeCtr;                 //  회피컨트롤러 컴포넌트 참조 변수

    [Header("체력 설정")]
    [SerializeField] float maxHealth = 100f;        // 대상의 최대 체력
    float currentHealth;                            // 현재 체력
    bool isDead;                                    // 대상의 사망여부

    // 외부에서의 현재 체력, 사망을 직접 변경하는걸 막는 용도의 프로퍼티
    public float CurrentHealth { get { return currentHealth; } }
    public bool IsDead { get { return isDead; } }


    void Awake()
    {
        if(dodgeCtr == null)
            dodgeCtr = GetComponent<PlayerDodgeController>();

        InitializeHealth();
    }

    // 게임이 시작 될 때 현재 체력을 최대 체력으로 초기화
    void InitializeHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    // IDamageable 인터페이스로부터 정의한 피해 처리 메소드
    // 전달받은 피해량 만큼 체력이 감소하는 역할을 담당

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;
        if (dodgeCtr != null && dodgeCtr.IsDodging)
            return;

        // 0 이하의 피해가 들어온다면 변경 x
        if (damage <= 0f)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"{gameObject.name}이 {damage}의 피해를 받았습니다. 현재 체력 : {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }


    // 현재 체력이 0이 되면 사망 상태로 전환하는 메소드
    void Die()
    {
        isDead = true;

        Debug.Log($"{gameObject.name}이 사망하였습니다");
    }

    // 피해 테스트가 적용되는지 확인하기 위한 임시 메소드 적용될 시 삭제 예정
    [ContextMenu("Take Test Damege")]
    void TakeTestDamage()
    {
        TakeDamage(20f);
    }
}
