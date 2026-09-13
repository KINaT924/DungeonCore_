using UnityEngine;
using System.Collections;

public class EnemyDeathSink : MonoBehaviour
{
    /// <summary>
    /// 몬스터 사망 후 사망 애니메이션 재생 후
    /// 천천히 내려가게 만드는 연출 컴포넌트
    /// 이 컴포넌트는 사망 후 시각적 정리만 담당합니다
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] Health health;
    [SerializeField] Transform root;                // 밑으로 내려갈 몬스터 위치값
    [SerializeField] Collider bodyCollider;         // 바닥으로 내려갈 때 충돌을 제거하기 위한 콜라이더

    [Header("연출 설정")]
    [SerializeField, Min(0f)] float startDelay = 0.8f;          // 시작 딜레이
    [SerializeField, Min(0.1f)] float sinkDur = 1.2f;           //  가라앉는 지속시간
    [SerializeField, Min(0.1f)] float sinkDistance = 2f;        // 가라앉는 거리
    [SerializeField] bool deactivateAfterSink = true;           // 연출이 끝난 몬스터를 비활성화 할지의 여부

    Vector3 startLocalPosition;
    Coroutine sinkCoroutine;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
        if (bodyCollider == null) 
            bodyCollider = GetComponent<Collider>();
        if(root != null)
            startLocalPosition = root.localPosition;
    }

    private void OnEnable()
    {
        // 비활성화 된 후 다시 사용되는 경우 복구
        if (root != null)
            root.localPosition = startLocalPosition;
        if (bodyCollider != null)
            bodyCollider.enabled = true;
        if (health != null)
            health.OnDied += HandleDeath;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandleDeath;
        if(sinkCoroutine != null)
        {
            StopCoroutine(sinkCoroutine);
            sinkCoroutine = null;
        }

        // 풀에서 다시 꺼낼 때에 원래 위치로 복구
        if (root != null)
            root.localPosition = startLocalPosition;
    }

    // 사망 이벤트가 발생할 때에 연출 시작
    void HandleDeath()
    {
        if (root == null || sinkCoroutine != null)
            return;
        sinkCoroutine = StartCoroutine(SinkRoutine());
    }

    IEnumerator SinkRoutine()
    {
        // 사망 애니메이션을 끝내고 가라앉기 시작
        if( startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        // 가라앉는 동안 충돌하지않게 설정
        if(bodyCollider != null)
            bodyCollider.enabled = false;

        Vector3 startPos = root.localPosition;
        Vector3 endPos = startPos + Vector3.down * sinkDistance;
        float elapsedTime = 0f;

        while(elapsedTime < sinkDur)
        {
            elapsedTime += Time.deltaTime;
            // 경과 시간을 전체 연출 시간으로 나눈 뒤 진행률 계산
            float ratio = Mathf.Clamp01(elapsedTime / sinkDur);
            // 시작과 끝이 움직이지 않도록 부드럽게 설정
            float smmothRatio = Mathf.SmoothStep(0f, 1f, ratio);

            root.localPosition = Vector3.Lerp(startPos, endPos, smmothRatio);

            yield return null;
        }

        root.localPosition = endPos;
        sinkCoroutine = null;

        // 현재 장면에서 사라진 후 리젠에서 재사용
        if(deactivateAfterSink)
            gameObject.SetActive(false);
    }
}
