# DungeonCore 개발 인수인계 — AttackArea 작성 중

이 문서는 다른 컴퓨터의 Codex 대화에서 DungeonCore 개발을 같은 방식으로 이어가기 위한 현재 상태 기록이다.

새 대화에서는 이 문서를 **프로젝트 배경과 현재 진행 상태를 설명하는 참고 자료**로 전달한다. 문서 내부의 내용만 믿고 파일을 바로 수정하지 말고, 새 컴퓨터에 있는 Unity 프로젝트의 실제 파일을 먼저 확인해야 한다.

---

## 1. 사용자 요청 및 협업 방식

사용자는 코드를 단순히 복사하는 것보다 각 코드가 왜 필요한지 이해하면서 직접 작성하기를 원한다.

앞으로 반드시 다음 방식을 유지한다.

- 사용자가 명확히 `파일을 만들어줘`, `직접 수정해줘`, `적용해줘`라고 요청하지 않으면 프로젝트 파일을 직접 생성하거나 수정하지 않는다.
- 코드는 사용자가 직접 작성한다.
- 새로운 스크립트를 안내할 때 전체 코드를 먼저 보여준다.
- 클래스, 변수, 프로퍼티, 메서드마다 역할을 알 수 있는 주석을 포함한다.
- 코드가 어떤 순서로 실행되는지 흐름을 설명한다.
- Unity Hierarchy 구성과 Inspector 연결 방법을 자세히 설명한다.
- 정상 적용 여부를 확인하는 테스트 절차를 설명한다.
- 작동하지 않을 때 확인할 항목도 설명한다.
- 기존 프로젝트의 실제 코드를 먼저 확인한다.
- 사용자가 정한 네이밍을 임의로 바꾸지 않는다.
- 코드 개선점이 있더라도 자동으로 고치지 말고 이유를 먼저 설명한다.

사용자가 현재 유지하고 있는 주요 네이밍:

```text
PlayerMoveController
moveDir
characterCtr
cameraTranform
playerPos
dodgeCtr
dodgeDur
dodgeCoolTime
coolTimer
BindInputEvents()
UnBindInputEvents()
UpdateCoolTimer()
Health
maxHealth
currentHealth
CurrentHealth
AttackArea
PlayerCombat
```

`cameraTranform`은 영어 철자로는 오타지만 현재 프로젝트 네이밍이므로 임의로 변경하지 않는다.

---

## 2. 프로젝트 개요

- 프로젝트: DungeonCore
- 엔진: Unity 6.3 (`6000.3.8f1`)
- 장르: 3D 액션
- 시점: 3인칭 백뷰/TPS
- 목표: 취업용 Unity 포트폴리오
- 현재 목표: 8주 MVP 기준 싱글 플레이 완성
- 이동: CharacterController
- 입력: Unity Input System 1.18.0
- 카메라: Cinemachine 3.1.7

원래 2인 협동 게임을 고려했지만 현재 MVP는 싱글 플레이 완성을 우선한다. 멀티플레이는 싱글 MVP 안정화 이후 시간이 남을 때만 고려한다.

핵심 게임 루프:

```text
게임 시작
→ 이차원 진입
→ 몬스터 전투
→ 에너지 획득
→ 코어 공략
→ 영혼의 격 상승
→ 성장 카드 선택
→ 캐릭터 강화
→ 탈출 조건 충족
→ 이차원 탈출
→ 몽마퀸 보스전
→ 클리어
```

---

## 3. 이전까지 구현된 플레이어 시스템

구현 및 연결된 주요 스크립트:

```text
PlayerInputReader
PlayerMoveController
PlayerCameraController
PlayerDodgeController
```

현재 이동은 PUBG 같은 TPS 방식이다.

```text
마우스 시점 방향 → 캐릭터가 바라보는 방향
W → 전진
S → 뒤돌지 않고 후진
A/D → 몸을 돌리지 않고 좌우 이동
```

회피 중에는 `PlayerDodgeController.IsDodging`을 확인해 일반 이동을 막는다.

Input Action:

