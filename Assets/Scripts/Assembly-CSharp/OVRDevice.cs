using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.InputSystem;

using XRDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

public static class AsunderInput
{
	private static int _lastPolledFrame = -1;

	private static XRDevice _rightHandDevice;
	private static XRDevice _leftHandDevice;
	private static XRDevice _headDevice;

	public static XRNode ActivePointerHand = XRNode.RightHand;

	private static bool _prevXrInteract;
	private static bool _xrInteractDown;
	private static bool _xrInteractUp;

	private static bool _prevXrMenu;
	private static bool _xrMenuDown;
	private static bool _xrMenuUp;

	private static bool _prevXrRecenter;
	private static bool _xrRecenterDown;
	private static bool _xrRecenterUp;

	private static bool _prevXrGrip;
	private static bool _xrGripDown;

	private static bool _prevNodState;
	private static bool _nodStickDown;
	private static bool _prevShakeState;
	private static bool _shakeStickDown;

	private static float _recenterYawOffset;
	private static Vector3 _recenterPosOffset = Vector3.zero;
	private static bool _hasInitialRecenter;

	private static float _snapTurnYawOffset;
	private static bool _prevSnapTurnStick;

	public static float SnapTurnYawOffset => _snapTurnYawOffset;

	public static void SnapTurn(float angle)
	{
		_snapTurnYawOffset = Mathf.Repeat(_snapTurnYawOffset + angle, 360f);
		TriggerHaptic(XRNode.RightHand, 0.25f, 0.03f);
	}

	public static void ResetSnapTurn()
	{
		_snapTurnYawOffset = 0f;
	}

	private static float _fallbackYaw;
	private static float _fallbackPitch;

	private static readonly RaycastHit[] _rayHitsBuffer = new RaycastHit[32];
	private static readonly RaycastHit[] _sphereHitsBuffer = new RaycastHit[32];

	public static bool IsOpenXRActive => (AppManager.Instance != null && AppManager.Instance._ApplicationType == AppManager.AppType.PC) ? false : XRSettings.isDeviceActive;

	/// <summary>True while a tracked VR controller is being used as a pointing ray (menus, or gameplay after a grip toggle).</summary>
	public static bool PointerActive;

	public static Ray PointerRay;

	public static bool TryGetAimRay(out Ray ray)
	{
		ray = PointerRay;
		return PointerActive;
	}

	public static void TriggerHaptic(XRNode hand, float amplitude = 0.5f, float duration = 0.05f)
	{
		if (!XRSettings.isDeviceActive) return;
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		if (dev.isValid && dev.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
		{
			dev.SendHapticImpulse(0u, Mathf.Clamp01(amplitude), Mathf.Clamp(duration, 0.01f, 1f));
		}
	}

	public static void TriggerHapticBoth(float amplitude = 0.5f, float duration = 0.05f)
	{
		TriggerHaptic(XRNode.RightHand, amplitude, duration);
		TriggerHaptic(XRNode.LeftHand, amplitude, duration);
	}

	public static RaycastHit[] AimRaycastAll(Transform headCamera)
	{
		if (TryGetAimRay(out Ray ray))
		{
			int count = Physics.RaycastNonAlloc(ray, _rayHitsBuffer, 50f);
			bool hitInteractive = false;
			for (int i = 0; i < count; i++)
			{
				Collider c = _rayHitsBuffer[i].collider;
				if (c != null && (c.CompareTag("Interactive") || c.GetComponent<AppButton>() != null))
				{
					hitInteractive = true;
					break;
				}
			}

			if (hitInteractive)
			{
				RaycastHit[] result = new RaycastHit[count];
				Array.Copy(_rayHitsBuffer, result, count);
				return result;
			}

			if (headCamera != null)
			{
				int headCount = Physics.RaycastNonAlloc(headCamera.position, headCamera.forward, _sphereHitsBuffer, 50f);
				for (int i = 0; i < headCount; i++)
				{
					Collider c = _sphereHitsBuffer[i].collider;
					if (c != null && (c.CompareTag("Interactive") || c.GetComponent<AppButton>() != null))
					{
						RaycastHit[] headResult = new RaycastHit[headCount];
						Array.Copy(_sphereHitsBuffer, headResult, headCount);
						return headResult;
					}
				}
			}

			if (count > 0)
			{
				RaycastHit[] result = new RaycastHit[count];
				Array.Copy(_rayHitsBuffer, result, count);
				return result;
			}
			return Array.Empty<RaycastHit>();
		}

		if (headCamera != null)
		{
			int headCount = Physics.RaycastNonAlloc(headCamera.position, headCamera.forward, _rayHitsBuffer, 50f);
			if (headCount == 0) return Array.Empty<RaycastHit>();
			RaycastHit[] headResult = new RaycastHit[headCount];
			Array.Copy(_rayHitsBuffer, headResult, headCount);
			return headResult;
		}

		return Array.Empty<RaycastHit>();
	}

	public static RaycastHit[] AimSphereCastAll(Transform headCamera, float radius)
	{
		if (TryGetAimRay(out Ray ray))
		{
			int count = Physics.SphereCastNonAlloc(ray.origin, radius, ray.direction, _sphereHitsBuffer, 50f);
			bool hitInteractive = false;
			for (int i = 0; i < count; i++)
			{
				Collider c = _sphereHitsBuffer[i].collider;
				if (c != null && (c.CompareTag("Interactive") || c.GetComponent<AppButton>() != null))
				{
					hitInteractive = true;
					break;
				}
			}

			if (hitInteractive)
			{
				RaycastHit[] result = new RaycastHit[count];
				Array.Copy(_sphereHitsBuffer, result, count);
				return result;
			}

			if (headCamera != null)
			{
				int headCount = Physics.SphereCastNonAlloc(headCamera.position, radius, headCamera.forward, _rayHitsBuffer, 50f);
				for (int i = 0; i < headCount; i++)
				{
					Collider c = _rayHitsBuffer[i].collider;
					if (c != null && (c.CompareTag("Interactive") || c.GetComponent<AppButton>() != null))
					{
						RaycastHit[] headResult = new RaycastHit[headCount];
						Array.Copy(_rayHitsBuffer, headResult, headCount);
						return headResult;
					}
				}
			}

			if (count > 0)
			{
				RaycastHit[] result = new RaycastHit[count];
				Array.Copy(_sphereHitsBuffer, result, count);
				return result;
			}
			return Array.Empty<RaycastHit>();
		}

		if (headCamera != null)
		{
			int headCount = Physics.SphereCastNonAlloc(headCamera.position, radius, headCamera.forward, _sphereHitsBuffer, 50f);
			if (headCount == 0) return Array.Empty<RaycastHit>();
			RaycastHit[] headResult = new RaycastHit[headCount];
			Array.Copy(_sphereHitsBuffer, headResult, headCount);
			return headResult;
		}

		return Array.Empty<RaycastHit>();
	}

	public static bool GetGripToggleDown()
	{
		EnsurePolledThisFrame();
		return _xrGripDown;
	}

	/// <summary>Controller aim pose relative to the (recentered) tracking origin, using the same space as GetEyePose.</summary>
	public static bool GetControllerLocalPose(XRNode hand, out Quaternion rot, out Vector3 pos)
	{
		rot = Quaternion.identity;
		pos = Vector3.zero;
		if (!XRSettings.isDeviceActive)
		{
			return false;
		}
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid)
		{
			dev = InputDevices.GetDeviceAtXRNode(hand);
		}
		if (!dev.isValid)
		{
			return false;
		}
		if (dev.TryGetFeatureValue(XRCommonUsages.isTracked, out bool tracked) && !tracked)
		{
			return false;
		}
		bool gotRot = dev.TryGetFeatureValue(new InputFeatureUsage<Quaternion>("PointerRotation"), out Quaternion r) ||
		              dev.TryGetFeatureValue(XRCommonUsages.deviceRotation, out r);
		bool gotPos = dev.TryGetFeatureValue(new InputFeatureUsage<Vector3>("PointerPosition"), out Vector3 p) ||
		              dev.TryGetFeatureValue(XRCommonUsages.devicePosition, out p);
		if (!gotRot || !gotPos)
		{
			return false;
		}
		Quaternion yawFix = Quaternion.Euler(0f, _recenterYawOffset + _snapTurnYawOffset, 0f);
		rot = yawFix * r;
		pos = yawFix * (p - _recenterPosOffset);
		return true;
	}

