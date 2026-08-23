using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    /// <summary>
    ///  Input System을 활용하여 입력처리를 별도의 컴포넌트로 분리하였습니다
    ///  이를 통해 플레이어의 이동과 전투 시스템이 입력 장치에 의해 직접적인 의존을 하지않아도 되도록 구성됩니다
    /// </summary>

    // 플레이어의 이동 입력 값입니다
    Vector2 moveInput;

    // 플레이어의 마우스 이동 입력값입니다
    // Cinemachine 카메라의 제어 및 플레이어의 바라보는 방향입니다
    Vector2 lookInput;


    // 프로퍼티를 사용하여 자기 자신의 입력 데이터를 관리할 수 있도록 설정
    public Vector2 MoveInput { get { return moveInput; } }
    public Vector2 LookInput { get {return lookInput; } }


    // 버튼을 눌렀을 시에 발생하는 이벤트를 실행하여
    // 정해진 메소드를 실행하지않고 각자의 이벤트를 실행합니다
    public event System.Action AttackPressed;
    public event System.Action DodgePressed;
    public event System.Action Skill1Pressed;
    public event System.Action Skill2Pressed;
    public event System.Action Skill3Pressed;
    public event System.Action InteractPressed;

    // 실제 InputAction으로부터 호출되는 메소드들입니다
    // 값이 변경된다면 InputAction.CallbackContext가 대신 전달됩니다
    public void OnMove(InputAction.CallbackContext context)         // 현재값 WASD
    {
        // Move 입력이 실제로 실행되고 있을 경우
        if(context.performed)
            moveInput = context.ReadValue<Vector2>();

        // 키 입력을 놓아서 움직이는 것이 취소가 됬을 경우
        if (context.canceled)
            moveInput = Vector2.zero;

        //Debug.Log($"Move Input : {moveInput}");
    }

    public void OnLook(InputAction.CallbackContext context)         // 현재값 마우스 좌표
    {
        // Mouse Delta 값이 제대로 실행되고 있을 경우
        if(context.performed)
            lookInput = context.ReadValue<Vector2>();

        // 취소가 되었을 경우
        if(context.canceled)
            lookInput = Vector2.zero;
    }

    public void OnAttack(InputAction.CallbackContext context)       // 현재값 좌클릭
    {
        // Attack값은 실행중이 아니라면 의미가 없기에 아닐경우 반환해버립니다
        if (!context.performed)
            return;

        // 대상이 존재하는가?를 물어보고 맞다면 실행합니다
        AttackPressed?.Invoke();
    }

    public void OnDodge(InputAction.CallbackContext context)        // 현재값 왼쪽 쉬프트
    {
        // 기본적으로 공격과 같습니다
        if (!context.performed)
            return;

        DodgePressed?.Invoke();
    }

    public void OnSkill1(InputAction.CallbackContext context)       // 현재값 1
    {
        // 기본적으로 공격과 같습니다
        if (!context.performed)
            return;

        Skill1Pressed?.Invoke();
    }
    public void OnSkill2(InputAction.CallbackContext context)       // 현재값 2
    {
        // 기본적으로 공격과 같습니다
        if (!context.performed)
            return;

        Skill2Pressed?.Invoke();
    }
    public void OnSkill3(InputAction.CallbackContext context)       // 현재값 3
    {
        // 기본적으로 공격과 같습니다
        if (!context.performed)
            return;

        Skill3Pressed?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)     // 현재값 E
    {
        // 기본적으로 공격과 같습니다 -> 이 메소드는 상호작용을 위한 입력값입니다
        if (!context.performed)
            return;

        InteractPressed?.Invoke();
    }
}
