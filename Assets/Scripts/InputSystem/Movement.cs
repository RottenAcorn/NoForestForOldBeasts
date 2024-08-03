using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _moveInput;
    private Vector2 _mousePos;
    private InputAction _inputAction;
    private InputAction _aimAction;
    private InputAction _changeAimAction;
    private bool _aimWithMouse = true;
    private ControlActionsConfig _controlActionsConfig;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _controlActionsConfig = ControlActionsConfig.Instance;
        _inputAction = _controlActionsConfig.pc.Move;
        _aimAction = _controlActionsConfig.pc.Aim;
        _changeAimAction = _controlActionsConfig.pc.SwapAimType;
        _inputAction.Enable();
        _aimAction.Enable();
        _changeAimAction.Enable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (GetTransformFromRaycast(out Vector3 output))
        {
            Debug.Log($"Raycast hit: {output}");
        }

        if (_aimWithMouse)
            AimWithMouse();
        else
            Debug.Log("Aim enemies automatically");

        if (_changeAimAction.triggered)
        {
            _aimWithMouse = !_aimWithMouse;
            Debug.Log("Switching aim");
        }


        // How to change binding
        // if (Keyboard.current.enterKey.wasPressedThisFrame)
        // {
        //     _controlActionsConfig.ModifyActionBinding("pc", "Move", "<Keyboard>/upArrow", "<Keyboard>/w");
        // }


    }

    private void AimWithMouse()
    {
        _moveInput = _controlActionsConfig.pc.Move.ReadValue<Vector3>();
        _characterController.Move(new Vector3(_moveInput.x, _moveInput.y, _moveInput.z) * Time.deltaTime);

        _mousePos = _aimAction.ReadValue<Vector2>();

        Vector2 normalizedMousePosition = (_mousePos / new Vector2(Screen.width, Screen.height)) * 2 - Vector2.one;
        Vector3 direction = new Vector3(normalizedMousePosition.x, 0, normalizedMousePosition.y).normalized;

        Quaternion targetRotation = Quaternion.Euler(0f, Quaternion.LookRotation(direction).eulerAngles.y - 90, 0f);
        _characterController.transform.rotation = Quaternion.Slerp(_characterController.transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    bool GetTransformFromRaycast(out Vector3 output)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            output = hitInfo.point;
            return true;
        }

        output = Vector3.zero;
        return false;
    }
}
