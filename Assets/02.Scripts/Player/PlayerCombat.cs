using System;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 기본 공격 입력, 공격 판정을 처리하는 컴포넌트
    /// 공격 입력을 받으면 AttackPoint를 중심으로 공격 범위를 검사, 후 범위 안에 대상에게 피해를 전달합니다
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] PlayerInputReader inputReader;             // 입력 이벤트를 위한 참조
    [SerializeField] Transform attackPoint;                     // 실제 공격 범위의 중심 위치

    [Header("공격 설정")]
    [SerializeField] float attackDamage = 20f;      // 기본 공격 데미지
    [SerializeField] float attackRadius = 1f;       // 기본 공격 범위
    [SerializeField] float attackCoolTime = 0.5f;   // 다음 공격이 가능해질 때까지의 시간

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
