using UnityEngine;
using UnityEngine.InputSystem;

public class TestNewUnityInputSystem : MonoBehaviour
{
    public InputAction TestAction;
    public InputAction TestAxis;
    public bool ActionDebug;
    public float DebugAxis;
    private void Start()
    {
        TestAction.Enable();
        TestAxis.Enable();
    }

    private void Update()
    {
        ActionDebug = TestAction.IsPressed();
        DebugAxis = TestAxis.ReadValue<float>();
    }
}