	public static bool StartVR()
	{
		try
		{
			XRGeneralSettings instance = XRGeneralSettings.Instance;
			if (instance != null && instance.Manager != null)
			{
				if (instance.Manager.activeLoader == null)
				{
					instance.Manager.InitializeLoaderSync();
				}
				if (instance.Manager.activeLoader != null)
				{
					instance.Manager.StartSubsystems();
				}
			}

			List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
			SubsystemManager.GetSubsystems(subsystems);
			for (int i = 0; i < subsystems.Count; i++)
			{
				subsystems[i].TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
				subsystems[i].TryRecenter();
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("OpenXR startup notice (falling back to desktop VR preview if no HMD connected): " + ex.Message);
		}

		_hasInitialRecenter = false;
		Recenter();
		return XRSettings.isDeviceActive;
	}

	public static void StopVR()
	{
		try
		{
			XRGeneralSettings instance = XRGeneralSettings.Instance;
			if (instance != null && instance.Manager != null && instance.Manager.activeLoader != null)
			{
				instance.Manager.StopSubsystems();
				instance.Manager.DeinitializeLoader();
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("OpenXR shutdown notice: " + ex.Message);
		}
	}

	public static void Recenter()
	{
		_fallbackYaw = 0f;
		_fallbackPitch = 0f;

		if (!XRSettings.isDeviceActive)
		{
			_recenterYawOffset = 0f;
			_recenterPosOffset = Vector3.zero;
			return;
		}

		List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
		SubsystemManager.GetSubsystems(subsystems);
		for (int i = 0; i < subsystems.Count; i++)
		{
			subsystems[i].TryRecenter();
		}

		if (TryReadRawNodePose(XRNode.CenterEye, out Quaternion rot, out Vector3 pos) ||
		    TryReadRawNodePose(XRNode.Head, out rot, out pos))
		{
			_recenterYawOffset = -rot.eulerAngles.y;
			_recenterPosOffset = pos;
			_hasInitialRecenter = true;
		}
		_snapTurnYawOffset = 0f;
	}

	public static Vector3 SoftClampHeadOffset(Vector3 v, float softStart = 0.40f, float maxLimit = 0.60f)
	{
		float mag = v.magnitude;
		if (mag <= softStart) return v;
		if (mag >= 5f) return v.normalized * maxLimit;
		float excess = mag - softStart;
		float range = maxLimit - softStart;
		float compressed = softStart + range * (1f - Mathf.Exp(-excess / range));
		return v * (compressed / mag);
	}

	public static void GetEyePose(bool isLeftEye, float fallbackIpd, out Quaternion localRot, out Vector3 localPos)
	{
		if (XRSettings.isDeviceActive)
		{
			XRNode targetNode = isLeftEye ? XRNode.LeftEye : XRNode.RightEye;
			if (!TryReadRawNodePose(targetNode, out Quaternion rawRot, out Vector3 rawPos))
			{
				if (!TryReadRawNodePose(XRNode.CenterEye, out rawRot, out rawPos))
				{
					TryReadRawNodePose(XRNode.Head, out rawRot, out rawPos);
				}
				Vector3 ipdShift = (isLeftEye ? Vector3.left : Vector3.right) * (fallbackIpd * 0.5f);
				rawPos += rawRot * ipdShift;
			}

			if (!_hasInitialRecenter && TryReadRawNodePose(XRNode.CenterEye, out Quaternion centerRot, out Vector3 centerPos))
			{
				_recenterYawOffset = -centerRot.eulerAngles.y;
				_recenterPosOffset = centerPos;
				_hasInitialRecenter = true;
			}

			Quaternion yawFix = Quaternion.Euler(0f, _recenterYawOffset + _snapTurnYawOffset, 0f);
			localRot = yawFix * rawRot;

			Vector3 relPos = yawFix * (rawPos - _recenterPosOffset);
			localPos = SoftClampHeadOffset(relPos);
			return;
		}

		EnsurePolledThisFrame();
		localRot = Quaternion.Euler(_fallbackPitch, _fallbackYaw, 0f);
		localPos = Vector3.zero;
	}

	public static void GetCenterHeadPose(out Quaternion localRot, out Vector3 localPos)
	{
		if (XRSettings.isDeviceActive)
		{
			if (!TryReadRawNodePose(XRNode.CenterEye, out Quaternion rawRot, out Vector3 rawPos))
			{
				TryReadRawNodePose(XRNode.Head, out rawRot, out rawPos);
			}
			if (!_hasInitialRecenter)
			{
				_recenterYawOffset = -rawRot.eulerAngles.y;
				_recenterPosOffset = rawPos;
				_hasInitialRecenter = true;
			}
			Quaternion yawFix = Quaternion.Euler(0f, _recenterYawOffset + _snapTurnYawOffset, 0f);
			localRot = yawFix * rawRot;
			localPos = SoftClampHeadOffset(yawFix * (rawPos - _recenterPosOffset));
			return;
		}

		EnsurePolledThisFrame();
		localRot = Quaternion.Euler(_fallbackPitch, _fallbackYaw, 0f);
		localPos = Vector3.zero;
	}

	private static bool TryReadRawNodePose(XRNode node, out Quaternion rot, out Vector3 pos)
	{
		rot = Quaternion.identity;
		pos = Vector3.zero;
		XRDevice dev = (node == XRNode.LeftEye || node == XRNode.LeftHand) ? _leftHandDevice :
		               (node == XRNode.RightEye || node == XRNode.RightHand) ? _rightHandDevice : _headDevice;
		if (!dev.isValid)
		{
			dev = InputDevices.GetDeviceAtXRNode(node);
		}
		if (!dev.isValid)
		{
			return false;
		}

		bool gotRot = false;
		if (node == XRNode.LeftEye && dev.TryGetFeatureValue(XRCommonUsages.leftEyeRotation, out Quaternion lRot))
		{
			rot = lRot;
			gotRot = true;
		}
		else if (node == XRNode.RightEye && dev.TryGetFeatureValue(XRCommonUsages.rightEyeRotation, out Quaternion rRot))
		{
			rot = rRot;
			gotRot = true;
		}
		else if (dev.TryGetFeatureValue(XRCommonUsages.centerEyeRotation, out Quaternion cRot))
		{
			rot = cRot;
			gotRot = true;
		}
		else if (dev.TryGetFeatureValue(XRCommonUsages.deviceRotation, out Quaternion dRot))
		{
			rot = dRot;
			gotRot = true;
		}

		if (node == XRNode.LeftEye && dev.TryGetFeatureValue(XRCommonUsages.leftEyePosition, out Vector3 lPos))
		{
			pos = lPos;
		}
		else if (node == XRNode.RightEye && dev.TryGetFeatureValue(XRCommonUsages.rightEyePosition, out Vector3 rPos))
		{
			pos = rPos;
		}
		else if (dev.TryGetFeatureValue(XRCommonUsages.centerEyePosition, out Vector3 cPos))
		{
			pos = cPos;
		}
		else if (dev.TryGetFeatureValue(XRCommonUsages.devicePosition, out Vector3 dPos))
		{
			pos = dPos;
		}

		return gotRot;
	}

	private static void EnsurePolledThisFrame()
	{
		if (_lastPolledFrame == Time.frameCount)
		{
			return;
		}
		_lastPolledFrame = Time.frameCount;

		if (!_rightHandDevice.isValid) _rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		if (!_leftHandDevice.isValid) _leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		if (!_headDevice.isValid) _headDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);

		if (!XRSettings.isDeviceActive && Cursor.lockState == CursorLockMode.Locked && Mouse.current != null)
		{
			Vector2 delta = Mouse.current.delta.ReadValue();
			_fallbackYaw += delta.x * 0.15f;
			_fallbackPitch = Mathf.Clamp(_fallbackPitch - delta.y * 0.15f, -85f, 85f);
		}

		bool rInteract = ReadControllerInteract(XRNode.RightHand);
		bool lInteract = ReadControllerInteract(XRNode.LeftHand);
		if (lInteract && !rInteract) ActivePointerHand = XRNode.LeftHand;
		else if (rInteract && !lInteract) ActivePointerHand = XRNode.RightHand;

		bool interactNow = rInteract || lInteract;
		_xrInteractDown = interactNow && !_prevXrInteract;
		_xrInteractUp = !interactNow && _prevXrInteract;
		_prevXrInteract = interactNow;

		bool menuNow = ReadControllerMenu(XRNode.RightHand) || ReadControllerMenu(XRNode.LeftHand);
		_xrMenuDown = menuNow && !_prevXrMenu;
		_xrMenuUp = !menuNow && _prevXrMenu;
		_prevXrMenu = menuNow;

		bool recenterNow = ReadControllerRecenter(XRNode.RightHand) || ReadControllerRecenter(XRNode.LeftHand);
		_xrRecenterDown = recenterNow && !_prevXrRecenter;
		_xrRecenterUp = !recenterNow && _prevXrRecenter;
		_prevXrRecenter = recenterNow;

		bool rGrip = ReadControllerGrip(XRNode.RightHand);
		bool lGrip = ReadControllerGrip(XRNode.LeftHand);
		if (lGrip && !rGrip) ActivePointerHand = XRNode.LeftHand;
		else if (rGrip && !lGrip) ActivePointerHand = XRNode.RightHand;
		bool gripNow = rGrip || lGrip;
		_xrGripDown = gripNow && !_prevXrGrip;
		_prevXrGrip = gripNow;

		Vector2 rStick = Vector2.zero;
		Vector2 lStick = Vector2.zero;
		if (_rightHandDevice.isValid) _rightHandDevice.TryGetFeatureValue(XRCommonUsages.primary2DAxis, out rStick);
		if (_leftHandDevice.isValid) _leftHandDevice.TryGetFeatureValue(XRCommonUsages.primary2DAxis, out lStick);

		bool rA = false, rB = false, lX = false, lY = false;
		if (_rightHandDevice.isValid)
		{
			_rightHandDevice.TryGetFeatureValue(XRCommonUsages.primaryButton, out rA);
			_rightHandDevice.TryGetFeatureValue(XRCommonUsages.secondaryButton, out rB);
		}
		if (_leftHandDevice.isValid)
		{
			_leftHandDevice.TryGetFeatureValue(XRCommonUsages.primaryButton, out lX);
			_leftHandDevice.TryGetFeatureValue(XRCommonUsages.secondaryButton, out lY);
		}

		// Modern 30-degree snap turn on right stick horizontal flick (or Q / E keys for desktop accessibility)
		bool snapTurnRight = rStick.x > 0.65f || Input.GetKeyDown(KeyCode.E);
		bool snapTurnLeft = rStick.x < -0.65f || Input.GetKeyDown(KeyCode.Q);
		if (snapTurnRight && !_prevSnapTurnStick)
		{
			SnapTurn(30f);
			_prevSnapTurnStick = true;
		}
		else if (snapTurnLeft && !_prevSnapTurnStick)
		{
			SnapTurn(-30f);
			_prevSnapTurnStick = true;
		}
		else if (Mathf.Abs(rStick.x) < 0.25f && !Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
		{
			_prevSnapTurnStick = false;
		}

		// Gesture recognition: left stick or A/B/X/Y buttons (or vertical head nod / horizontal shake)
		bool nodNow = Mathf.Abs(lStick.y) > 0.65f || rStick.y > 0.65f || rA || lX;
		_nodStickDown = nodNow && !_prevNodState;
		_prevNodState = nodNow;

		bool shakeNow = Mathf.Abs(lStick.x) > 0.65f || rB || lY;
		_shakeStickDown = shakeNow && !_prevShakeState;
		_prevShakeState = shakeNow;

		if (_nodStickDown || _shakeStickDown)
		{
			TriggerHaptic(ActivePointerHand, 0.35f, 0.05f);
		}
	}

	public static bool ReadControllerInteract(XRNode hand)
	{
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		if (!dev.isValid) return false;
		if (dev.TryGetFeatureValue(XRCommonUsages.triggerButton, out bool tb) && tb) return true;
		if (dev.TryGetFeatureValue(XRCommonUsages.trigger, out float t) && t > 0.6f) return true;
		if (dev.TryGetFeatureValue(XRCommonUsages.primaryButton, out bool pb) && pb) return true;
		return false;
	}

	public static bool ReadControllerMenu(XRNode hand)
	{
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		return dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.menuButton, out bool m) && m;
	}

	public static bool ReadControllerRecenter(XRNode hand)
	{
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		if (!dev.isValid) return false;
		if (dev.TryGetFeatureValue(XRCommonUsages.secondaryButton, out bool sb) && sb) return true;
		if (dev.TryGetFeatureValue(XRCommonUsages.primary2DAxisClick, out bool c) && c) return true;
		return false;
	}

	public static bool ReadControllerGrip(XRNode hand)
	{
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		return dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.gripButton, out bool g) && g;
	}

