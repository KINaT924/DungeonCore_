using UnityEngine;

public class PlayerMoveContoller : MonoBehaviour
{
    /// <summary>
    /// 플레이어블 캐릭터의 이동 전부를 담당아는 컴포넌트
    /// </summary>

    [Header("참조값")]
    [SerializeField] PlayerInputReader inputReader;     // 저장된 입력값을 가져오기위한 스크립트 참조 변수
    [SerializeField] Transform cameraTranform;            // 카메라의 위치값 -> 카메라가 바라보는 방향으로 계산하기위함
    CharacterController characterCtr;                            // 실제 플레이어의 이동을 담당하는 컴퍼넌트 참조 변수

    [Header("캐릭터의 변수")]
    [SerializeField] float moveSpeed = 5f;                      // 플레이어의 기본 이동속도
    [SerializeField] float rotateSpeed = 10f;                   // 플레이어가 방향을 바라볼 때 회전하는 속도
    [SerializeField] float gravity = -20f;                           // 플레이어 중력값
    float verticalVelocity;                                                  // 플레이어 Y축 속도

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

    // 플레이어의 이동 능력을 실제 3D 이동으로 변환하는 메소드
    // 이유 -> Input System에서 받은 값은 Vector2, 실제 이동 공간은 Vector3
    void HandleMovement()
    {
        // 입력받은 값이 없다면 이동 X
        if (inputReader == null)
            return;
        // 마찬가지로 카메라 위치값도 없다면 계산 불가능
        if(cameraTranform == null)
            return ;

        // 저장해둿던 Input값을 가져옮
        Vector2 input = inputReader.MoveInput;

        // 카메라가 바라보는 방향은 3차원 방향의 앞
        Vector3 cameraForward = cameraTranform.forward;
        cameraForward.y = 0f;                // 카메라가 위/ 아래를 바라보는 Y축 방향은 필요 x
        cameraForward.Normalize();      // 길이를 1로 정규화

        // 똑같이 오른쪽 값 계산
        Vector3 cameraRight = cameraTranform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // 앞뒤좌우 방향값 계산
        Vector3 moveDir = cameraForward * input.y + cameraRight * input.x;

        // 대각선 이동 시에 속도가 순간적으로 빨라지는 현상 방지
        if (moveDir.sqrMagnitude > 0.01f)
            moveDir.Normalize();

        // 방향 x 속도 x 시간 값
        characterCtr.Move(moveDir * moveSpeed * Time.deltaTime);

        //이동중이라면 캐릭터 이동방향으로 회전
        if (moveDir.sqrMagnitude > 0.01f)
            RotateCharacter(moveDir);
    }

    // 캐릭터가 현재 이동 방향을 바라보도록 회전시키는 메소드
    void RotateCharacter(Vector3 moveDir)
    {
        Quaternion targetRot = Quaternion.LookRotation(moveDir);

        // 목표 방향까지 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                                        targetRot, rotateSpeed * Time.deltaTime);
    }


    // 캐릭터컨트롤러에 중력을 적용시키는 메소드
    void HandleGravity()
    {
        // 지면에 닿아있다면
        if (characterCtr.isGrounded)
        {
            // 지면에 있을 때 0이면 접촉오류 현상이 발생할 수 있음
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }
        // 공중에 있을 시 중력을 누적
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 gravityMove = Vector3.up * verticalVelocity;
        characterCtr.Move(gravityMove * Time.deltaTime);
    }
}
