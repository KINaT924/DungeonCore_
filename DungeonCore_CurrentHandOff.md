# DungeonCore 개발 인수인계 - 사망 애니메이션 연결 전

이 문서는 다른 컴퓨터의 Codex 대화에서 DungeonCore 개발을 같은 흐름으로 이어가기 위한 최신 인수인계 자료다.

새 대화에서는 이 문서를 참고하되, 문서 내용만 믿고 바로 파일을 수정하지 않는다. 반드시 새 컴퓨터에 있는 Unity 프로젝트의 실제 파일을 먼저 읽고 현재 코드와 Inspector 상태를 확인한 뒤 안내한다.

---

## 1. 협업 방식

사용자는 코드를 단순 복사하기보다 각 코드가 왜 필요한지 이해하면서 직접 작성하기를 원한다.

앞으로 반드시 다음 방식을 유지한다.

- 사용자가 명확히 `파일을 만들어줘`, `직접 수정해줘`, `적용해줘`라고 요청하지 않으면 프로젝트 파일을 직접 생성하거나 수정하지 않는다.
- 기본적으로 Codex는 파일을 읽고, 구조를 분석한 뒤, 사용자가 직접 적용할 수 있게 방향성과 전체 코드를 보여준다.
- 새로운 스크립트나 수정안을 안내할 때는 전체 코드와 변경 지점을 함께 보여준다.
- 클래스, 변수, 프로퍼티, 메소드마다 역할을 알 수 있도록 주석을 자세히 포함한다.
- Unity Inspector에서 무엇을 어디에 연결해야 하는지 구체적으로 설명한다.
- Animator, Animation Event, Collider, Layer, Prefab Override처럼 Unity 에디터에서 확인해야 하는 부분을 빠뜨리지 않는다.
- 정상 적용 여부를 확인하는 테스트 절차와, 작동하지 않을 때 확인할 항목을 같이 설명한다.
- 기존 프로젝트의 실제 코드를 먼저 확인한다.
- 사용자가 정한 네이밍을 임의로 바꾸지 않는다.
- 코드 개선점이 있어도 자동으로 고치지 말고 이유를 먼저 설명한다.

