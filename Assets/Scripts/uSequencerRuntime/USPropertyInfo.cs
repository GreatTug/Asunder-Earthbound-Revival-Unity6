using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class USPropertyInfo : ScriptableObject
{
	[SerializeField]
	private PropertyTypeInfo propertyType = PropertyTypeInfo.None;

	[SerializeField]
	private Component component;

	[SerializeField]
	private string componentType;

	public string propertyName;

	public string fieldName;

	private PropertyInfo cachedPropertyInfo;

	private FieldInfo cachedFieldInfo;

	public List<USInternalCurve> curves = new List<USInternalCurve>();

	[Obsolete("curves0 - curves 4 is now Obsolete, please use the curves list, instead.")]
	public AnimationCurve curves0;

	[Obsolete("curves0 - curves 4 is now Obsolete, please use the curves list, instead.")]
	public AnimationCurve curves1;

	[Obsolete("curves0 - curves 4 is now Obsolete, please use the curves list, instead.")]
	public AnimationCurve curves2;

	[Obsolete("curves0 - curves 4 is now Obsolete, please use the curves list, instead.")]
	public AnimationCurve curves3;

	private bool tmpBool;

	private Keyframe tmpKeyframe = default(Keyframe);

	private Vector2 tmpVector2 = Vector2.zero;

	private Vector3 tmpVector3 = Vector3.zero;

	private Vector4 tmpVector4 = Vector4.zero;

	private Quaternion tmpQuat = Quaternion.identity;

	private Color tmpColour = Color.black;

	private float previousTime = -1f;

	public PropertyInfo propertyInfo
	{
		get
		{
			if (cachedPropertyInfo != null)
			{
				return cachedPropertyInfo;
			}
			if (!component || propertyName == null)
			{
				return null;
			}
			Type type = component.GetType();
			cachedPropertyInfo = type.GetProperty(propertyName);
			return cachedPropertyInfo;
		}
		set
		{
			if (value != null)
			{
				propertyName = value.Name;
			}
			else
			{
				propertyName = null;
			}
			cachedPropertyInfo = value;
		}
	}

	public FieldInfo fieldInfo
	{
		get
		{
			if (cachedFieldInfo != null)
			{
				return cachedFieldInfo;
			}
			if (!component || fieldName == null)
			{
				return null;
			}
			Type type = component.GetType();
			cachedFieldInfo = type.GetField(fieldName);
			return cachedFieldInfo;
		}
		set
		{
			if (value != null)
			{
				fieldName = value.Name;
			}
			else
			{
				fieldName = null;
			}
			cachedFieldInfo = value;
		}
	}

	public Component Component
	{
		get
		{
			return component;
		}
		set
		{
			component = value;
			ComponentType = ((!component) ? string.Empty : component.GetType().ToString());
		}
	}

	public string ComponentType
	{
		get
		{
			if (componentType == string.Empty && (bool)component)
			{
				componentType = component.GetType().ToString();
			}
			return componentType;
		}
		set
		{
			componentType = value.Replace("UnityEngine.", string.Empty);
		}
	}

	public PropertyTypeInfo PropertyType
	{
		get
		{
			return propertyType;
		}
	}

	public void CreatePropertyInfo(PropertyTypeInfo createdPropertyType)
	{
		if (propertyType != PropertyTypeInfo.None)
		{
			Debug.LogError("We are trying to CreatePropertyInfo, but it will be an overwrite!");
			return;
		}
		propertyType = createdPropertyType;
		CreatePropertyCurves();
	}

	public void SetValue(float time)
	{
		if (curves.Count == 0)
		{
			return;
		}
		if (propertyType == PropertyTypeInfo.None)
		{
			Debug.LogError("Atempting to Set A Value, before calling CreatePropertyInfo");
			return;
		}
		float num = float.PositiveInfinity;
		float num2 = 0f;
		foreach (USInternalCurve curf in curves)
		{
			if (num > curf.FirstKeyframeTime)
			{
				num = curf.FirstKeyframeTime;
			}
			if (num2 < curf.LastKeyframeTime)
			{
				num2 = curf.LastKeyframeTime;
			}
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (previousTime >= num && previousTime <= num2)
		{
			flag = true;
		}
		if (time >= num && time <= num2)
		{
			flag2 = true;
		}
		if ((previousTime < num && time > num2) || (previousTime > num2 && time < num))
		{
			flag3 = true;
		}
		previousTime = time;
		bool flag4 = false;
		if (flag3)
		{
			flag4 = true;
		}
		if (flag2)
		{
			flag4 = true;
		}
		if (!flag2 && flag)
		{
			flag4 = true;
		}
		if (!flag4)
		{
			return;
		}
		object obj = null;
		switch (propertyType)
		{
		case PropertyTypeInfo.Int:
			obj = (int)curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Long:
			obj = (long)curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Float:
			obj = curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Double:
			obj = (double)curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Bool:
		{
			bool flag5 = false;
			USInternalKeyframe[] keys = curves[0].Keys;
			foreach (USInternalKeyframe uSInternalKeyframe in keys)
			{
				if (uSInternalKeyframe.Time > time)
				{
					break;
				}
				if (uSInternalKeyframe.Value >= 1f)
				{
					flag5 = true;
				}
				else if (uSInternalKeyframe.Value <= 0f)
				{
					flag5 = false;
				}
			}
			if (curves[0].Evaluate(time) >= 1f)
			{
				tmpBool = true;
			}
			else if (curves[0].Evaluate(time) <= 0f)
			{
				tmpBool = false;
			}
			else
			{
				tmpBool = flag5;
			}
			obj = tmpBool;
			break;
		}
		case PropertyTypeInfo.Vec2:
			tmpVector2.x = curves[0].Evaluate(time);
			tmpVector2.y = curves[1].Evaluate(time);
			obj = tmpVector2;
			break;
		case PropertyTypeInfo.Vec3:
			tmpVector3.x = curves[0].Evaluate(time);
			tmpVector3.y = curves[1].Evaluate(time);
			tmpVector3.z = curves[2].Evaluate(time);
			obj = tmpVector3;
			break;
		case PropertyTypeInfo.Vec4:
			tmpVector4.x = curves[0].Evaluate(time);
			tmpVector4.y = curves[1].Evaluate(time);
			tmpVector4.z = curves[2].Evaluate(time);
			tmpVector4.w = curves[3].Evaluate(time);
			obj = tmpVector4;
			break;
		case PropertyTypeInfo.Quat:
			tmpQuat.x = curves[0].Evaluate(time);
			tmpQuat.y = curves[1].Evaluate(time);
			tmpQuat.z = curves[2].Evaluate(time);
			tmpQuat.w = curves[3].Evaluate(time);
			obj = tmpQuat;
			break;
		case PropertyTypeInfo.Colour:
			tmpColour.r = curves[0].Evaluate(time);
			tmpColour.g = curves[1].Evaluate(time);
			tmpColour.b = curves[2].Evaluate(time);
			tmpColour.a = curves[3].Evaluate(time);
			obj = tmpColour;
			break;
		}
		if (obj != null && !(component == null))
		{
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(component, obj);
			}
			if (propertyInfo != null)
			{
				propertyInfo.SetValue(component, obj, null);
			}
		}
	}

	public void AddKeyframe(List<USInternalCurve> settingCurves, object ourValue, float time, bool autoSmooth)
	{
		if (settingCurves.Count == 0)
		{
			return;
		}
		if (propertyType == PropertyTypeInfo.None)
		{
			Debug.LogError("Atempting to Add A Keyframe, before calling CreatePropertyInfo");
			return;
		}
		if (GetMappedType(ourValue.GetType()) != propertyType)
		{
			Debug.LogError("Trying to Add a Keyframe of the wrong type");
			return;
		}
		switch (GetMappedType(ourValue.GetType()))
		{
		case PropertyTypeInfo.Int:
			AddKeyframe((int)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Long:
			AddKeyframe((long)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Float:
			AddKeyframe((float)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Double:
			AddKeyframe((double)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Bool:
			AddKeyframe((bool)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Vec2:
		{
			Vector2 vector = (Vector2)ourValue;
			tmpKeyframe.time = time;
			if (settingCurves.Contains(curves[0]))
			{
				tmpKeyframe.value = vector.x;
				AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[1]))
			{
				tmpKeyframe.value = vector.y;
				AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
			}
			break;
		}
		case PropertyTypeInfo.Vec3:
		{
			Vector3 vector3 = (Vector3)ourValue;
			tmpKeyframe.time = time;
			if (settingCurves.Contains(curves[0]))
			{
				tmpKeyframe.value = vector3.x;
				AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[1]))
			{
				tmpKeyframe.value = vector3.y;
				AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[2]))
			{
				tmpKeyframe.value = vector3.z;
				AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
			}
			break;
		}
		case PropertyTypeInfo.Vec4:
		{
			Vector4 vector2 = (Vector4)ourValue;
			tmpKeyframe.time = time;
			if (settingCurves.Contains(curves[0]))
			{
				tmpKeyframe.value = vector2.x;
				AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[1]))
			{
				tmpKeyframe.value = vector2.y;
				AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[2]))
			{
				tmpKeyframe.value = vector2.z;
				AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[3]))
			{
				tmpKeyframe.value = vector2.w;
				AddKeyframe(curves[3], tmpKeyframe, autoSmooth);
			}
			break;
		}
		case PropertyTypeInfo.Quat:
		{
			Quaternion quaternion = (Quaternion)ourValue;
			tmpKeyframe.time = time;
			if (settingCurves.Contains(curves[0]))
			{
				tmpKeyframe.value = quaternion.x;
				AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[1]))
			{
				tmpKeyframe.value = quaternion.y;
				AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[2]))
			{
				tmpKeyframe.value = quaternion.z;
				AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[3]))
			{
				tmpKeyframe.value = quaternion.w;
				AddKeyframe(curves[3], tmpKeyframe, autoSmooth);
			}
			break;
		}
		case PropertyTypeInfo.Colour:
		{
			Color color = (Color)ourValue;
			tmpKeyframe.time = time;
			if (settingCurves.Contains(curves[0]))
			{
				tmpKeyframe.value = color.r;
				AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[1]))
			{
				tmpKeyframe.value = color.g;
				AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[2]))
			{
				tmpKeyframe.value = color.b;
				AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
			}
			if (settingCurves.Contains(curves[3]))
			{
				tmpKeyframe.value = color.a;
				AddKeyframe(curves[3], tmpKeyframe, autoSmooth);
			}
			break;
		}
		}
	}

	public void AddKeyframe(object ourValue, float time, bool autoSmooth)
	{
		if (propertyType == PropertyTypeInfo.None)
		{
			Debug.LogError("Atempting to Add A Keyframe, before calling CreatePropertyInfo");
			return;
		}
		if (GetMappedType(ourValue.GetType()) != propertyType)
		{
			Debug.LogError("Trying to Add a Keyframe of the wrong type");
			return;
		}
		switch (GetMappedType(ourValue.GetType()))
		{
		case PropertyTypeInfo.Int:
			AddKeyframe((int)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Long:
			AddKeyframe((long)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Float:
			AddKeyframe((float)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Double:
			AddKeyframe((double)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Bool:
			AddKeyframe((bool)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Vec2:
			AddKeyframe((Vector2)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Vec3:
			AddKeyframe((Vector3)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Vec4:
			AddKeyframe((Vector4)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Quat:
			AddKeyframe((Quaternion)ourValue, time, autoSmooth);
			break;
		case PropertyTypeInfo.Colour:
			AddKeyframe((Color)ourValue, time, autoSmooth);
			break;
		}
	}

	private void AddKeyframe(USInternalCurve curve, Keyframe keyframe, bool autoSmooth)
	{
		USInternalKeyframe uSInternalKeyframe = curve.AddKeyframe(keyframe.time, keyframe.value);
		if (propertyType == PropertyTypeInfo.Bool)
		{
			autoSmooth = false;
			uSInternalKeyframe.RightTangentConstant();
			uSInternalKeyframe.LeftTangentConstant();
		}
		if (autoSmooth)
		{
			uSInternalKeyframe.Smooth();
		}
		SetValue(keyframe.time);
	}

	private void AddKeyframe(int ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(long ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(float ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(double ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = (float)ourValue;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(bool ourValue, float time, bool autoSmooth)
	{
		float value = 0f;
		if (ourValue)
		{
			value = 1f;
		}
		tmpKeyframe.time = time;
		tmpKeyframe.value = value;
		AddKeyframe(curves[0], tmpKeyframe, false);
	}

	private void AddKeyframe(Vector2 ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.x;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.y;
		AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(Vector3 ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.x;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.y;
		AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.z;
		AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(Vector4 ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.x;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.y;
		AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.z;
		AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.w;
		AddKeyframe(curves[3], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(Quaternion ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.x;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.y;
		AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.z;
		AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.w;
		AddKeyframe(curves[3], tmpKeyframe, autoSmooth);
	}

	private void AddKeyframe(Color ourValue, float time, bool autoSmooth)
	{
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.r;
		AddKeyframe(curves[0], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.g;
		AddKeyframe(curves[1], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.b;
		AddKeyframe(curves[2], tmpKeyframe, autoSmooth);
		tmpKeyframe.time = time;
		tmpKeyframe.value = ourValue.a;
		AddKeyframe(curves[3], tmpKeyframe, autoSmooth);
	}

	private void CreatePropertyCurves()
	{
		switch (propertyType)
		{
		case PropertyTypeInfo.Int:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Long:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Float:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Double:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Bool:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Vec2:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			curves[1].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Vec3:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			curves[1].UnityAnimationCurve = new AnimationCurve();
			curves[2].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Vec4:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			curves[1].UnityAnimationCurve = new AnimationCurve();
			curves[2].UnityAnimationCurve = new AnimationCurve();
			curves[3].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Quat:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			curves[1].UnityAnimationCurve = new AnimationCurve();
			curves[2].UnityAnimationCurve = new AnimationCurve();
			curves[3].UnityAnimationCurve = new AnimationCurve();
			break;
		case PropertyTypeInfo.Colour:
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves.Add(ScriptableObject.CreateInstance<USInternalCurve>());
			curves[0].UnityAnimationCurve = new AnimationCurve();
			curves[1].UnityAnimationCurve = new AnimationCurve();
			curves[2].UnityAnimationCurve = new AnimationCurve();
			curves[3].UnityAnimationCurve = new AnimationCurve();
			break;
		}
	}

	public object GetValueForTime(float time)
	{
		object obj = null;
		switch (propertyType)
		{
		case PropertyTypeInfo.Int:
			obj = (int)curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Long:
			obj = (long)curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Float:
			obj = curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Double:
			obj = (double)curves[0].Evaluate(time);
			break;
		case PropertyTypeInfo.Bool:
		{
			bool flag = false;
			USInternalKeyframe[] keys = curves[0].Keys;
			foreach (USInternalKeyframe uSInternalKeyframe in keys)
			{
				if (uSInternalKeyframe.Time > time)
				{
					break;
				}
				if (uSInternalKeyframe.Value >= 1f)
				{
					flag = true;
				}
				else if (uSInternalKeyframe.Value <= 0f)
				{
					flag = false;
				}
			}
			if (curves[0].Evaluate(time) >= 1f)
			{
				tmpBool = true;
			}
			else if (curves[0].Evaluate(time) <= 0f)
			{
				tmpBool = false;
			}
			else
			{
				tmpBool = flag;
			}
			obj = tmpBool;
			break;
		}
		case PropertyTypeInfo.Vec2:
			tmpVector2.x = curves[0].Evaluate(time);
			tmpVector2.y = curves[1].Evaluate(time);
			obj = tmpVector2;
			break;
		case PropertyTypeInfo.Vec3:
			tmpVector3.x = curves[0].Evaluate(time);
			tmpVector3.y = curves[1].Evaluate(time);
			tmpVector3.z = curves[2].Evaluate(time);
			obj = tmpVector3;
			break;
		case PropertyTypeInfo.Vec4:
			tmpVector4.x = curves[0].Evaluate(time);
			tmpVector4.y = curves[1].Evaluate(time);
			tmpVector4.z = curves[2].Evaluate(time);
			tmpVector4.w = curves[3].Evaluate(time);
			obj = tmpVector4;
			break;
		case PropertyTypeInfo.Quat:
			tmpQuat.x = curves[0].Evaluate(time);
			tmpQuat.y = curves[1].Evaluate(time);
			tmpQuat.z = curves[2].Evaluate(time);
			tmpQuat.w = curves[3].Evaluate(time);
			obj = tmpQuat;
			break;
		case PropertyTypeInfo.Colour:
			tmpColour.r = curves[0].Evaluate(time);
			tmpColour.g = curves[1].Evaluate(time);
			tmpColour.b = curves[2].Evaluate(time);
			tmpColour.a = curves[3].Evaluate(time);
			obj = tmpColour;
			break;
		}
		if (obj == null)
		{
			return null;
		}
		return obj;
	}

	public bool HasPropertyBeenModified(float runningTime)
	{
		if (!component)
		{
			return false;
		}
		bool flag = false;
		foreach (USInternalCurve curf in curves)
		{
			if (runningTime >= curf.FirstKeyframeTime && runningTime <= curf.LastKeyframeTime)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return false;
		}
		object obj = null;
		if (fieldInfo != null)
		{
			obj = fieldInfo.GetValue(component);
		}
		if (propertyInfo != null)
		{
			obj = propertyInfo.GetValue(component, null);
		}
		object valueForTime = GetValueForTime(runningTime);
		if (obj == null || valueForTime == null)
		{
			return false;
		}
		if (obj.GetType() != valueForTime.GetType())
		{
			return false;
		}
		switch (propertyType)
		{
		case PropertyTypeInfo.Int:
			return (int)obj != (int)valueForTime;
		case PropertyTypeInfo.Long:
			return (long)obj != (long)valueForTime;
		case PropertyTypeInfo.Float:
			return !Mathf.Approximately((float)obj, (float)valueForTime);
		case PropertyTypeInfo.Double:
			return !Mathf.Approximately((float)(double)obj, (float)(double)valueForTime);
		case PropertyTypeInfo.Bool:
			return (bool)obj != (bool)valueForTime;
		case PropertyTypeInfo.Vec2:
		{
			tmpVector2 = (Vector2)obj;
			Vector2 vector3 = (Vector2)valueForTime;
			return tmpVector2 != vector3;
		}
		case PropertyTypeInfo.Vec3:
		{
			tmpVector3 = (Vector3)obj;
			Vector3 vector2 = (Vector3)valueForTime;
			return tmpVector3 != vector2;
		}
		case PropertyTypeInfo.Vec4:
		{
			tmpVector4 = (Vector4)obj;
			Vector4 vector = (Vector4)valueForTime;
			return tmpVector4 != vector;
		}
		case PropertyTypeInfo.Quat:
		{
			tmpQuat = (Quaternion)obj;
			Quaternion quaternion = (Quaternion)valueForTime;
			bool flag2 = Mathf.Abs(quaternion.eulerAngles.x - tmpQuat.eulerAngles.x) > 0.05f;
			bool flag3 = Mathf.Abs(quaternion.eulerAngles.y - tmpQuat.eulerAngles.y) > 0.05f;
			bool flag4 = Mathf.Abs(quaternion.eulerAngles.z - tmpQuat.eulerAngles.z) > 0.05f;
			return flag2 || flag3 || flag4;
		}
		case PropertyTypeInfo.Colour:
		{
			tmpColour = (Color)obj;
			Color color = (Color)valueForTime;
			return tmpColour != color;
		}
		default:
			return false;
		}
	}

	public static PropertyTypeInfo GetMappedType(Type type)
	{
		if (type == typeof(int))
		{
			return PropertyTypeInfo.Int;
		}
		if (type == typeof(long))
		{
			return PropertyTypeInfo.Long;
		}
		if (type == typeof(float) || type == typeof(float))
		{
			return PropertyTypeInfo.Float;
		}
		if (type == typeof(double))
		{
			return PropertyTypeInfo.Double;
		}
		if (type == typeof(bool))
		{
			return PropertyTypeInfo.Bool;
		}
		if (type == typeof(Vector2))
		{
			return PropertyTypeInfo.Vec2;
		}
		if (type == typeof(Vector3))
		{
			return PropertyTypeInfo.Vec3;
		}
		if (type == typeof(Vector4))
		{
			return PropertyTypeInfo.Vec4;
		}
		if (type == typeof(Quaternion))
		{
			return PropertyTypeInfo.Quat;
		}
		if (type == typeof(Color))
		{
			return PropertyTypeInfo.Colour;
		}
		return PropertyTypeInfo.None;
	}

	public static Type GetMappedType(PropertyTypeInfo type)
	{
		switch (type)
		{
		case PropertyTypeInfo.Int:
			return typeof(int);
		case PropertyTypeInfo.Long:
			return typeof(long);
		case PropertyTypeInfo.Float:
			return typeof(float);
		case PropertyTypeInfo.Double:
			return typeof(double);
		case PropertyTypeInfo.Bool:
			return typeof(bool);
		case PropertyTypeInfo.Vec2:
			return typeof(Vector2);
		case PropertyTypeInfo.Vec3:
			return typeof(Vector3);
		case PropertyTypeInfo.Vec4:
			return typeof(Vector4);
		case PropertyTypeInfo.Quat:
			return typeof(Quaternion);
		case PropertyTypeInfo.Colour:
			return typeof(Color);
		default:
			return null;
		}
	}
}