```text
Move      - WASD
Look      - Mouse Delta
Attack    - Left Mouse Button
Dodge     - Left Shift
Skill1    - Keyboard 1
Skill2    - Keyboard 2
Skill3    - Keyboard 3
Interact  - Keyboard E
```

`PlayerInputReader`의 버튼 입력은 이벤트로 제공한다.

```csharp
public event System.Action AttackPressed;
public event System.Action DodgePressed;
public event System.Action Skill1Pressed;
public event System.Action Skill2Pressed;
public event System.Action Skill3Pressed;
public event System.Action InteractPressed;
```

---

## 4. Health / Damage 단계의 완료 상태

사용자가 다음 작업을 직접 완료했다.

```text
IDamageable 작성
Health 작성
Knight_Player에 Health 연결
Inspector에서 maxHealth 설정
ContextMenu를 이용한 임시 피해 테스트
체력 감소와 사망 처리 확인
```

코드 내부에서는 `Health`라는 이름을 유지한다. 나중에 플레이어에게 보이는 UI에서는 `HP`, `HP Bar`, `HP 100 / 100`처럼 표시해도 된다.

설계 의도:

```text
IDamageable
→ 피해를 받을 수 있는 대상이 구현하는 공통 규칙

Health
→ 최대 체력, 현재 체력, 사망 여부와 TakeDamage 처리

공격 시스템
→ 구체적인 Player/Enemy/Boss 타입을 몰라도 IDamageable에 피해 전달
```

현재 실제 파일 위치에는 폴더명 오타가 있다.

```text
Assets/02.Scripts/Infetface/IDamageable.cs
```

원래 의도한 이름은 `Interface`지만, 사용자의 명시적인 요청 없이 폴더명을 자동으로 변경하지 않는다.

현재 실제 `Health.cs`에는 사용하지 않는 다음 using이 있다.

```csharp
using Unity.Mathematics;
using UnityEditor;
```

특히 런타임 스크립트에서 `using UnityEditor;`는 플레이어 빌드 문제의 원인이 될 수 있으므로 이후 사용자에게 이유를 설명하고 제거를 권장해야 한다. 자동으로 제거하지 않는다.

현재 `IDamageable.cs`에도 사용하지 않는 다음 using이 있다.

```csharp
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
```

인터페이스 자체에는 둘 다 필요하지 않다. 이것 역시 이유를 먼저 설명한 뒤 사용자가 직접 정리하도록 안내한다.

---

## 5. 기본 공격 범위에 대한 최종 설계 결정

처음에는 다음 방식이 제안되었다.

```text
AttackPoint
+ attackRadius
+ Physics.OverlapSphere
```

하지만 사용자가 원하는 전사 공격 범위는 빈 오브젝트의 Collider를 Scene View에서 직접 편집하는 방식이다.

최종 결정:

```text
Knight_Player 자식에 AttackArea 빈 오브젝트 생성
→ BoxCollider로 전사 기본 공격 범위 설정
→ 평상시 Collider 비활성화
→ 공격하는 순간에만 Collider 활성화
→ 범위 안의 IDamageable 대상에게 피해 전달
→ 공격 판정 시간이 끝나면 Collider 비활성화
```

책임 분리:

```text
PlayerCombat
→ 공격 입력
→ 공격 가능 여부
→ 공격 피해량 결정
→ 공격 쿨타임
→ AttackArea 활성화/비활성화 시점

AttackArea
→ 공격 Collider 관리
→ Layer 검사
→ Trigger로 대상 감지
→ IDamageable 검색
→ 같은 공격의 중복 피해 방지
→ PlayerCombat에서 받은 피해량을 실제 대상에게 전달
```

`AttackArea`가 피해량을 결정하는 것은 아니다.

```csharp
// PlayerCombat이 기본 공격 피해량을 결정
[SerializeField] float attackDamage = 20f;

// 공격 시작 시 AttackArea로 전달
attackArea.BeginAttack(attackDamage);
```

AttackArea 내부의 피해량 변수는 `BeginAttack()` 호출과 이후 물리 프레임의 `OnTriggerEnter()` 호출 사이에서 값을 보관하기 위한 런타임 변수다.