사용자가 유지 중인 주요 네이밍:

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
PlayerAnimationController
```

`cameraTranform`은 영어 철자로는 오타지만 현재 프로젝트 네이밍이므로 임의로 `cameraTransform`으로 바꾸지 않는다.

---

## 2. 프로젝트 개요

- 프로젝트: DungeonCore
- 엔진: Unity 6.3 (`6000.3.8f1`)
- 장르: 3D 액션
- 시점: 3인칭 백뷰/TPS
- 목표: 취업용 Unity 포트폴리오
- 현재 목표: 8주 MVP 기준 싱글 플레이 완성
- 이동: `CharacterController`
- 입력: Unity Input System 1.18.0
- 카메라: Cinemachine 3.1.7

현재 MVP는 싱글 플레이 완성을 우선한다. 멀티플레이는 싱글 MVP 안정화 이후 시간이 남을 때만 고려한다.

핵심 게임 루프:

```text
게임 시작
-> 이차원 진입
-> 몬스터 전투
-> 에너지 획득
-> 코어 공략
-> 영혼의 격 상승
-> 성장 카드 선택
-> 캐릭터 강화
-> 탈출 조건 충족
-> 이차원 탈출
-> 몽마퀸 보스전
-> 클리어
```

---

## 3. 현재 실제 파일 구성

주요 스크립트:

```text
Assets/02.Scripts/Input/PlayerInputReader.cs
Assets/02.Scripts/Player/PlayerMoveController.cs
Assets/02.Scripts/Player/PlayerCameraController.cs
Assets/02.Scripts/Player/PlayerDodgeController.cs
Assets/02.Scripts/Player/PlayerCombat.cs
Assets/02.Scripts/Player/PlayerAnimationController.cs
Assets/02.Scripts/Combat/AttackArea.cs
Assets/02.Scripts/Combat/Health.cs
Assets/02.Scripts/Infetface/IDamageable.cs
```

현재 `IDamageable` 폴더 경로에는 오타가 있다.

```text
Assets/02.Scripts/Infetface/IDamageable.cs
```

원래 의도는 `Interface`지만, 사용자의 명시 요청 없이 폴더명을 자동 변경하지 않는다.

애니메이션 관련 파일:

```text
Assets/03.Art/Animations/PlayerAnimatorController.controller
Assets/03.Art/Animations/KnightAvatar.asset
Assets/03.Art/Animations/Idle_A.anim
Assets/03.Art/Animations/Running_A.anim
Assets/03.Art/Animations/Attack_Slice_Horizontal.anim
Assets/03.Art/Animations/Dodge_Forward.anim
```

KayKit 애니메이션 원본 위치:

```text
Assets/99.Assets/kaykit_Anim/Animations/fbx/Rig_Medium
```

주요 사용 클립:

```text
Idle_A
Running_A
Melee_1H_Attack_Slice_Horizontal
Dodge_Forward
```

나중에 방향별 이동/회피 확장에 쓸 수 있는 클립:

```text
Running_Strafe_Left
Running_Strafe_Right
Walking_Backwards
Dodge_Backward
Dodge_Left
Dodge_Right
```

---

## 4. 현재 완료된 시스템

완료된 흐름:

```text
[x] Input System 입력 구조
[x] PlayerInputReader
[x] Cinemachine 기반 TPS 카메라
[x] 마우스 시점 기준 Player 몸 방향 회전
[x] PUBG식 이동 구조
[x] CharacterController 이동
[x] 회피 시스템
[x] 회피 중 일반 이동 불가
[x] 공격 중 회피 가능
[x] 공격 중 회피 시 공격 캔슬
[x] 회피 중 무적
[x] Health / IDamageable
[x] AttackArea 기본 공격 판정
[x] PlayerCombat 기본 공격 입력/쿨타임/판정
[x] Animator 연결
[x] Idle / Move / Attack 애니메이션 연결
[x] 이동 애니메이션 Loop Time 문제 해결
[x] 공격 Animation Event 연결
[x] 회피 애니메이션 연결
[x] 사망 중 이동/공격/회피/카메라 입력 차단 방향 정리
```

현재 1주차 목표를 `플레이어 기본 조작 + 기본 전투 뼈대`로 보면 90% 이상 완료된 상태다.

---

## 5. 플레이어 이동/카메라 방식

현재 이동은 PUBG 같은 TPS 방식이다.

```text
마우스 시점 방향 -> 캐릭터가 바라보는 방향
W -> 전진
S -> 뒤돌지 않고 후진
A/D -> 몸을 돌리지 않고 좌우 이동
```

현재는 이동 애니메이션이 `Running_A` 하나라 좌우 이동 시 앞으로 뛰는 모션처럼 보일 수 있다. 이는 현재 단계에서는 정상이다. 나중에 `MoveX`, `MoveY`를 이용한 2D Blend Tree로 방향별 이동 모션을 확장한다.

현재 중요한 규칙:

```text
회피 중 일반 이동 불가
공격 중 일반 이동 가능
사망 중 일반 이동 불가
```

`PlayerMoveController`에는 `Health` 참조를 두고 `health.IsDead`일 때 이동을 막는 구조가 들어갔다. 다만 죽은 뒤에도 걷는 것처럼 보인다면 실제 위치 이동인지, Animator의 이동 모션만 반응하는지 먼저 구분해야 한다.

확인 방법:

```text
Play 중 Player 선택
-> Transform Position X/Z 확인
-> WASD를 눌렀을 때 Position이 바뀌면 실제 이동
-> Position은 그대로인데 걷기 모션만 나오면 PlayerAnimationController 문제
```

`PlayerCameraController`에도 `Health` 참조를 두고 죽으면 `LateUpdate()`에서 조기 반환하도록 안내했다. 이렇게 해야 죽은 뒤 마우스 시점/몸 회전도 멈춘다.

---

## 6. 입력 구조

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

`PlayerInputReader`는 입력 값을 저장하고 버튼 입력은 이벤트로 제공한다.

```csharp
public event System.Action AttackPressed;
public event System.Action DodgePressed;
public event System.Action Skill1Pressed;
public event System.Action Skill2Pressed;
public event System.Action Skill3Pressed;
public event System.Action InteractPressed;
```

---

## 7. Health / Damage 상태

`Health`는 `IDamageable`을 구현한다.

역할:

```text
IDamageable
-> 피해를 받을 수 있는 대상이 구현하는 공통 규칙

