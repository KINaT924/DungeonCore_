using UnityEditor.ShaderGraph.Internal;
using UnityEngine;


public interface IDamageable
{
    /// <summary>
    /// 외부 공격으로부터 피해를 전달해받는 인터페이스
    /// 피해를 받을 수 있는 오브젝트가 공통으로 구현해야 될 인터페이스입니다
    /// </summary>

    void TakeDamage(float damage);
}