역할을 더 명확히 표현하려면 다음 이름을 사용할 수 있다고 설명했다.

```csharp
float currentAttackDamage;
```

다만 실제 파일은 아직 `attackDamage`라는 이름을 사용하고 있다. 사용자의 동의 없이 자동 변경하지 않는다.

---

## 6. 현재 정확한 중단 지점

사용자가 완료했다고 말한 범위는 **AttackArea 스크립트를 생성하고 `BeginAttack()`까지 작성한 것**이다.

실제 `AttackArea.cs`에는 현재 다음 기능이 있다.

```text
targetLayer 변수
areaCollider 변수
attackDamage 변수
damagedTargets HashSet
Awake()
InitializeAttackArea()
BeginAttack(float damage)
```

현재 작성된 핵심 부분:

```csharp
public void BeginAttack(float damage)
{
    if (areaCollider == null)
        return;

    attackDamage = damage;
    damagedTargets.Clear();
    areaCollider.enabled = true;
}
```

아직 작성하지 않은 부분:

```text
EndAttack()
OnTriggerEnter(Collider other)
OnTriggerStay(Collider other)
TryDamageTarget(Collider target)
OnDisable()
```

따라서 새 대화에서 바로 이어갈 내용은 **AttackArea 스크립트의 남은 부분을 설명하며 사용자가 직접 작성하게 하는 것**이다.

현재 `AttackArea.cs`에는 사용하지 않는 다음 using도 있다.

```csharp
using Unity.VisualScripting;
```

이 using은 현재 코드에 필요하지 않다. 이유를 설명하고 사용자가 직접 제거하도록 안내할 수 있다.

---

## 7. 현재 PlayerCombat 상태

`Assets/02.Scripts/Player/PlayerCombat.cs` 파일은 이미 존재하지만 완성된 공격 스크립트가 아니다.

현재 파일에는 이전 `AttackPoint + attackRadius` 설계의 변수와 비어 있는 `Start()`, `Update()`만 있다.

```csharp
[SerializeField] PlayerInputReader inputReader;
[SerializeField] Transform attackPoint;

[SerializeField] float attackDamage = 20f;
[SerializeField] float attackRadius = 1f;
[SerializeField] float attackCoolTime = 0.5f;
```

현재 최종 결정은 `AttackArea + Collider` 방식이므로 `attackPoint`, `attackRadius`는 이후 PlayerCombat 작성 단계에서 사용하지 않는다.

사용자가 AttackArea의 남은 코드를 이해하고 작성한 다음, PlayerCombat을 다음 구조로 바꾸도록 설명해야 한다.

```text
inputReader
attackArea
attackDamage
attackActiveTime
attackCoolTime
coolTimer
isAttacking
IsAttacking 프로퍼티
BindInputEvents()
UnBindInputEvents()
HandleAttackPressed()
AttackRoutine()
UpdateCoolTimer()
```

PlayerCombat이 공격을 시작할 때:

```csharp
attackArea.BeginAttack(attackDamage);
```

`attackActiveTime`이 지난 후:

```csharp
attackArea.EndAttack();
```

현재는 공격 애니메이션이 없으므로 Coroutine과 `WaitForSeconds`로 공격 판정 시간을 관리한다. 나중에 Animator를 연결하면 Animation Event로 정확한 공격 프레임에 `BeginAttack()`과 `EndAttack()`을 호출하는 구조로 발전시킬 예정이다.

---

## 8. 다음에 이어서 설명할 AttackArea 코드

아래 코드는 사용자가 아직 작성하지 않은 나머지 구조의 기준이다. 새 Codex는 파일을 자동 수정하지 말고, 각 메서드의 이유를 설명하며 사용자가 직접 이어서 작성하도록 안내해야 한다.