Health
-> 최대 체력, 현재 체력, 사망 여부, TakeDamage 처리
```

현재 `Health`에는 `PlayerDodgeController dodgeCtr` 참조를 추가해서 회피 중 피해를 무시하도록 했다.

핵심 규칙:

```csharp
if (dodgeCtr != null && dodgeCtr.IsDodging)
    return;
```

주의:

- 이 구조는 플레이어의 `Health`에는 적합하다.
- 나중에 Enemy도 같은 `Health`를 공유한다면 Enemy에는 `dodgeCtr`가 없으므로 그냥 일반 피해 처리된다.
- 장기적으로는 `Invincible`, `DamageReceiver`, `CharacterState` 같은 별도 구조로 분리할 수 있지만 지금은 1주차 범위라 현재 방식이 충분하다.

---

## 8. AttackArea 상태

현재 공격 범위는 `AttackArea` 자식 오브젝트의 `BoxCollider`로 관리한다.

Hierarchy 권장 구조:

```text
Player
├─ CameraTarget
└─ AttackArea
   ├─ BoxCollider
   ├─ Rigidbody
   └─ AttackArea
```

AttackArea 설정:

```text
BoxCollider
-> Is Trigger: On

Rigidbody
-> Use Gravity: Off
-> Is Kinematic: On

AttackArea
-> Target Layer: Enemy
```

현재 `AttackArea`는 다음 역할을 담당한다.

```text
공격 Collider 관리
Layer 검사
Trigger로 대상 감지
IDamageable 검색
같은 공격의 중복 피해 방지
PlayerCombat에서 받은 피해량을 실제 대상에게 전달
```

현재 `TryDamageTarget(Collider target)`에서 사용자가 작성한 방식은:

```csharp
IDamageable damageable = target.GetComponent<IDamageable>();
```

나중에 적의 Collider가 자식 오브젝트에 있고 `Health`가 부모에 있으면 공격이 안 맞을 수 있다. 그 경우 다음으로 바꾸는 것을 추천한다.

```csharp
IDamageable damageable = target.GetComponentInParent<IDamageable>();
```

단, 사용자가 요청하기 전에는 자동 수정하지 않는다.

---

## 9. PlayerCombat 상태

현재 `PlayerCombat`은 기본 공격 입력과 공격 상태를 관리한다.

현재 구조:

```text
inputReader
attackArea
dodgeCtr
health
attackDamage
attackCoolTime
coolTimer
isAttacking
IsAttacking
BindInputEvents()
UnBindInputEvents()
HandleAttackPressed()
StartAttack()
CancelAttack()
EnableAttackArea()
DisableAttackArea()
EndAttack()
UpdateCoolTimer()
```

현재 전투 규칙:

```text
회피 중 공격 불가
공격 중 회피 가능
공격 중 회피하면 공격 캔슬
공격 중 일반 이동 가능
사망 중 공격 불가
```

공격 판정은 더 이상 코루틴 시간으로 켜고 끄지 않는다. 현재는 Animation Event에서 아래 메소드를 호출한다.

```csharp
public void EnableAttackArea()
public void DisableAttackArea()
public void EndAttack()
```

공격 애니메이션 이벤트 구성:

```text
Melee_1H_Attack_Slice_Horizontal
-> 검 휘두르기 시작: EnableAttackArea
-> 검 휘두르기 끝: DisableAttackArea
-> 공격 모션 끝부분: EndAttack
```

공격 중 회피 캔슬:

```csharp
public void CancelAttack()
{
    isAttacking = false;
    DisableAttackArea();
}
```

`PlayerDodgeController.StartDodge()` 시작 부분에서 `playerCombat.CancelAttack()`을 호출하도록 안내했고, 사용자가 적용 및 테스트 완료했다.

---

## 10. PlayerDodgeController 상태

현재 `PlayerDodgeController`는 회피 입력, 회피 방향, 회피 이동, 쿨타임을 관리한다.

현재 구조:

```text
inputReader
cameraTranform
playerCombat
health
characterCtr
dodgeSpeed
dodgeDur
dodgeCoolTime
dodgeDir
dodgeTimer
coolTimer
isDodging
IsDodging
BindInputEvents()
UnBindInputEvents()
HandleDodgePressed()
StartDodge()
CalculateDodgeDirection()
UpdateCoolTimer()
UpdateDodge()
EndDodge()
```

현재 규칙:

```text
사망 중 회피 불가
회피 중 재회피 불가
회피 쿨타임 중 회피 불가
회피 시작 시 공격 중이면 공격 취소
```

Inspector 추천값:

```text
dodgeSpeed: 12 ~ 18
dodgeDur: 0.35 ~ 0.45
dodgeCoolTime: 0.8
```

현재 테스트 중 사용한 값:

```text
dodgeSpeed: 15
dodgeDur: 0.4
dodgeCoolTime: 0.8
```

---

## 11. PlayerAnimationController 상태

현재 `PlayerAnimationController`는 실제 이동/공격/회피를 처리하지 않고 Animator 파라미터만 갱신한다.

역할:

```text
PlayerInputReader.MoveInput 읽기
PlayerDodgeController.IsDodging 읽기
PlayerCombat.IsAttacking 읽기
Animator 파라미터 갱신
```

현재 Animator 파라미터:

```text
Float   MoveX
Float   MoveY
Float   MoveAmount
Bool    IsMoving
Bool    IsDodging
Bool    IsAttacking
Trigger Attack
```

최근 문제:

죽은 뒤 WASD를 누르면 실제 위치 이동은 막혀도 걷는 모션이 나와서 이동하는 것처럼 보일 수 있었다. 원인은 `PlayerAnimationController`가 죽은 상태를 모르고 계속 `MoveInput`을 Animator에 전달하기 때문이다.

해결 방향:

```text
PlayerAnimationController에 Health 참조 추가
죽었을 때 이동/회피/공격 파라미터를 false 또는 0으로 초기화
다음 단계에서 IsDead 파라미터까지 추가해 Death 애니메이션으로 전환
```

현재 안내한 핵심 코드:

```csharp
if (health != null && health.IsDead)
{
    animator.SetFloat(moveXHash, 0f);
    animator.SetFloat(moveYHash, 0f);
    animator.SetFloat(moveAmountHash, 0f);
    animator.SetBool(isMovingHash, false);
    animator.SetBool(isDodgingHash, false);
    animator.SetBool(isAttackingHash, false);
    return;
}
```

다음 대화에서는 실제 `PlayerAnimationController.cs`를 먼저 읽고 이 내용이 반영되어 있는지 확인해야 한다.

---

## 12. Animator / Animation 현재 상태

현재 Animator Controller:

```text
Assets/03.Art/Animations/PlayerAnimatorController.controller
```

Player의 Animator:

```text
Controller: PlayerAnimatorController
Avatar: KnightAvatar.asset
Apply Root Motion: Off
```

기본 상태:

```text
Idle_A
Running_A
Attack_Slice_Horizontal
Dodge
```

기본 전환:

```text
Idle_A -> Running_A
Condition: IsMoving == true
Has Exit Time: Off