	public static bool ReadXrPrimary(XRNode hand)
	{
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		return dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.primaryButton, out bool p) && p;
	}

	public static bool ReadXrSecondary(XRNode hand)
	{
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		return dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.secondaryButton, out bool s) && s;
	}

	public static bool ReadXrStickClick(XRNode hand)
	{
		XRDevice dev = (hand == XRNode.RightHand) ? _rightHandDevice : _leftHandDevice;
		if (!dev.isValid) dev = InputDevices.GetDeviceAtXRNode(hand);
		return dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.primary2DAxisClick, out bool sc) && sc;
	}

	public static bool GetXrControllerInteractUp()
	{
		EnsurePolledThisFrame();
		return _xrInteractUp;
	}

	public static bool GetInteractDown()
	{
		EnsurePolledThisFrame();
		return GetButtonDown("Interact") || _xrInteractDown;
	}

	public static bool GetInteractUp()
	{
		EnsurePolledThisFrame();
		return GetButtonUp("Interact") || _xrInteractUp;
	}

	public static bool GetMenuExitUp()
	{
		EnsurePolledThisFrame();
		return GetButtonUp("Menu/Exit") || _xrMenuUp;
	}

	public static bool GetResetOrientationUp()
	{
		EnsurePolledThisFrame();
		return GetButtonUp("Reset Orientation") || _xrRecenterUp;
	}

	public static bool GetThumbstickNodDown()
	{
		EnsurePolledThisFrame();
		return _nodStickDown;
	}

	public static bool GetThumbstickShakeDown()
	{
		EnsurePolledThisFrame();
		return _shakeStickDown;
	}

	// -------------------------------------------------------------
	// NEW INPUT SYSTEM IMPLEMENTATIONS REPLACING UnityEngine.Input
	// -------------------------------------------------------------

	private static Key ToInputSystemKey(KeyCode key)
	{
		switch (key)
		{
			case KeyCode.A: return Key.A;
			case KeyCode.B: return Key.B;
			case KeyCode.C: return Key.C;
			case KeyCode.D: return Key.D;
			case KeyCode.E: return Key.E;
			case KeyCode.F: return Key.F;
			case KeyCode.G: return Key.G;
			case KeyCode.H: return Key.H;
			case KeyCode.I: return Key.I;
			case KeyCode.J: return Key.J;
			case KeyCode.K: return Key.K;
			case KeyCode.L: return Key.L;
			case KeyCode.M: return Key.M;
			case KeyCode.N: return Key.N;
			case KeyCode.O: return Key.O;
			case KeyCode.P: return Key.P;
			case KeyCode.Q: return Key.Q;
			case KeyCode.R: return Key.R;
			case KeyCode.S: return Key.S;
			case KeyCode.T: return Key.T;
			case KeyCode.U: return Key.U;
			case KeyCode.V: return Key.V;
			case KeyCode.W: return Key.W;
			case KeyCode.X: return Key.X;
			case KeyCode.Y: return Key.Y;
			case KeyCode.Z: return Key.Z;

			case KeyCode.Alpha0: return Key.Digit0;
			case KeyCode.Alpha1: return Key.Digit1;
			case KeyCode.Alpha2: return Key.Digit2;
			case KeyCode.Alpha3: return Key.Digit3;
			case KeyCode.Alpha4: return Key.Digit4;
			case KeyCode.Alpha5: return Key.Digit5;
			case KeyCode.Alpha6: return Key.Digit6;
			case KeyCode.Alpha7: return Key.Digit7;
			case KeyCode.Alpha8: return Key.Digit8;
			case KeyCode.Alpha9: return Key.Digit9;

			case KeyCode.Keypad0: return Key.Numpad0;
			case KeyCode.Keypad1: return Key.Numpad1;
			case KeyCode.Keypad2: return Key.Numpad2;
			case KeyCode.Keypad3: return Key.Numpad3;
			case KeyCode.Keypad4: return Key.Numpad4;
			case KeyCode.Keypad5: return Key.Numpad5;
			case KeyCode.Keypad6: return Key.Numpad6;
			case KeyCode.Keypad7: return Key.Numpad7;
			case KeyCode.Keypad8: return Key.Numpad8;
			case KeyCode.Keypad9: return Key.Numpad9;

			case KeyCode.Space: return Key.Space;
			case KeyCode.Return: return Key.Enter;
			case KeyCode.Escape: return Key.Escape;
			case KeyCode.Tab: return Key.Tab;
			case KeyCode.LeftShift: return Key.LeftShift;
			case KeyCode.RightShift: return Key.RightShift;
			case KeyCode.LeftControl: return Key.LeftCtrl;
			case KeyCode.RightControl: return Key.RightCtrl;
			case KeyCode.LeftAlt: return Key.LeftAlt;
			case KeyCode.RightAlt: return Key.RightAlt;

			case KeyCode.UpArrow: return Key.UpArrow;
			case KeyCode.DownArrow: return Key.DownArrow;
			case KeyCode.LeftArrow: return Key.LeftArrow;
			case KeyCode.RightArrow: return Key.RightArrow;

			case KeyCode.Minus: return Key.Minus;
			case KeyCode.Equals: return Key.Equals;
			case KeyCode.LeftBracket: return Key.LeftBracket;
			case KeyCode.RightBracket: return Key.RightBracket;
			case KeyCode.Comma: return Key.Comma;
			case KeyCode.Period: return Key.Period;

			case KeyCode.F1: return Key.F1;
			case KeyCode.F2: return Key.F2;
			case KeyCode.F3: return Key.F3;
			case KeyCode.F4: return Key.F4;
			case KeyCode.F5: return Key.F5;
			case KeyCode.F6: return Key.F6;
			case KeyCode.F7: return Key.F7;
			case KeyCode.F8: return Key.F8;
			case KeyCode.F9: return Key.F9;
			case KeyCode.F10: return Key.F10;
			case KeyCode.F11: return Key.F11;
			case KeyCode.F12: return Key.F12;

			default: return Key.None;
		}
	}

	private static bool CheckJoystickButton(KeyCode key, int stateType)
	{
		Gamepad gp = Gamepad.current;
		switch (key)
		{
			case KeyCode.JoystickButton0:
				if (gp != null)
				{
					if (stateType == 0 && gp.buttonSouth.isPressed) return true;
					if (stateType == 1 && gp.buttonSouth.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.buttonSouth.wasReleasedThisFrame) return true;
				}
				if (stateType == 0) return ReadXrPrimary(XRNode.RightHand);
				if (stateType == 1) return _xrInteractDown;
				if (stateType == 2) return _xrInteractUp;
				return false;
			case KeyCode.JoystickButton1:
				if (gp != null)
				{
					if (stateType == 0 && gp.buttonEast.isPressed) return true;
					if (stateType == 1 && gp.buttonEast.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.buttonEast.wasReleasedThisFrame) return true;
				}
				if (stateType == 0) return ReadXrSecondary(XRNode.RightHand);
				if (stateType == 1) return _xrRecenterDown;
				if (stateType == 2) return _xrRecenterUp;
				return false;
			case KeyCode.JoystickButton2:
				if (gp != null)
				{
					if (stateType == 0 && gp.buttonWest.isPressed) return true;
					if (stateType == 1 && gp.buttonWest.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.buttonWest.wasReleasedThisFrame) return true;
				}
				return ReadXrPrimary(XRNode.LeftHand);
			case KeyCode.JoystickButton3:
				if (gp != null)
				{
					if (stateType == 0 && gp.buttonNorth.isPressed) return true;
					if (stateType == 1 && gp.buttonNorth.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.buttonNorth.wasReleasedThisFrame) return true;
				}
				return ReadXrSecondary(XRNode.LeftHand);
			case KeyCode.JoystickButton4:
				if (gp != null)
				{
					if (stateType == 0 && gp.leftShoulder.isPressed) return true;
					if (stateType == 1 && gp.leftShoulder.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.leftShoulder.wasReleasedThisFrame) return true;
				}
				return ReadControllerGrip(XRNode.LeftHand);
			case KeyCode.JoystickButton5:
				if (gp != null)
				{
					if (stateType == 0 && gp.rightShoulder.isPressed) return true;
					if (stateType == 1 && gp.rightShoulder.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.rightShoulder.wasReleasedThisFrame) return true;
				}
				return ReadControllerGrip(XRNode.RightHand);
			case KeyCode.JoystickButton6:
				if (gp != null)
				{
					if (stateType == 0 && gp.selectButton.isPressed) return true;
					if (stateType == 1 && gp.selectButton.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.selectButton.wasReleasedThisFrame) return true;
				}
				return false;
			case KeyCode.JoystickButton7:
				if (gp != null)
				{
					if (stateType == 0 && gp.startButton.isPressed) return true;
					if (stateType == 1 && gp.startButton.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.startButton.wasReleasedThisFrame) return true;
				}
				if (stateType == 0) return ReadControllerMenu(XRNode.LeftHand) || ReadControllerMenu(XRNode.RightHand);
				if (stateType == 1) return _xrMenuDown;
				if (stateType == 2) return _xrMenuUp;
				return false;
			case KeyCode.JoystickButton8:
				if (gp != null)
				{
					if (stateType == 0 && gp.leftStickButton.isPressed) return true;
					if (stateType == 1 && gp.leftStickButton.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.leftStickButton.wasReleasedThisFrame) return true;
				}
				return ReadXrStickClick(XRNode.LeftHand);
			case KeyCode.JoystickButton9:
				if (gp != null)
				{
					if (stateType == 0 && gp.rightStickButton.isPressed) return true;
					if (stateType == 1 && gp.rightStickButton.wasPressedThisFrame) return true;
					if (stateType == 2 && gp.rightStickButton.wasReleasedThisFrame) return true;
				}
				return ReadXrStickClick(XRNode.RightHand);
			default:
				return false;
		}
	}

	public static bool GetKey(string name)
	{
		if (string.Equals(name, "space", StringComparison.OrdinalIgnoreCase)) return GetKey(KeyCode.Space);
		if (Enum.TryParse<KeyCode>(name, true, out KeyCode kc)) return GetKey(kc);
		return false;
	}

	public static bool GetKeyDown(string name)
	{
		if (string.Equals(name, "space", StringComparison.OrdinalIgnoreCase)) return GetKeyDown(KeyCode.Space);
		if (Enum.TryParse<KeyCode>(name, true, out KeyCode kc)) return GetKeyDown(kc);
		return false;
	}

	public static bool GetKeyUp(string name)
	{
		if (string.Equals(name, "space", StringComparison.OrdinalIgnoreCase)) return GetKeyUp(KeyCode.Space);
		if (Enum.TryParse<KeyCode>(name, true, out KeyCode kc)) return GetKeyUp(kc);
		return false;
	}

	public static bool GetKey(KeyCode key)
	{
		EnsurePolledThisFrame();
		if (key >= KeyCode.JoystickButton0 && key <= KeyCode.JoystickButton19)
		{
			return CheckJoystickButton(key, 0);
		}
		Key k = ToInputSystemKey(key);
		if (k != Key.None && Keyboard.current != null)
		{
			return Keyboard.current[k].isPressed;
		}
		return false;
	}

	public static bool GetKeyDown(KeyCode key)
	{
		EnsurePolledThisFrame();
		if (key >= KeyCode.JoystickButton0 && key <= KeyCode.JoystickButton19)
		{
			return CheckJoystickButton(key, 1);
		}
		Key k = ToInputSystemKey(key);
		if (k != Key.None && Keyboard.current != null)
		{
			return Keyboard.current[k].wasPressedThisFrame;
		}
		return false;
	}

	public static bool GetKeyUp(KeyCode key)
	{
		EnsurePolledThisFrame();
		if (key >= KeyCode.JoystickButton0 && key <= KeyCode.JoystickButton19)
		{
			return CheckJoystickButton(key, 2);
		}
		Key k = ToInputSystemKey(key);
		if (k != Key.None && Keyboard.current != null)
		{
			return Keyboard.current[k].wasReleasedThisFrame;
		}
		return false;
	}

	public static bool GetMouseButton(int button)
	{
		EnsurePolledThisFrame();
		Mouse mouse = Mouse.current;
		if (mouse == null) return false;
		if (button == 0) return mouse.leftButton.isPressed;
		if (button == 1) return mouse.rightButton.isPressed;
		if (button == 2) return mouse.middleButton.isPressed;
		return false;
	}

	public static bool GetMouseButtonDown(int button)
	{
		EnsurePolledThisFrame();
		Mouse mouse = Mouse.current;
		if (mouse == null) return false;
		if (button == 0) return mouse.leftButton.wasPressedThisFrame;
		if (button == 1) return mouse.rightButton.wasPressedThisFrame;
		if (button == 2) return mouse.middleButton.wasPressedThisFrame;
		return false;
	}

	public static bool GetMouseButtonUp(int button)
	{
		EnsurePolledThisFrame();
		Mouse mouse = Mouse.current;
		if (mouse == null) return false;
		if (button == 0) return mouse.leftButton.wasReleasedThisFrame;
		if (button == 1) return mouse.rightButton.wasReleasedThisFrame;
		if (button == 2) return mouse.middleButton.wasReleasedThisFrame;
		return false;
	}

	public static float GetAxis(string axisName)
	{
		EnsurePolledThisFrame();
		switch (axisName)
		{
			case "Mouse X":
				if (Mouse.current != null)
				{
					return Mouse.current.delta.x.ReadValue() * 0.15f;
				}
				return 0f;
			case "Mouse Y":
				if (Mouse.current != null)
				{
					return Mouse.current.delta.y.ReadValue() * 0.15f;
				}
				return 0f;
			case "Mouse ScrollWheel":
				if (Mouse.current != null)
				{
					return Mouse.current.scroll.y.ReadValue() / 120f;
				}
				return 0f;
			case "Horizontal":
			{
				float val = 0f;
				Keyboard kb = Keyboard.current;
				if (kb != null)
				{
					if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) val += 1f;
					if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) val -= 1f;
				}
				Gamepad gp = Gamepad.current;
				if (gp != null)
				{
					float stick = gp.leftStick.x.ReadValue();
					if (Mathf.Abs(stick) > Mathf.Abs(val)) val = stick;
					if (gp.dpad.right.isPressed) val = 1f;
					if (gp.dpad.left.isPressed) val = -1f;
				}
				XRDevice dev = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
				Vector2 stickX = Vector2.zero;
				if (dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.primary2DAxis, out stickX) && Mathf.Abs(stickX.x) > 0.1f)
				{
					if (Mathf.Abs(stickX.x) > Mathf.Abs(val)) val = stickX.x;
				}
				return val;
			}
			case "Vertical":
			{
				float val = 0f;
				Keyboard kb = Keyboard.current;
				if (kb != null)
				{
					if (kb.wKey.isPressed || kb.upArrowKey.isPressed) val += 1f;
					if (kb.sKey.isPressed || kb.downArrowKey.isPressed) val -= 1f;
				}
				Gamepad gp = Gamepad.current;
				if (gp != null)
				{
					float stick = gp.leftStick.y.ReadValue();
					if (Mathf.Abs(stick) > Mathf.Abs(val)) val = stick;
					if (gp.dpad.up.isPressed) val = 1f;
					if (gp.dpad.down.isPressed) val = -1f;
				}
				XRDevice dev = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
				Vector2 stickY = Vector2.zero;
				if (dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.primary2DAxis, out stickY) && Mathf.Abs(stickY.y) > 0.1f)
				{
					if (Mathf.Abs(stickY.y) > Mathf.Abs(val)) val = stickY.y;
				}
				return val;
			}
			default:
				return 0f;
		}
	}

	public static float GetAxisRaw(string axisName)
	{
		float a = GetAxis(axisName);
		if (axisName == "Horizontal" || axisName == "Vertical")
		{
			if (a > 0.2f) return 1f;
			if (a < -0.2f) return -1f;
			return 0f;
		}
		return a;
	}

	public static bool GetButton(string buttonName)
	{
		EnsurePolledThisFrame();
		switch (buttonName)
		{
			case "Interact":
				return (Keyboard.current != null && (Keyboard.current.spaceKey.isPressed || Keyboard.current.enterKey.isPressed)) ||
				       (Mouse.current != null && Mouse.current.leftButton.isPressed) ||
				       (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed) ||
				       _prevXrInteract;
			case "Menu/Exit":
				return (Keyboard.current != null && Keyboard.current.escapeKey.isPressed) ||
				       (Gamepad.current != null && (Gamepad.current.buttonEast.isPressed || Gamepad.current.startButton.isPressed)) ||
				       _prevXrMenu;
			case "Reset Orientation":
				return (Keyboard.current != null && Keyboard.current.rKey.isPressed) ||
				       (Gamepad.current != null && (Gamepad.current.buttonNorth.isPressed || (Gamepad.current.rightStickButton != null && Gamepad.current.rightStickButton.isPressed))) ||
				       _prevXrRecenter;
			case "Jump":
				return (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) ||
				       (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed) ||
				       _prevXrInteract;
			default:
				return false;
		}
	}

	public static bool GetButtonDown(string buttonName)
	{
		EnsurePolledThisFrame();
		switch (buttonName)
		{
			case "Interact":
				return (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)) ||
				       (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
				       (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) ||
				       _xrInteractDown;
			case "Menu/Exit":
				return (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
				       (Gamepad.current != null && (Gamepad.current.buttonEast.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame)) ||
				       _xrMenuDown;
			case "Reset Orientation":
				return (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) ||
				       (Gamepad.current != null && (Gamepad.current.buttonNorth.wasPressedThisFrame || (Gamepad.current.rightStickButton != null && Gamepad.current.rightStickButton.wasPressedThisFrame))) ||
				       _xrRecenterDown;
			case "Jump":
				return (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
				       (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) ||
				       _xrInteractDown;
			default:
				return false;
		}
	}

	public static bool GetButtonUp(string buttonName)
	{
		EnsurePolledThisFrame();
		switch (buttonName)
		{
			case "Interact":
				return (Keyboard.current != null && (Keyboard.current.spaceKey.wasReleasedThisFrame || Keyboard.current.enterKey.wasReleasedThisFrame)) ||
				       (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame) ||
				       (Gamepad.current != null && Gamepad.current.buttonSouth.wasReleasedThisFrame) ||
				       _xrInteractUp;
			case "Menu/Exit":
				return (Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame) ||
				       (Gamepad.current != null && (Gamepad.current.buttonEast.wasReleasedThisFrame || Gamepad.current.startButton.wasReleasedThisFrame)) ||
				       _xrMenuUp;
			case "Reset Orientation":
				return (Keyboard.current != null && Keyboard.current.rKey.wasReleasedThisFrame) ||
				       (Gamepad.current != null && (Gamepad.current.buttonNorth.wasReleasedThisFrame || (Gamepad.current.rightStickButton != null && Gamepad.current.rightStickButton.wasReleasedThisFrame))) ||
				       _xrRecenterUp;
			case "Jump":
				return (Keyboard.current != null && Keyboard.current.spaceKey.wasReleasedThisFrame) ||
				       (Gamepad.current != null && Gamepad.current.buttonSouth.wasReleasedThisFrame) ||
				       _xrInteractUp;
			default:
				return false;
		}
	}

	public static Vector3 mousePosition
	{
		get
		{
			Mouse mouse = Mouse.current;
			if (mouse != null)
			{
				Vector2 pos = mouse.position.ReadValue();
				return new Vector3(pos.x, pos.y, 0f);
			}
			return new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
		}
	}

	public static string[] GetJoystickNames()
	{
		List<string> names = new List<string>();
		foreach (var dev in UnityEngine.InputSystem.InputSystem.devices)
		{
			if (dev is Gamepad || dev is Joystick)
			{
				names.Add(dev.name);
			}
		}
		if (XRSettings.isDeviceActive)
		{
			names.Add("OpenXR Controller");
		}
		return names.ToArray();
	}

	public static bool anyKeyDown
	{
		get
		{
			Keyboard kb = Keyboard.current;
			if (kb != null && kb.anyKey.wasPressedThisFrame) return true;
			Mouse mouse = Mouse.current;
			if (mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)) return true;
			Gamepad gp = Gamepad.current;
			if (gp != null && (gp.buttonSouth.wasPressedThisFrame || gp.buttonEast.wasPressedThisFrame || gp.startButton.wasPressedThisFrame)) return true;
			return _xrInteractDown || _xrMenuDown || _xrRecenterDown;
		}
	}

	public static bool anyKey
	{
		get
		{
			Keyboard kb = Keyboard.current;
			if (kb != null && kb.anyKey.isPressed) return true;
			Mouse mouse = Mouse.current;
			if (mouse != null && (mouse.leftButton.isPressed || mouse.rightButton.isPressed)) return true;
			Gamepad gp = Gamepad.current;
			if (gp != null && (gp.buttonSouth.isPressed || gp.buttonEast.isPressed || gp.startButton.isPressed)) return true;
			return _prevXrInteract;
		}
	}
}

