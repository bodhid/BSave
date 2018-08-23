//Types that can be serialized
//https://docs.unity3d.com/Manual/script-Serialization.html#FieldSerliaized2
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class SaveData
{
	public enum Enum
	{
		Small = 1,
		Medium = 2,
		Large = 4
	}
	public Primitives primitives;
	public Lists lists;
	public UnityTypes unityTypes;
	public Enum enumValue;
	[Serializable]
	public class Primitives
	{
		public short shortValue;
		public int intValue;
		public long longValue;
		public UInt16 uint16Value;
		public UInt32 uint32Value;
		public UInt64 uint64Value;
		public sbyte sbyteValue;
		public byte byteValue;
		public float floatValue;
		public double doubleValue;
		public decimal decimalValue;
		public bool booleanValue;
		public char charValue;
	}
	[Serializable]
	public class Lists
	{
		public List<string> listOfStrings;
		public string[] arrayOfStrings;
	}
	[Serializable]
	public class UnityTypes
	{

		public Vector2 vector2;
		public Vector3 vector3;
		public Vector4 vector4;
		public Rect rect;
		public Quaternion quaternion;
		public Matrix4x4 matrix4x4;
		public Color color;
		public Color32 color32;
		public LayerMask layerMask;
		public AnimationCurve animationCurve;
		public Gradient gradient;
		public RectOffset rectOffset;
		public GUIStyle guiStyle;
	}
}
