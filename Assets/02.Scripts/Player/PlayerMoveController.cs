using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    /// <summary>
    /// 플레이어의 캐릭터 이동 처리를 담당하는 컴포넌트
    /// </summary>

    [Header("레퍼런스")]
    [SerializeField] PlayerInputReader inputReader;              // 플레이어 입력값을 전달해주는 스크립트 참조 변수
    [SerializeField] PlayerDodgeController dodgeCtr;             // 플레이어의 회피 동작을 처리하는 컴포넌트 참조 변수
    [SerializeField] Transform cameraTranform;                   // 카메라의 위치값 -> 카메라 바라보는 방향으로 움직이기위해
    CharacterController characterCtr;                            // 실제 플레이어의 이동을 처리하는 컴포넌트 참조 변수

    [Header("캐릭터 설정")]
    [SerializeField] float moveSpeed = 5f;                       // 플레이어의 기본 이동속도
    [SerializeField] float gravity = -20f;                       // 플레이어 중력값
    float verticalVelocity;                                      // 플레이어 Y축 속도

    private void Awake()
    {
        characterCtr = GetComponent<CharacterController>();
    }


    void Start()
    {
        
    }


    void Update()
    {
        HandleMovement();
        HandleGravity();
    }

    // 플레이어의 이동 입력값을 WASD 입력에 따라 카메라 기준 이동 방향으로 변환
    // 입력 -> Input System에서 받은 2차원 Vector2을 활용
    void HandleMovement()
    {
        // 입력값을 받을 수 없다면 이동 X
        if (inputReader == null)
            return;
        if(cameraTranform == null)
            return ;
        if(characterCtr == null)
            return;
        if (dodgeCtr != null && dodgeCtr.IsDodging)
            return;


        // 전달받은 Input값 가져오기
        Vector2 input = inputReader.MoveInput;

        // 카메라 바라보는 방향을 정규화하여 3차원 방향값 얻기
        Vector3 cameraForward = cameraTranform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // 똑같은 방식의 좌우 방향
        Vector3 cameraRight = cameraTranform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // 앞뒤좌우 방향값 계산
        Vector3 moveDir = cameraForward * input.y + cameraRight * input.x;

        // 대각선 이동 시에 속도가 빨라지는걸 방지
        if (moveDir.sqrMagnitude > 0.01f)
            moveDir.Normalize();

        // 방향 x 속도 x 시간 값
        characterCtr.Move(moveDir * moveSpeed * Time.deltaTime);
    }


    // 캐릭터컨트롤러에 중력을 적용시키는 메소드
    void HandleGravity()
    {
        // 바닥에 붙어있다면
        if (characterCtr.isGrounded)
        {
            // 바닥에 붙어 있을 때 0이면 지면에서 떨어지는 현상이 발생할 수 있음
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }
        // 공중에 있을 때 중력을 누적
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 gravityMove = Vector3.up * verticalVelocity;
        characterCtr.Move(gravityMove * Time.deltaTime);
    }
}