public static class AsunderQuestRuntime
{
	public static bool IsQuestBuild
	{
		get
		{
			return Application.platform == RuntimePlatform.Android;
		}
	}

	public static void ApplyQuestQuality()
	{
		QualitySettings.softParticles = false;
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
		QualitySettings.antiAliasing = 4;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 90;
		XRSettings.eyeTextureResolutionScale = 1.2f;
	}
}

public class AsunderQuestControllerRay : MonoBehaviour
{
	private LineRenderer _lineRenderer;
	private GameObject _hitReticle;
	private Material _lineMat;
	private Material _reticleMat;
	private bool _pointerEnabledInGameplay = true;
	private readonly RaycastHit[] _rayHitsBuffer = new RaycastHit[32];
	private bool _prevHoverInteractive;

	private void Awake()
	{
		CreateLaserVisuals();
	}

	private void CreateLaserVisuals()
	{
		if (_lineRenderer == null)
		{
			_lineRenderer = base.gameObject.AddComponent<LineRenderer>();
			Shader s = Shader.Find("Sprites/Default") ?? Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("GUI/3D Text Shader");
			if (s != null)
			{
				_lineMat = new Material(s);
				_lineMat.color = new Color(0.2f, 0.8f, 1f, 0.7f);
				_lineRenderer.material = _lineMat;
			}
			_lineRenderer.startWidth = 0.005f;
			_lineRenderer.endWidth = 0.001f;
			_lineRenderer.positionCount = 2;
			_lineRenderer.useWorldSpace = true;
			_lineRenderer.enabled = false;
		}

		if (_hitReticle == null)
		{
			_hitReticle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			_hitReticle.name = "AsunderRayReticle";
			Collider col = _hitReticle.GetComponent<Collider>();
			if (col != null)
			{
				Destroy(col);
			}
			_hitReticle.transform.localScale = Vector3.one * 0.025f;
			Shader s = Shader.Find("Sprites/Default") ?? Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("GUI/3D Text Shader");
			if (s != null)
			{
				_reticleMat = new Material(s);
				_reticleMat.color = new Color(0.3f, 0.9f, 1f, 0.9f);
				Renderer rend = _hitReticle.GetComponent<Renderer>();
				if (rend != null)
				{
					rend.material = _reticleMat;
				}
			}
			_hitReticle.SetActive(false);
		}
	}