Running_A -> Idle_A
Condition: IsMoving == false
Has Exit Time: Off

Any State -> Attack_Slice_Horizontal
Condition: Attack Trigger
Has Exit Time: Off

Attack_Slice_Horizontal -> Idle_A
Has Exit Time: On
Condition 없음

Any State -> Dodge
Condition: IsDodging == true
Has Exit Time: Off

Dodge -> Idle_A
Condition: IsDodging == false
Has Exit Time: Off
```

Loop 설정:

```text
Idle_A: Loop Time On
Running_A: Loop Time On
Attack_Slice_Horizontal: Loop Time Off
Dodge_Forward: Loop Time Off
```

회피가 공격을 덮어쓰지 않으면 Animator에 아래 전환을 추가한다.

```text
Attack_Slice_Horizontal -> Dodge
Condition: IsDodging == true
Has Exit Time: Off
Transition Duration: 0.05
```

---

## 13. 현재 정확한 중단 지점

현재 사용자는 다음까지 완료하고 테스트했다.

```text
공격 Animation Event 연결 완료
공격 판정 테스트 완료
회피 애니메이션 연결 완료
회피 테스트 완료
공격 중 회피 캔슬 완료
회피 중 무적 완료
회피 중 공격 불가 / 회피 중 이동 불가 / 공격 중 이동 가능 / 사망 중 모든 입력 불가 규칙 적용 진행
죽은 뒤 WASD가 동작하는 것처럼 보이는 문제를 확인
PlayerAnimationController에 사망 체크가 필요하다는 원인 파악
```

다음 대화에서 바로 시작할 작업:

```text
사망 애니메이션 연결
```

정확한 시작점:

```text
1. 실제 PlayerAnimationController.cs를 읽는다.
2. Health 참조와 사망 시 Animator 파라미터 초기화가 반영되어 있는지 확인한다.
3. Animator에 Bool IsDead 파라미터를 추가하도록 안내한다.
4. Death_A / Death_A_Pose 클립을 연결하도록 안내한다.
5. PlayerAnimationController에서 Health.IsDead를 Animator의 IsDead로 전달하는 전체 코드를 주석과 함께 보여준다.
6. 사용자가 직접 적용한 뒤 Inspector 연결과 테스트 절차를 안내한다.
```

---

## 14. 다음에 안내할 사망 애니메이션 방향

KayKit 사망 애니메이션 원본:

```text
Assets/99.Assets/kaykit_Anim/Animations/fbx/Rig_Medium/Rig_Medium_General.fbx
```

사용 후보:

```text
Death_A
Death_B
Death_A_Pose
Death_B_Pose
```

처음 추천:

```text
Death_A -> 죽는 순간 재생되는 모션
Death_A_Pose -> 죽은 뒤 유지되는 포즈
```

Animator 파라미터 추가:

```text
Bool IsDead
```

Animator 전환:

```text
Any State -> Death
Condition: IsDead == true
Has Exit Time: Off
Transition Duration: 0.05

