using UnityEngine;
using UnityEngine.XR;

public class OVRGamepadController : MonoBehaviour
{
	public enum Axis
	{
		LeftXAxis = 0,
		LeftYAxis = 1,
		RightXAxis = 2,
		RightYAxis = 3,
		LeftTrigger = 4,
		RightTrigger = 5
	}

	public enum Button
	{
		A = 0,
		B = 1,
		X = 2,
		Y = 3,
		Up = 4,
		Down = 5,
		Left = 6,
		Right = 7,
		Start = 8,
		Back = 9,
		LStick = 10,
		RStick = 11,
		L1 = 12,
		R1 = 13
	}

	private static bool GPC_Available = true;

	public static bool GPC_Initialize()
	{
		GPC_Available = true;
		return true;
	}

	public static bool GPC_Destroy()
	{
		GPC_Available = false;
		return true;
	}

	public static bool GPC_Update()
	{
		string[] joysticks = AsunderInput.GetJoystickNames();
		GPC_Available = (joysticks != null && joysticks.Length > 0) || XRSettings.isDeviceActive;
		return GPC_Available;
	}

	public static float GPC_GetAxis(int axis)
	{
		InputDevice left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		InputDevice right = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

		switch ((Axis)axis)
		{
		case Axis.LeftXAxis:
			if (left.isValid && left.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 lStickX) && Mathf.Abs(lStickX.x) > 0.05f)
			{
				return lStickX.x;
			}
			return AsunderInput.GetAxis("Horizontal");
		case Axis.LeftYAxis:
			if (left.isValid && left.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 lStickY) && Mathf.Abs(lStickY.y) > 0.05f)
			{
				return lStickY.y;
			}
			return AsunderInput.GetAxis("Vertical");
		case Axis.RightXAxis:
			if (right.isValid && right.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rStickX) && Mathf.Abs(rStickX.x) > 0.05f)
			{
				return rStickX.x;
			}
			return 0f;
		case Axis.RightYAxis:
			if (right.isValid && right.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rStickY) && Mathf.Abs(rStickY.y) > 0.05f)
			{
				return rStickY.y;
			}
			return 0f;
		case Axis.LeftTrigger:
			if (left.isValid && left.TryGetFeatureValue(CommonUsages.trigger, out float lt))
			{
				return lt;
			}
			return 0f;
		case Axis.RightTrigger:
			if (right.isValid && right.TryGetFeatureValue(CommonUsages.trigger, out float rt))
			{
				return rt;
			}
			return 0f;
		default:
			return 0f;
		}
	}

	public static bool GPC_GetButton(int button)
	{
		InputDevice left = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		InputDevice right = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

		switch ((Button)button)
		{
		case Button.A:
			return AsunderInput.GetKey(KeyCode.JoystickButton0) || (right.isValid && right.TryGetFeatureValue(CommonUsages.primaryButton, out bool a) && a);
		case Button.B:
			return AsunderInput.GetKey(KeyCode.JoystickButton1) || (right.isValid && right.TryGetFeatureValue(CommonUsages.secondaryButton, out bool b) && b);
		case Button.X:
			return AsunderInput.GetKey(KeyCode.JoystickButton2) || (left.isValid && left.TryGetFeatureValue(CommonUsages.primaryButton, out bool x) && x);
		case Button.Y:
			return AsunderInput.GetKey(KeyCode.JoystickButton3) || (left.isValid && left.TryGetFeatureValue(CommonUsages.secondaryButton, out bool y) && y);
		case Button.Start:
			return AsunderInput.GetKey(KeyCode.JoystickButton7) || (left.isValid && left.TryGetFeatureValue(CommonUsages.menuButton, out bool m1) && m1) || (right.isValid && right.TryGetFeatureValue(CommonUsages.menuButton, out bool m2) && m2);
		case Button.Back:
			return AsunderInput.GetKey(KeyCode.JoystickButton6);
		case Button.LStick:
			return AsunderInput.GetKey(KeyCode.JoystickButton8) || (left.isValid && left.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool ls) && ls);
		case Button.RStick:
			return AsunderInput.GetKey(KeyCode.JoystickButton9) || (right.isValid && right.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool rs) && rs);
		case Button.L1:
			return AsunderInput.GetKey(KeyCode.JoystickButton4) || (left.isValid && left.TryGetFeatureValue(CommonUsages.gripButton, out bool lg) && lg);
		case Button.R1:
			return AsunderInput.GetKey(KeyCode.JoystickButton5) || (right.isValid && right.TryGetFeatureValue(CommonUsages.gripButton, out bool rg) && rg);
		default:
			return false;
		}
	}

	public static bool GPC_IsAvailable()
	{
		return GPC_Available;
	}

	private void Awake()
	{
	}

	private void Start()
	{
		GPC_Available = GPC_Initialize();
	}

	private void Update()
	{
		GPC_Available = GPC_Update();
	}

	private void OnDestroy()
	{
		GPC_Destroy();
	}
}