	private bool TryGetWorldPose(XRNode hand, out Vector3 worldPos, out Quaternion worldRot)
	{
		worldPos = Vector3.zero;
		worldRot = Quaternion.identity;
		if (!AsunderInput.GetControllerLocalPose(hand, out Quaternion localRot, out Vector3 localPos))
		{
			return false;
		}
		OVRCameraController cc = GetComponent<OVRCameraController>() ?? GetComponentInParent<OVRCameraController>();
		if (cc != null)
		{
			float yRot = 0f;
			cc.GetYRotation(ref yRot);
			Quaternion orientationOffset = Quaternion.identity;
			cc.GetOrientationOffset(ref orientationOffset);
			Quaternion rigBaseRot = orientationOffset * Quaternion.Euler(0f, yRot, 0f);
			Vector3 neckCenter = cc.transform.position + rigBaseRot * (cc.NeckPosition + new Vector3(0f, cc.EyeCenterPosition.y, cc.EyeCenterPosition.z));
			worldPos = neckCenter + rigBaseRot * localPos;
			worldRot = rigBaseRot * localRot;
			return true;
		}
		worldRot = base.transform.rotation * localRot;
		worldPos = base.transform.TransformPoint(localPos);
		return true;
	}

	private void Update()
	{
		if (AsunderInput.GetGripToggleDown())
		{
			_pointerEnabledInGameplay = !_pointerEnabledInGameplay;
		}

		string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		bool isMenuScene = (currentScene != "MainScene");
		bool wantPointer = isMenuScene || _pointerEnabledInGameplay;

		XRNode dominantHand = AsunderInput.ActivePointerHand;
		XRNode otherHand = (dominantHand == XRNode.RightHand) ? XRNode.LeftHand : XRNode.RightHand;
		bool hasController = TryGetWorldPose(dominantHand, out Vector3 cPos, out Quaternion cRot) ||
		                     TryGetWorldPose(otherHand, out cPos, out cRot);

		if (wantPointer && hasController)
		{
			AsunderInput.PointerActive = true;
			Vector3 forward = cRot * Vector3.forward;
			Ray ray = new Ray(cPos, forward);
			AsunderInput.PointerRay = ray;

			float maxDist = 25f;
			Vector3 endPos = cPos + forward * maxDist;
			bool hitInteractive = false;

			int hitCount = Physics.RaycastNonAlloc(ray, _rayHitsBuffer, maxDist);
			float closestSolidDist = float.MaxValue;
			bool hasSolidHit = false;
			RaycastHit closestHit = default;

			for (int i = 0; i < hitCount; i++)
			{
				RaycastHit h = _rayHitsBuffer[i];
				if (h.collider == null) continue;
				if (h.collider.CompareTag("Interactive") || h.collider.GetComponent<AppButton>() != null)
				{
					hitInteractive = true;
				}
				if (!h.collider.isTrigger && h.distance < closestSolidDist)
				{
					closestSolidDist = h.distance;
					closestHit = h;
					hasSolidHit = true;
				}
			}

			if (hitInteractive && !_prevHoverInteractive)
			{
				AsunderInput.TriggerHaptic(dominantHand, 0.25f, 0.03f);
			}
			if (hitInteractive && AsunderInput.GetInteractDown())
			{
				AsunderInput.TriggerHaptic(dominantHand, 0.65f, 0.08f);
			}
			_prevHoverInteractive = hitInteractive;

			if (hasSolidHit)
			{
				endPos = closestHit.point;
			}

			float alphaMult = 1f;
			if (currentScene == "MainScene" && !hitInteractive && forward.y < -0.5f)
			{
				alphaMult = Mathf.Clamp01((forward.y + 0.85f) / 0.35f);
			}

			if (_lineRenderer != null)
			{
				_lineRenderer.enabled = (alphaMult > 0.01f);
				_lineRenderer.SetPosition(0, cPos + forward * 0.05f);
				_lineRenderer.SetPosition(1, endPos);
				Color beamColor = hitInteractive ? new Color(1f, 0.85f, 0.2f, 0.9f * alphaMult) : new Color(0.2f, 0.8f, 1f, 0.55f * alphaMult);
				_lineRenderer.startColor = beamColor;
				_lineRenderer.endColor = new Color(beamColor.r, beamColor.g, beamColor.b, 0.05f * alphaMult);
			}

			if (_hitReticle != null)
			{
				_hitReticle.SetActive(alphaMult > 0.01f);
				_hitReticle.transform.position = endPos;
				_hitReticle.transform.localScale = Vector3.one * (hitInteractive ? 0.035f : 0.015f);
				if (_reticleMat != null)
				{
					_reticleMat.color = hitInteractive ? new Color(1f, 0.9f, 0.3f, 1f * alphaMult) : new Color(0.3f, 0.85f, 1f, 0.6f * alphaMult);
				}
			}
		}
		else
		{
			AsunderInput.PointerActive = false;
			_prevHoverInteractive = false;
			if (_lineRenderer != null) _lineRenderer.enabled = false;
			if (_hitReticle != null) _hitReticle.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		if (_hitReticle != null)
		{
			Destroy(_hitReticle);
			_hitReticle = null;
		}
		if (_lineMat != null)
		{
			Destroy(_lineMat);
			_lineMat = null;
		}
		if (_reticleMat != null)
		{
			Destroy(_reticleMat);
			_reticleMat = null;
		}
	}
}

public class OVRDevice : MonoBehaviour
{
	public struct MessageList
	{
		public byte isHMDSensorAttached;
		public byte isHMDAttached;
		public byte isLatencyTesterAttached;

