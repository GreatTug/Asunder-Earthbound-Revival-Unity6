using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class OVRPlayerController : OVRComponent
{
	protected CharacterController Controller;

	protected OVRCameraController CameraController;

	public float Acceleration = 0.1f;

	public float Damping = 0.15f;

	public float BackAndSideDampen = 0.5f;

	public float JumpForce = 0.3f;

	public float RotationAmount = 1.5f;

	public float GravityModifier = 0.379f;

	private float MoveScale = 1f;

	private Vector3 MoveThrottle = Vector3.zero;

	private float FallSpeed;

	private Quaternion OrientationOffset = Quaternion.identity;

	private float YRotation;

	protected Transform DirXform;

	private float MoveScaleMultiplier = 1f;

	private float RotationScaleMultiplier = 1f;

	private bool AllowMouseRotation = true;

	private bool HaltUpdateMovement;

	private float YfromSensor2;

	private static float sDeltaRotationOld;

	public new virtual void Awake()
	{
		base.Awake();
		Controller = base.gameObject.GetComponent<CharacterController>();
		if (Controller == null)
		{
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");
		}
		OVRCameraController[] componentsInChildren = base.gameObject.GetComponentsInChildren<OVRCameraController>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("OVRPlayerController: No OVRCameraController attached.");
		}
		else if (componentsInChildren.Length > 1)
		{
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraController attached.");
		}
		else
		{
			CameraController = componentsInChildren[0];
		}
		DirXform = null;
		Transform[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			if (componentsInChildren2[i].name == "ForwardDirection")
			{
				DirXform = componentsInChildren2[i];
				break;
			}
		}
		if (DirXform == null)
		{
			Debug.LogWarning("OVRPlayerController: ForwardDirection game object not found. Do not use.");
		}
	}

	public new virtual void Start()
	{
		base.Start();
		InitializeInputs();
		SetCameras();
	}

	public new virtual void Update()
	{
		base.Update();
		if (OVRDevice.SensorCount == 2)
		{
			Quaternion q = Quaternion.identity;
			OVRDevice.GetPredictedOrientation(1, ref q);
			YfromSensor2 = q.eulerAngles.y;
		}
		UpdateMovement();
		Vector3 zero = Vector3.zero;
		float num = 1f + Damping * DeltaTime;
		MoveThrottle.x /= num;
		MoveThrottle.y = ((!(MoveThrottle.y > 0f)) ? MoveThrottle.y : (MoveThrottle.y / num));
		MoveThrottle.z /= num;
		zero += MoveThrottle * DeltaTime;
		if (Controller.isGrounded && FallSpeed <= 0f)
		{
			FallSpeed = Physics.gravity.y * (GravityModifier * 0.002f);
		}
		else
		{
			FallSpeed += Physics.gravity.y * (GravityModifier * 0.002f) * DeltaTime;
		}
		zero.y += FallSpeed * DeltaTime;
		float num2 = 0f;
		if (Controller.isGrounded && MoveThrottle.y <= 0.001f)
		{
			num2 = Mathf.Max(Controller.stepOffset, new Vector3(zero.x, 0f, zero.z).magnitude);
			zero -= num2 * Vector3.up;
		}
		Vector3 vector = Vector3.Scale(Controller.transform.localPosition + zero, new Vector3(1f, 0f, 1f));
		Controller.Move(zero);
		Vector3 vector2 = Vector3.Scale(Controller.transform.localPosition, new Vector3(1f, 0f, 1f));
		if (vector != vector2)
		{
			MoveThrottle += (vector2 - vector) / DeltaTime;
		}
		UpdatePlayerForwardDirTransform();
	}

	public virtual void UpdateMovement()
	{
		if (HaltUpdateMovement)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		MoveScale = 1f;
		if (AsunderInput.GetKey(KeyCode.W))
		{
			flag = true;
		}
		if (AsunderInput.GetKey(KeyCode.A))
		{
			flag2 = true;
		}
		if (AsunderInput.GetKey(KeyCode.S))
		{
			flag4 = true;
		}
		if (AsunderInput.GetKey(KeyCode.D))
		{
			flag3 = true;
		}
		if (AsunderInput.GetKey(KeyCode.UpArrow))
		{
			flag = true;
		}
		if (AsunderInput.GetKey(KeyCode.LeftArrow))
		{
			flag2 = true;
		}
		if (AsunderInput.GetKey(KeyCode.DownArrow))
		{
			flag4 = true;
		}
		if (AsunderInput.GetKey(KeyCode.RightArrow))
		{
			flag3 = true;
		}
		if ((flag && flag2) || (flag && flag3) || (flag4 && flag2) || (flag4 && flag3))
		{
			MoveScale = 0.70710677f;
		}
		if (!Controller.isGrounded)
		{
			MoveScale = 0f;
		}
		MoveScale *= DeltaTime;
		float num = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;
		if (AsunderInput.GetKey(KeyCode.LeftShift) || AsunderInput.GetKey(KeyCode.RightShift))
		{
			num *= 2f;
		}
		if (DirXform != null)
		{
			if (flag)
			{
				MoveThrottle += DirXform.TransformDirection(Vector3.forward * num);
			}
			if (flag4)
			{
				MoveThrottle += DirXform.TransformDirection(Vector3.back * num) * BackAndSideDampen;
			}
			if (flag2)
			{
				MoveThrottle += DirXform.TransformDirection(Vector3.left * num) * BackAndSideDampen;
			}
			if (flag3)
			{
				MoveThrottle += DirXform.TransformDirection(Vector3.right * num) * BackAndSideDampen;
			}
		}
		float num2 = DeltaTime * RotationAmount * RotationScaleMultiplier;
		if (AsunderInput.GetKey(KeyCode.Q))
		{
			YRotation -= num2 * 0.5f;
		}
		if (AsunderInput.GetKey(KeyCode.E))
		{
			YRotation += num2 * 0.5f;
		}
		float num3 = 0f;
		if (!AllowMouseRotation)
		{
			num3 = AsunderInput.GetAxis("Mouse X") * num2 * 3.25f;
		}
		float num4 = sDeltaRotationOld * 0f + num3 * 1f;
		YRotation += num4;
		sDeltaRotationOld = num4;
		num = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;
		num *= 1f + OVRGamepadController.GPC_GetAxis(4);
		if (DirXform != null)
		{
			float num5 = OVRGamepadController.GPC_GetAxis(1);
			float num6 = OVRGamepadController.GPC_GetAxis(0);
			if (num5 > 0f)
			{
				MoveThrottle += num5 * DirXform.TransformDirection(Vector3.forward * num);
			}
			if (num5 < 0f)
			{
				MoveThrottle += Mathf.Abs(num5) * DirXform.TransformDirection(Vector3.back * num) * BackAndSideDampen;
			}
			if (num6 < 0f)
			{
				MoveThrottle += Mathf.Abs(num6) * DirXform.TransformDirection(Vector3.left * num) * BackAndSideDampen;
			}
			if (num6 > 0f)
			{
				MoveThrottle += num6 * DirXform.TransformDirection(Vector3.right * num) * BackAndSideDampen;
			}
		}
		float num7 = OVRGamepadController.GPC_GetAxis(2);
		YRotation += num7 * num2;
		SetCameras();
	}

	public virtual void UpdatePlayerForwardDirTransform()
	{
		if (DirXform != null && CameraController != null)
		{
			Quaternion identity = Quaternion.identity;
			identity = Quaternion.Euler(0f, YfromSensor2, 0f);
			DirXform.rotation = identity * CameraController.transform.rotation;
		}
	}

	public bool Jump()
	{
		if (!Controller.isGrounded)
		{
			return false;
		}
		MoveThrottle += new Vector3(0f, JumpForce, 0f);
		return true;
	}

	public void Stop()
	{
		Controller.Move(Vector3.zero);
		MoveThrottle = Vector3.zero;
		FallSpeed = 0f;
	}

	public void InitializeInputs()
	{
		OrientationOffset = base.transform.rotation;
		YRotation = 0f;
	}

	public void SetCameras()
	{
		if (CameraController != null)
		{
			CameraController.SetOrientationOffset(OrientationOffset);
			CameraController.SetYRotation(YRotation);
		}
	}

	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = MoveScaleMultiplier;
	}

	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		MoveScaleMultiplier = moveScaleMultiplier;
	}

	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = RotationScaleMultiplier;
	}

	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		RotationScaleMultiplier = rotationScaleMultiplier;
	}

	public void GetAllowMouseRotation(ref bool allowMouseRotation)
	{
		allowMouseRotation = AllowMouseRotation;
	}

	public void SetAllowMouseRotation(bool allowMouseRotation)
	{
		AllowMouseRotation = allowMouseRotation;
	}

	public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
	{
		haltUpdateMovement = HaltUpdateMovement;
	}

	public void SetHaltUpdateMovement(bool haltUpdateMovement)
	{
		HaltUpdateMovement = haltUpdateMovement;
	}
}
