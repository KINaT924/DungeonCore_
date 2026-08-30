using System;
using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 기본 공격 입력, 공격 판정을 처리하는 컴포넌트
    /// 공격 입력을 받으면 AttackArea를 중심으로 공격 범위를 검사, 후 범위 안에 대상에게 피해를 전달합니다
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] PlayerInputReader inputReader;             // 입력 이벤트를 위한 참조
    [SerializeField] AttackArea attackArea;                     // 실제 공격 범위의 중심 오브젝트
    [SerializeField] PlayerDodgeController dodgeCtr;            // 회피 중 공격 입력을 막기 위한 회피 컴포넌트
    [SerializeField] Health health;                             // 플레이어가 사망했는지 알기위한 체력 확인 컴포넌트

    [Header("공격 설정")]
    [SerializeField] float attackDamage = 20f;              // 기본 공격 데미지
    [SerializeField] float attackCoolTime = 0.5f;           // 다음 공격이 가능해질 때까지의 시간
    float coolTimer;                                        // 현재 남아있는 공격 쿨타임
    bool isAttacking;                                       // 현재 공격 중인지 체크하는 변수

    public bool IsAttacking { get { return isAttacking; } }      // 외부에서 공격중인지 확인할 수 있는 프로퍼티

    private void Awake()
    {
        if (dodgeCtr == null)
            dodgeCtr = GetComponent<PlayerDodgeController>();
        if (health == null)
            health = GetComponent<Health>();
    }
    void OnEnable()
    {
        BindInputEvents();
    }

    private void OnDisable()
    {
        UnBindInputEvents();
        DisableAttackArea();
    }
    void Update()
    {
        // 죽었으면 행동 x
        if(health != null && health.IsDead)
        {
            CancelAttack();
            return;
        }

        UpdateCoolTimer();
    }

    // 공격 입력시 발생하는 이벤트 처리 메소드
    void BindInputEvents()
    {
        if(inputReader == null)
            return;

        inputReader.AttackPressed += HandleAttackPressed;
    }

    // 공격 입력시 발생했던 이벤트를 해제하는 메소드
    // OnEnabla에서 이벤트가 호출되었다면 OnDisable에서 호출되어 같은 이벤트가 중복 구독되는 문제를 방지합니다
    void UnBindInputEvents()
    {
        if (inputReader == null)
            return;

        inputReader.AttackPressed -= HandleAttackPressed;
    }

    // 공격 입력시 공격을 실행하기전에 공격할 수 있는 상태인지에 대한 조건을 체크하는 메소드
    void HandleAttackPressed()
    {
        // 죽었거나 회피중일 시 행동 x
        if (health != null && health.IsDead)
            return;
        if (dodgeCtr != null && dodgeCtr.IsDodging)
            return;

        // 공격중일 때 또 공격을 반복하는것을 방지
        if (isAttacking)
            return;

        if(coolTimer > 0f)
            return;

        if(attackArea == null)
            return;

        StartAttack();
    }

    // 공격 상태를 시작하는 메소드
    void StartAttack()
    {
        isAttacking = true;
        coolTimer = attackCoolTime;
    }

    public void CancelAttack()
    {
        isAttacking = false;
        DisableAttackArea();
    }
    // 공격 애니메이션의 특정 프레임에서 호출되는 메소드
    public void EnableAttackArea()
    {
        if (attackArea == null)
            return;

        // 해당 데미지만큼 피해
        attackArea.BeginAttack(attackDamage);
    }

    // 공격 애니메이션이 끝나는 프레임에서 호출되는 메소드
    // 이벤트를 통해 호출
    public void DisableAttackArea()
    {
        if (attackArea == null)
            return;

        attackArea.EndAttack();
    }

    // 공격 에니메이션이 끝나기 직전에 호출되는 메소드
    // 다음 상태 애니메이션으로 돌아갈 수 있게 만듦
    // 마찬가지로 이벤트로 호출
    public void EndAttack()
    {
        isAttacking = false;
        DisableAttackArea();
    }
    // 공격 쿨타임일 떄 남은 시간을 갱신하는 메소드
    void UpdateCoolTimer()
    {
        if (coolTimer <= 0f)
            return;

        // 쿨타임이 0보다 클 때만 감소하며, 0보다 작아지지 않도록 Mathf.Max를 사용하여 0으로 고정
        coolTimer -= Time.deltaTime;
        coolTimer = Mathf.Max(coolTimer, 0f);
    }
}