		public MessageList(byte HMDSensor, byte HMD, byte LatencyTester)
		{
			isHMDSensorAttached = HMDSensor;
			isHMDAttached = HMD;
			isLatencyTesterAttached = LatencyTester;
		}
	}

	private enum SensorOrientation
	{
		Head = 0,
		Other = 1
	}

	public float InitialPredictionTime = 0.05f;
	public bool ResetTrackerOnLoad = true;

	private static MessageList MsgList = new MessageList(0, 0, 0);
	private static bool OVRInit = false;

	public static int SensorCount = 1;
	public static string DisplayDeviceName = "OpenXR HMD";
	public static int HResolution = 1920;
	public static int VResolution = 1080;
	public static float HScreenSize = 0.14976f;
	public static float VScreenSize = 0.0936f;
	public static float EyeToScreenDistance = 0.041f;
	public static float LensSeparationDistance = 0.0635f;
	public static float LeftEyeOffset = 0.032f;
	public static float RightEyeOffset = 0.032f;
	public static float ScreenVCenter = 0.0468f;
	public static float DistK0 = 1f;
	public static float DistK1 = 0f;
	public static float DistK2 = 0f;
	public static float DistK3 = 0f;
	public static float DistortionFitScale = 1f;

	private static float LensOffsetLeft = 0f;
	private static float LensOffsetRight = 0f;
	private static float PredictionTime = 0.03f;

