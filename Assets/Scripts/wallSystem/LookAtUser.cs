using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using data;
using UnityEditor;
using UnityEngine;
using C = data.Constants;
using E = main.Loader;
using DS = data.DataSingleton;
public class LookAtUser : MonoBehaviour
{
	private GameObject _player;
	
	// Use this for initialization
	private void Start ()
	{
		_player = GameObject.Find("FirstPerson");

	}
	
	// Update is called once per frame
	private void Update ()
	{
		var origin = transform.position - _player.transform.position;
		origin = origin.normalized;
		
		var a = Mathf.Rad2Deg * Mathf.Atan2(origin.x, origin.z);
 
		transform.rotation = Quaternion.Euler(0, a, 0);		
	}
}
