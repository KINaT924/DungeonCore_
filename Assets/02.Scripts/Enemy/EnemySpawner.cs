using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// 플레이어 주변, 코어 주변의 유효한 NavMesh 위치에 몬스터를 생성하고,
    /// 동시에 살아 있을 수 있는 몬스터 수를 제한하는 스포너입니다.
    ///
    /// 사망한 몬스터는 EnemyDeathSink에 의해 비활성화되며,
    /// 이후 새 몬스터가 필요할 때 비활성화된 몬스터를 다시 사용합니다.
    /// </summary>

    [Header("스폰 대상")]
    [SerializeField] EnemyController enemyPrefab;
    [SerializeField] Health playerHealth;

    [Header("스폰 관련")]
    [SerializeField, Min(0f)] float playerMinDistance = 10f;                     // 플레이어 주변 최소 스폰거리
    [SerializeField, Min(0.1f)] float playerMaxDistance = 25f;                 // 플레이어 주변 최대 스폰거리
    [SerializeField] Transform coreCenter;                                             // 코어 주변 스폰 위치 값
    [SerializeField, Min(0f)] float coreMinDistance = 3f;                         // 코어 주변 최소 스폰거리
    [SerializeField, Min(0.1f)] float coreMaxDistance = 8f;                     // 코어 주변 최대 스폰거리
    [SerializeField, Range(0f, 1f)] float spawnChance = 0.6f;                // 0일 시 플레이어 주변 스폰, 1일 시 코어 주변 스폰

    [Header("NavMash 관련")]
    [SerializeField, Min(0.1f)] float navMeshDistance = 3f;                     // 무작위 지점 주변에서 NavMesh를 찾기 위한 거리
    [SerializeField, Min(1f)] int maxPosAttempts = 30;                           // 몬스터를 생성한뒤 다음 생성 지점을 검사하는데 필요한 최대 지점 횟수

    [Header("몬스터 관련")]
    [SerializeField, Min(0)] int enemyCount = 5;                                    // 시작하였을 때 몬스터 개채 수
    [SerializeField, Min(1)] int maxEnemyCount = 50;                           // 최대 몬스터 개채 수
    [SerializeField, Min(0f)] float reSpawnDeley = 3f;                            // 리스폰 딜레이


    // 생성되었거나 비활성화 상태인 몬스터를 기록 후
    // 스폰에서 우선적으로 사용
    readonly List<EnemyController> enemyPool = new List<EnemyController>();
    // 각 몬스터의 컴포넌트들을 해당 몬스터와 함께 콜백을 거쳐 저장합니다
    readonly Dictionary<Health, Action> deathHandlers = new Dictionary<Health, Action>();

    int aliveEnemyCount;        // 살아있는 몬스터 수
    bool isCoreActive;             // 코어가 존재하고 있는지
    bool isShuttingDown;        // 리젠 코루틴의 오류발생 방지

    // 초기 몬스터 생성
    private void Start()
    {
        if (!isCoreActive)
            return;
        if(enemyPrefab == null)
        {
            enabled = false;
            return;
        }
        if (playerHealth == null)
        {
            enabled = false;
            return;
        }

        bool coreWasRemoved = coreCenter == null || !coreCenter.gameObject.activeInHierarchy;

        if (coreWasRemoved)
            SetCoreState(null, false);

        // 테스트용 !! - 코어강화 시스템 테스트용!!
        isCoreActive = coreCenter != null && coreCenter.gameObject.activeInHierarchy;

        int spawnCount = Mathf.Min(enemyCount, maxEnemyCount);
        for(int i =0; i<spawnCount; i++)
        {
            if(!TrySpawnEnemy())
            {
                Debug.LogError($"초기 몬스터 {i + 1}번째 유효 위치를 찾지 못하였습니다.", this);
            }
        }
    }

    // 최대 생존 몬스터를 검사하고 유효한 NavMesh 위치를 찾은 뒤
    // 몬스터를 생성합니다. 비활성 몬스터가 존재한다면 재사용합니다
    bool TrySpawnEnemy()
    {
        if(aliveEnemyCount >= maxEnemyCount)
            return false;
        if(!TryGetSpawnPosition(out Vector3 spawnPos))
                return false;

        EnemyController enemy = GetOrCreateEnemy(spawnPos);

        if(enemy ==null)
            return false;

        // 첫 실행 전 추적 대상과 현재 전역코어 강화 상태를 전달합니다
        enemy.Initialize(playerHealth, isCoreActive);

        aliveEnemyCount++;
        return true;
    }

    // 스폰 중심이 선택됬다면, 선택된 중심을 기준으로 최소,최대 거리 안에서 무작위 위치를 생성합니다
    bool TryGetSpawnPosition(out Vector3 spawnPos)
    {
        for(int i = 0; i < maxPosAttempts; i++)
        {
            // 위치 검사를 실행해 플레이어와 코어 중 어느 위치를 중심으로 사용할지 결정
            SelectSpawnArea(out Vector3 spawnCenter, out float minDis, out float maxDis);
            Vector2 ranDir = Random.insideUnitCircle;

            // 드물게 0의 길이가 선택되면 정규화 결과를 사용하지않고 기본 방향을 사용합니다
            if (ranDir.sqrMagnitude < 0.0001f)
                ranDir = Vector2.right;
            else
                ranDir.Normalize();

            float ranDis = Random.Range(minDis, maxDis);
            Vector3 candidatePos = spawnCenter + new Vector3(ranDir.x, 0f, ranDir.y) * ranDis;
            bool foundNavMesh = NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, navMeshDistance, NavMesh.AllAreas);

            if (!foundNavMesh)
                continue;

            // 최종 위치도 선택한 중심의 거리 조건을 다시 검사
            Vector3 centerOffset = hit.position - spawnCenter;
            centerOffset.y = 0f;

            float centerDisSqr = centerOffset.sqrMagnitude;
            float minDisSqr = minDis * minDis;
            float maxDisSqr = maxDis * maxDis;

            // 너무 가까운 위치나 너무 먼 곳이라면 위치를 다시 검사
            if (centerDisSqr < minDisSqr || centerDisSqr > maxDisSqr)
                continue;

            spawnPos = hit.position;
            return true;
        }

        // 모든 위치 검사를 실패한다면 기본값을 전달
        spawnPos = Vector3.zero;
        return false;
    }

    // 위치 탐색을 통해 플레이어와 코어 중 어떤 대상을 스폰 중심으로 사용할지 결정
    void SelectSpawnArea(out Vector3 spawnCenter, out float minDis, out float maxDis)
    {
        // 코어 강화 상태가 활성호 되어있고, 코어가 존재하는지 활성화인지 확인하는 과정
        bool canUseCore = isCoreActive && coreCenter != null && coreCenter.gameObject.activeInHierarchy;

        //랜덤 밸류값을 반환하여 확률에 따라 코어중심인지 플레이어 중심인지 선택
        bool spawnAroundCore = canUseCore && Random.value < spawnChance;

        if (spawnAroundCore)
        {
            // 코어 주변 스폰이 결정되었을 때 위치와 거리값을 반환합니다
            spawnCenter = coreCenter.position;
            minDis = coreMinDistance;
            maxDis = coreMaxDistance;
            return;
        }

        // 코어가 없다면 플레이어 위치와 거리값을 반환합니다
        spawnCenter = playerHealth.transform.position;
        minDis = playerMinDistance;
        maxDis = playerMaxDistance;
    }

    // 오브젝트풀에 저장되어있는 비활성 몬스터를 먼저 탐색합니다
    EnemyController GetOrCreateEnemy(Vector3 spawnPos)
    {
        foreach (EnemyController enemy in enemyPool)
        {
            if(enemy == null)
                continue;
            if (enemy.gameObject.activeSelf)
                continue;

            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth == null)
            {
                Debug.LogError( $"{enemy.name}에 Health가 없습니다.",enemy);
                continue;
            }

            // OnEnable보다 먼저 체력과 사망 상태를 복구합니다
            enemyHealth.ResetHealth();

            enemy.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
            enemy.gameObject.SetActive(true);
            return enemy;
        }

        // 재사용 대상이 없을 시 새로운 프리팹을 생성
        EnemyController newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        enemyPool.Add(newEnemy);
        RegisterEnemy(newEnemy);
        return newEnemy;
    }


    // 새로 생성된 몬스터의 사망 이벤트를 연결하는 메소드
    // 최초 생성 시 한번 등록하여 중복을 방지합니다
    void RegisterEnemy(EnemyController enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();

        if (deathHandlers.ContainsKey(enemyHealth))
            return;

        // Ondied 이벤트 자체에는 누가 죽었는지 전달이 되지않으므로 현재 enemy를 기억하는 전용 콜백을 생성
        Action deathHandler = () => HandleEnemyDeath(enemy);
        deathHandlers.Add(enemyHealth, deathHandler);
        enemyHealth.OnDied += deathHandler;
    }

    // 관리하는 몬스터가 사망하면 생존 수를 감소시킨 후
    // 리스폰딜레이 이후 대체 몬스터를 생성하는 코루틴 실행
    void HandleEnemyDeath(EnemyController enemy)
    {
        if (isShuttingDown)
            return;

        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        StartCoroutine(RespawnRoutine());
    }

    // 몬스터 사망 이후 딜레이만큼 기다린 다음 플레이어 또는 코어 주변에서 생성합닏
    IEnumerator RespawnRoutine()
    {
        if(reSpawnDeley > 0f)
           yield return new WaitForSeconds(reSpawnDeley);

        while (!isShuttingDown)
        {
            if (TrySpawnEnemy())
                yield break;

            yield return new WaitForSeconds(1f);
        }
    }

    // 실제 코어가 생성되거나 파괴되었을 때 CoreManager가 호출됩니다
    // 활성화 시 모든 몬스터 강화, 비활성화 시 코어 위치를 제거하고 모든 몬스터를 일반상태로 되돌립니다
    public void SetCoreState(Transform activeCore, bool active)
    {
        bool canActivate = active && activeCore != null && activeCore.gameObject.activeInHierarchy;

        isCoreActive = canActivate;
        coreCenter = isCoreActive ? activeCore : null;

        foreach(EnemyController enemy in enemyPool)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) 
                continue;

            Health enemyHealth = enemy.GetComponent<Health>();

            if (enemyHealth == null || enemyHealth.IsDead)
                continue;

            enemy.SetCoreBoosted(isCoreActive);
        }
    }

    private void OnDestroy()
    {
        isShuttingDown = true;

        foreach(KeyValuePair<Health, Action> pair in deathHandlers)
        {
            if (pair.Key != null)
                pair.Key.OnDied -= pair.Value;
        }

        deathHandlers.Clear();
    }

    // 인스펙터에서 최소, 최대 거리오 몬스터 수가 서로 모순되지않도록 보정
    void OnValidate()
    {
        playerMaxDistance = Mathf.Max(playerMaxDistance, playerMinDistance + 0.1f);
        coreMaxDistance = Mathf.Max(coreMaxDistance, coreMinDistance + 0.1f);
        maxEnemyCount = Mathf.Max(1, maxEnemyCount);
        enemyCount = Mathf.Clamp(enemyCount, 0, maxEnemyCount);
    }
}