	private static Dictionary<int, int> SensorList = new Dictionary<int, int>();
	private static Dictionary<int, SensorOrientation> SensorOrientationList = new Dictionary<int, SensorOrientation>();

	private void Awake()
	{
		InitSensorList(false);
		InitOrientationSensorList();
		OVRInit = true;
		HResolution = Mathf.Max(1280, XRSettings.eyeTextureWidth > 0 ? XRSettings.eyeTextureWidth * 2 : Screen.width);
		VResolution = Mathf.Max(720, XRSettings.eyeTextureHeight > 0 ? XRSettings.eyeTextureHeight : Screen.height);
		DisplayDeviceName = string.IsNullOrEmpty(XRSettings.loadedDeviceName) ? "OpenXR" : XRSettings.loadedDeviceName;
		SensorCount = 1;
		PredictionTime = InitialPredictionTime;
		if (ResetTrackerOnLoad)
		{
			AsunderInput.Recenter();
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		MessageList prev = MsgList;
		byte attached = (byte)(XRSettings.isDeviceActive ? 1 : 0);
		MsgList.isHMDAttached = attached;
		MsgList.isHMDSensorAttached = attached;
		MsgList.isLatencyTesterAttached = 0;

		if (MsgList.isHMDSensorAttached != 0 && prev.isHMDSensorAttached == 0)
		{
			OVRMessenger.Broadcast("Sensor_Attached", OVRMainMenu.Device.HMDSensor, true);
		}
		else if (MsgList.isHMDSensorAttached == 0 && prev.isHMDSensorAttached != 0)
		{
			OVRMessenger.Broadcast("Sensor_Attached", OVRMainMenu.Device.HMDSensor, false);
		}
		if (MsgList.isHMDAttached != 0 && prev.isHMDAttached == 0)
		{
			OVRMessenger.Broadcast("Sensor_Attached", OVRMainMenu.Device.HMD, true);
		}
		else if (MsgList.isHMDAttached == 0 && prev.isHMDAttached != 0)
		{
			OVRMessenger.Broadcast("Sensor_Attached", OVRMainMenu.Device.HMD, false);
		}
	}

	private void OnDestroy()
	{
	}

	public static bool IsInitialized() => OVRInit;
	public static bool IsHMDPresent() => XRSettings.isDeviceActive;
	public static bool IsSensorPresent(int sensor) => XRSettings.isDeviceActive;

	public static bool GetOrientation(int sensor, ref Quaternion q)
	{
		AsunderInput.GetCenterHeadPose(out q, out _);
		return true;
	}

	public static bool GetPredictedOrientation(int sensor, ref Quaternion q)
	{
		AsunderInput.GetCenterHeadPose(out q, out _);
		return true;
	}

	public static bool ResetOrientation(int sensor)
	{
		AsunderInput.Recenter();
		return true;
	}

	public static bool GetAcceleration(int sensor, ref float x, ref float y, ref float z)
	{
		XRDevice dev = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		Vector3 acc = Vector3.zero;
		if (dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.deviceAcceleration, out acc))
		{
			x = acc.x;
			y = acc.y;
			z = acc.z;
			return true;
		}
		x = 0f; y = 0f; z = 0f;
		return false;
	}

