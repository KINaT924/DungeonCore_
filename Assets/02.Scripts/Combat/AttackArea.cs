using System.Collections.Generic;
using Unity.VisualScripting;
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
}