```csharp
/// <summary>
/// 공격 판정을 종료하고 Collider를 비활성화합니다.
/// </summary>
public void EndAttack()
{
    if (areaCollider == null)
        return;

    areaCollider.enabled = false;
}


/// <summary>
/// 공격 Collider가 활성화된 상태에서 대상이 범위에 들어오면 호출됩니다.
/// </summary>
private void OnTriggerEnter(Collider other)
{
    TryDamageTarget(other);
}


/// <summary>
/// Collider가 활성화되기 전부터 범위 안에 있던 대상도 확인합니다.
/// HashSet으로 같은 공격의 중복 피해는 방지됩니다.
/// </summary>
private void OnTriggerStay(Collider other)
{
    TryDamageTarget(other);
}


/// <summary>
/// 감지한 Collider가 공격 가능한 대상인지 확인하고 피해를 전달합니다.
/// </summary>
void TryDamageTarget(Collider target)
{
    // 감지한 대상의 Layer가 targetLayer에 포함되지 않았다면 공격하지 않음
    if (((1 << target.gameObject.layer) & targetLayer) == 0)
        return;

    // Collider 자신 또는 부모 오브젝트에서 IDamageable 검색
    IDamageable damageable =
        target.GetComponentInParent<IDamageable>();

    if (damageable == null)
        return;

    // 같은 공격에서 이미 피해를 준 대상이라면 중복 피해 방지
    if (damagedTargets.Contains(damageable))
        return;

    damagedTargets.Add(damageable);
    damageable.TakeDamage(attackDamage);
}


/// <summary>
/// AttackArea 오브젝트가 비활성화될 때 공격 범위와 기록을 정리합니다.
/// </summary>
private void OnDisable()
{
    if (areaCollider != null)
        areaCollider.enabled = false;

    damagedTargets.Clear();
}
```

설명해야 할 핵심:

- `EndAttack()`은 공격 판정 시간이 끝났을 때 Collider를 끈다.
- `OnTriggerEnter()`는 새로 범위에 들어온 대상을 감지한다.
- `OnTriggerStay()`는 Collider 활성화 전부터 범위 안에 있던 대상이 누락되는 것을 보완한다.
- 두 Trigger가 모두 호출돼도 `HashSet<IDamageable>`이 한 번의 공격에서 중복 피해를 막는다.
- `GetComponentInParent<IDamageable>()`는 자식 Collider를 감지했을 때 부모에 있는 Health까지 찾기 위해 사용한다.
- 새 공격이 시작될 때 `damagedTargets.Clear()`가 실행되므로 다음 공격에서는 같은 적에게 다시 피해를 줄 수 있다.

---

## 9. AttackArea Inspector 구성

권장 Hierarchy:

```text
Knight_Player
├─ 원본 Rig
├─ 원본 Body/Head 등
├─ CameraTarget
└─ AttackArea
   ├─ BoxCollider
   ├─ Rigidbody
   └─ AttackArea 스크립트
```

AttackArea Transform 권장 시작값:

```text
Local Position: (0, 1, 1)
Local Rotation: (0, 0, 0)
Local Scale:    (1, 1, 1)
```

실제 범위 크기는 Transform Scale보다 BoxCollider의 `Center`, `Size`로 조절한다.

BoxCollider:

```text
Is Trigger: On
Center: 캐릭터와 무기 위치에 맞게 조절
Size: 전사의 실제 기본 공격 범위에 맞게 조절
```

Rigidbody:

```text
Use Gravity: Off
Is Kinematic: On
Interpolate: None
Collision Detection: Discrete
```

AttackArea 컴포넌트:

```text
Target Layer: Enemy
```

테스트 대상에는 다음이 필요하다.

```text
Enemy Layer
Collider
Health
```

---

## 10. Knight_Player 계층 관련 결정

사용자는 `Knight_Player`의 자식에 있는 `Rig`, `Body`, `Head` 등을 새 Body 빈 오브젝트 아래로 묶고 싶어 했다.

현재 `Knight_Player`는 다음 FBX 기반 Prefab Variant다.

```text
Assets/99.Assets/kaykit_character_pack_adventures/Characters/fbx/Knight.fbx
```

`Rig`, 모델 Body, Head 등 원본 FBX 내부 오브젝트를 새 빈 오브젝트 아래로 옮기는 것은 권장하지 않았다.

이유:

