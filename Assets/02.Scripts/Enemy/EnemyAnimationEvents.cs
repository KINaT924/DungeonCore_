using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    /// <summary>
    /// 모델의 Animator에서 발생하는 Animation Event를
    /// 부모 EnemyController에 전달하는 용도의 컴포넌트
    /// 공격 조건이나 피해량은 판단하지 않고 이벤트 전달만 담당
    /// </summary>

    [SerializeField] EnemyController enemyCtr;

    private void Awake()
    {
        if (enemyCtr == null)
            enemyCtr = GetComponentInParent<EnemyController>();
    }

    // 몬스터의 기본공격이 실제 타격하는 프레임에서 호출하는 이벤트 메소드
    public void EnableAttackArea()
    {
        if (enemyCtr != null)
            enemyCtr.EnableAttackArea();
    }

    // 몬스터의 기본공격이 실제 끝나는 프레임에서 호출하는 이벤트 메소드
    public void DisableAttackArea()
    {
        if(enemyCtr != null)
            enemyCtr.DisableAttackArea();
    }
    // 클립의 종료 이벤트용 메소드, 공격이 끝남을 전달
    public void EndAttack()
    {
        if (enemyCtr != null)
            enemyCtr.EndAttack();
    }
    
    // Hit 애니메이션이 끝났음을 전달
    public void EndHit()
    {
        if(enemyCtr != null)
            enemyCtr.EndHit();
    }
}
