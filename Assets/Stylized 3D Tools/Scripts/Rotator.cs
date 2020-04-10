using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour 
{

	public enum RotationDirection 
	{
		animateRotationX,
		animateRotationY,
		animateRotationZ
	}
	public RotationDirection directionRotation;
	public float rotationSpeed = 0.0f;
	 

	void Update () 
	{

		switch(directionRotation)
		{
		case RotationDirection.animateRotationX:
			transform.Rotate(rotationSpeed * Time.deltaTime,0,0);
			break;
		case RotationDirection.animateRotationY:
			transform.Rotate(0,rotationSpeed * Time.deltaTime,0);
			break;
		case RotationDirection.animateRotationZ:
			transform.Rotate(0,0,rotationSpeed * Time.deltaTime);
			break;
			
		}
	}
}