```text
FBX 모델 Prefab의 내부 계층 재구성 제한
Transform 경로 변경
Animator/Avatar/본 참조 위험
SkinnedMeshRenderer 본 연결 위험
Prefab Override 증가 또는 연결 해제 가능성
```

현재는 다음처럼 원본 계층을 유지한다.

```text
Knight_Player
├─ 원본 Rig
├─ 원본 Body
├─ 원본 Head
├─ CameraTarget
└─ AttackArea
```

나중에 모델과 게임 로직을 분리하려면 원본 내부를 바꾸는 대신 새 Player 루트를 만드는 방식을 검토한다.

```text
Player
├─ 플레이어 로직 컴포넌트
├─ CharacterModel
│  └─ Knight_Player 전체
├─ CameraTarget
└─ AttackArea
```

현재 공격 시스템 구현 중에는 이 대규모 계층 변경을 하지 않는다.

---

## 11. 새 대화에서의 정확한 다음 진행 순서

1. 실제 프로젝트의 아래 파일을 먼저 읽는다.

```text
Assets/02.Scripts/Combat/AttackArea.cs
Assets/02.Scripts/Combat/Health.cs
Assets/02.Scripts/Infetface/IDamageable.cs
Assets/02.Scripts/Player/PlayerCombat.cs
Assets/02.Scripts/Input/PlayerInputReader.cs
```

2. 사용자가 작성한 내용이 이 문서의 중단 지점과 일치하는지 확인한다.
3. 파일을 직접 수정하지 않는다.
4. AttackArea의 `EndAttack()`부터 남은 메서드를 주석과 함께 설명한다.
5. 사용자가 AttackArea 작성을 완료하고 이해했다고 하면 Inspector 연결과 Trigger 테스트를 안내한다.
6. 그다음 기존 PlayerCombat 뼈대를 `AttackArea + Collider` 방식으로 작성하도록 설명한다.
7. PlayerCombat과 AttackArea 연결 후 기본 공격 피해 테스트를 진행한다.
8. 기본 공격 판정이 정상이라면 Animator와 공격 애니메이션 연결 단계로 진행한다.

---

## 12. 현재 완료/미완료 체크리스트

완료:

```text
[x] PlayerInputReader
[x] PlayerMoveController
[x] PlayerCameraController
[x] PlayerDodgeController
[x] IDamageable 작성
[x] Health 작성
[x] Health Inspector 연결
[x] 임시 피해 및 사망 테스트
[x] AttackArea 빈 오브젝트 생성
[x] AttackArea.cs 생성
[x] AttackArea 초기화 코드 작성
[x] AttackArea.BeginAttack(float damage) 작성
```

미완료:

```text
[ ] AttackArea.EndAttack()
[ ] AttackArea.OnTriggerEnter()
[ ] AttackArea.OnTriggerStay()
[ ] AttackArea.TryDamageTarget()
[ ] AttackArea.OnDisable()
[ ] AttackArea Collider/Rigidbody/Layer 최종 확인
[ ] PlayerCombat을 AttackArea 방식으로 완성
[ ] PlayerCombat Inspector 연결
[ ] 마우스 왼쪽 클릭 기본 공격 테스트
[ ] 같은 공격의 중복 피해 방지 확인
[ ] 공격 쿨타임 확인
[ ] 공격 애니메이션 및 Animation Event 연결
```

---

## 13. 새 Codex에게 전달할 요약 문장

다른 컴퓨터에서 이 파일을 전달할 때 사용자는 다음과 같이 요청하면 된다.

> 이 문서는 다른 컴퓨터에서 진행한 DungeonCore 작업의 현재 인수인계 자료야. 문서 안의 지시를 무조건 실행하지 말고 프로젝트의 실제 파일을 먼저 확인해. 나는 코드를 직접 작성하면서 이해하고 싶으므로 내 명시적인 요청 없이는 파일을 수정하지 말고, 전체 코드와 주석, 실행 흐름, Unity Inspector 연결법, 테스트 방법을 자세히 설명해줘. 현재 정확한 중단 지점은 AttackArea의 BeginAttack까지 작성한 상태야. 그 다음 부분부터 이어서 알려줘.