Death -> DeathPose
Has Exit Time: On
Exit Time: 0.95
Condition 없음
```

Loop 설정:

```text
Death_A: Loop Time Off
Death_A_Pose: 보통 Loop Time Off 또는 On 모두 큰 문제 없음
```

`PlayerAnimationController` 수정 방향:

```text
Health health 참조 추가
int isDeadHash 추가
Awake에서 isDeadHash = Animator.StringToHash("IsDead")
Update에서 죽었으면 IsDead true, 이동/공격/회피 파라미터 초기화
살아있으면 IsDead false 후 기존 이동/회피/공격 갱신
```

사망 상태에서 기대 흐름:

```text
Health.TakeDamage()
-> currentHealth <= 0
-> Die()
-> isDead = true
-> PlayerMoveController 이동 중단
-> PlayerCameraController 시점 회전 중단
-> PlayerCombat 공격 중단
-> PlayerDodgeController 회피 중단
-> PlayerAnimationController IsDead true
-> Death_A 재생
-> Death_A_Pose 유지
```

---

## 15. 다음 대화에서 먼저 읽어야 할 파일

새 컴퓨터에서 이어갈 때 Codex는 우선 아래 파일을 읽는다.

```text
Assets/02.Scripts/Player/PlayerAnimationController.cs
Assets/02.Scripts/Player/PlayerMoveController.cs
Assets/02.Scripts/Player/PlayerCameraController.cs
Assets/02.Scripts/Player/PlayerCombat.cs
Assets/02.Scripts/Player/PlayerDodgeController.cs
Assets/02.Scripts/Combat/Health.cs
Assets/02.Scripts/Combat/AttackArea.cs
Assets/03.Art/Animations/PlayerAnimatorController.controller
```

그 다음 현재 Inspector 연결 상태를 프리팹 또는 씬 파일 기준으로 확인한다.

```text
Assets/03.Art/Prefabs/Player.prefab
Assets/01.Scenes/SampleScene.unity
```

---

## 16. 새 Codex에게 전달할 요약 문장

다른 컴퓨터에서 이 파일을 전달할 때 사용자는 다음처럼 요청하면 된다.

> 이 문서는 DungeonCore 작업의 최신 인수인계 자료야. 문서 내용을 참고하되 실제 프로젝트 파일을 먼저 읽고 확인해. 나는 코드를 직접 작성하면서 이해하고 싶으니, 내가 명확히 요청하지 않으면 파일을 직접 수정하지 말고 전체 코드, 변경 지점, 클래스/변수/메소드 주석, 실행 흐름, Unity Inspector 연결법, Animator 설정법, 테스트 방법을 자세히 설명해줘. 현재 중단 지점은 공격/회피/무적/상태 규칙 테스트 이후이며, 다음 작업은 사망 애니메이션 연결부터 시작하면 돼.