	public static bool GetAngularVelocity(int sensor, ref float x, ref float y, ref float z)
	{
		XRDevice dev = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		Vector3 av = Vector3.zero;
		if (dev.isValid && dev.TryGetFeatureValue(XRCommonUsages.deviceAngularVelocity, out av))
		{
			x = av.x;
			y = av.y;
			z = av.z;
			return true;
		}
		x = 0f; y = 0f; z = 0f;
		return false;
	}

	public static bool GetMagnetometer(int sensor, ref float x, ref float y, ref float z)
	{
		x = 0f; y = 0f; z = 0f;
		return false;
	}

	public static float GetPredictionTime(int sensor) => PredictionTime;

	public static bool SetPredictionTime(int sensor, float predictionTime)
	{
		if (predictionTime >= 0f)
		{
			PredictionTime = predictionTime;
			return true;
		}
		return false;
	}

	public static bool GetDistortionCorrectionCoefficients(ref float k0, ref float k1, ref float k2, ref float k3)
	{
		k0 = DistK0; k1 = DistK1; k2 = DistK2; k3 = DistK3;
		return true;
	}

	public static bool SetDistortionCorrectionCoefficients(float k0, float k1, float k2, float k3)
	{
		DistK0 = k0; DistK1 = k1; DistK2 = k2; DistK3 = k3;
		return true;
	}

	public static bool GetPhysicalLensOffsets(ref float lensOffsetLeft, ref float lensOffsetRight)
	{
		lensOffsetLeft = LensOffsetLeft;
		lensOffsetRight = LensOffsetRight;
		return true;
	}

	public static bool GetIPD(ref float IPD)
	{
		XRDevice left = InputDevices.GetDeviceAtXRNode(XRNode.LeftEye);
		XRDevice right = InputDevices.GetDeviceAtXRNode(XRNode.RightEye);
		Vector3 lp = Vector3.zero;
		Vector3 rp = Vector3.zero;
		if (left.isValid && right.isValid &&
		    left.TryGetFeatureValue(XRCommonUsages.leftEyePosition, out lp) &&
		    right.TryGetFeatureValue(XRCommonUsages.rightEyePosition, out rp))
		{
			float dist = Vector3.Distance(lp, rp);
			if (dist > 0.04f && dist < 0.09f)
			{
				IPD = dist;
				return true;
			}
		}
		if (IPD <= 0f)
		{
			IPD = 0.064f;
		}
		return true;
	}

	public static float CalculateAspectRatio()
	{
		if (XRSettings.isDeviceActive && XRSettings.eyeTextureHeight > 0)
		{
			return (float)XRSettings.eyeTextureWidth / (float)XRSettings.eyeTextureHeight;
		}
		if (Screen.height > 0)
		{
			return (float)Screen.width / (float)Screen.height;
		}
		return 1.7777778f;
	}

	public static float VerticalFOV() => 90f;
	public static float DistortionScale() => 1f;
	public static void ProcessLatencyInputs() { }
	public static bool DisplayLatencyScreenColor(ref byte r, ref byte g, ref byte b) { r = 0; g = 0; b = 0; return false; }
	public static IntPtr GetLatencyResultsString() => IntPtr.Zero;
	public static float CalcScale(float fitRadius) => 1f;
	public static bool CalculatePhysicalLensOffsets(ref float leftOffset, ref float rightOffset) { leftOffset = 0f; rightOffset = 0f; return true; }
	public static bool IsMagCalibrated(int sensor) => true;
	public static bool EnableMagYawCorrection(int sensor, bool enable) => true;
	public static bool IsYawCorrectionEnabled(int sensor) => true;

	public static void InitSensorList(bool reverse)
	{
		SensorList.Clear();
		if (reverse)
		{
			SensorList.Add(0, 1);
			SensorList.Add(1, 0);
		}
		else
		{
			SensorList.Add(0, 0);
			SensorList.Add(1, 1);
		}
	}

	public static void InitOrientationSensorList()
	{
		SensorOrientationList.Clear();
		SensorOrientationList.Add(0, SensorOrientation.Head);
		SensorOrientationList.Add(1, SensorOrientation.Other);
	}

	public static void OrientSensor(int sensor, ref Quaternion q) { }
	public static bool RenderPortraitMode() => false;
	public static bool GetPlayerEyeHeight(ref float eyeHeight) { eyeHeight = 1.65f; return true; }
}
