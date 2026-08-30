using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    /// <summary>
    /// 플레이어 공격의 실제 충돌 범위를 관리하는 컴포넌트
    /// 공격 중에만 콜라이더가 활성화되고 공격이 끝날 시 비활성화 됩니다
    /// </summary>


    [Header("공격 대상 설정")]
    [SerializeField] LayerMask targetLayer;     // 공격 판정 대상의 레이어
    Collider areaCollider;                      // 실제 공격 범위의 콜라이더
    float attackDamage;                         // 현재 공격에서 대상에게 적용할 피해량

    HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();   // 한번의 공격으로 중복 피해주는것 방지

    private void Awake()
    {
        InitializeAttackArea();
    }
    

    // 연결된 콜라이더를 가져오고 게임 시작 시 공격범위 비활성화
    void InitializeAttackArea()
    {
        areaCollider = GetComponent<Collider>();

        if (areaCollider == null)
            return;

        areaCollider.isTrigger = true;
        areaCollider.enabled = false;
    }

    // 공격을 시작하면서 이번 공격의 피해량 전달
    public void BeginAttack(float damage)
    {
        if(areaCollider == null) 
            return;

        attackDamage = damage;
        damagedTargets.Clear();
        areaCollider.enabled = true;
    }

    // 공격이 끝나면서 공격범위 비활성화
    public void EndAttack()
    {
        if(areaCollider == null) 
            return;
        areaCollider.enabled = false;
    }

    // 트리거에 따른 호출
    private void OnTriggerEnter(Collider other)
    {
        TryDamageTarget(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryDamageTarget(other);
    }

    // 감지된 콜라이더가 공격 대상인지 확인하고 조건에 맞다면 피해를 적용
    void TryDamageTarget(Collider target)
    {
        // 공격 대상 레이어가 아닌 경우 무시
        if (((1 << target.gameObject.layer) & targetLayer) == 0)
            return;

        IDamageable damageable = target.GetComponent<IDamageable>();

        // 공격 대상이 IDamageable을 구현하지 않은 경우 무시
        if (damageable == null)
            return;
        // 이미 피해를 준 대상이라면 무시
        if (damagedTargets.Contains(damageable))
            return;

        // 피해 적용
        damagedTargets.Add(damageable);
        damageable.TakeDamage(attackDamage);
    }

    // 공격 범위가 비활성화 될 때 콜라이더를 비활성화하고 피해를 준 대상 목록 초기화
    private void OnDisable()
    {
        if (areaCollider != null)
            areaCollider.enabled = false;

        damagedTargets.Clear();
    }
}
