using System.Collections;
using UnityEngine;

public static class extension {

	public enum RotationAxis
	{
		Up, Down, Right, Left, Forward, Backward
	}
	public enum RotationSpace
	{
		Global, Local
	}
	public static Vector3 LookAtOneAxis(this Transform trans, Vector3 pos, Vector3 posPointingDirection, Vector3 axis, RotationAxis rotAxis = RotationAxis.Up, RotationSpace rotSpace = RotationSpace.Local)
	{
		Vector3 direction = (pos - trans.position).normalized;
		direction.y = 0f;
		float angle = Vector3.SignedAngle (posPointingDirection, direction, axis);
		if (rotAxis == RotationAxis.Up || rotAxis == RotationAxis.Down) {
			if (rotSpace == RotationSpace.Local) {
				angle += trans.localEulerAngles.y;
				trans.localEulerAngles = new Vector3 (trans.localEulerAngles.x, angle, trans.localEulerAngles.z);
			} else {
				angle += trans.eulerAngles.y;
				trans.eulerAngles = new Vector3 (trans.eulerAngles.x, angle, trans.eulerAngles.z);
			}
		} else if (rotAxis == RotationAxis.Left || rotAxis == RotationAxis.Right) {
			
			if (rotSpace == RotationSpace.Local) {
				angle += trans.localEulerAngles.x;
				trans.localEulerAngles = new Vector3 (angle, trans.localEulerAngles.y, trans.localEulerAngles.z);
			} else {
				angle += trans.eulerAngles.x;
				trans.eulerAngles = new Vector3 (angle, trans.eulerAngles.y, trans.eulerAngles.z);
			}
		} else if (rotAxis == RotationAxis.Forward || rotAxis == RotationAxis.Backward) {
			
			if (rotSpace == RotationSpace.Local) {
				angle += trans.localEulerAngles.z;
				trans.localEulerAngles = new Vector3 (trans.localEulerAngles.x, trans.localEulerAngles.y, angle);
			} else {
				angle += trans.eulerAngles.z;
				trans.eulerAngles = new Vector3 (trans.eulerAngles.x, trans.eulerAngles.y, angle);
			}
		}

		Vector3 a;
		if (rotSpace == RotationSpace.Local)
			a = trans.localEulerAngles;
		else
			a = trans.eulerAngles;

		return a;
	}
}
