using UnityEngine;
using System.Collections;

public class ItemRotate : MonoBehaviour 
{
	void Start () 
	{
	
	}
	
	void Update () 
	{
		transform.Rotate(Vector3.up * -100f * Time.deltaTime);
	
	}
}
