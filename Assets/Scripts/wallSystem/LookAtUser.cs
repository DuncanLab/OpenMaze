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
		var val = E.Get().CurrTrial.Value.PickupType;
		if (val == 0)
		{
			val++;
		}
		var item = DS.GetData ().PickupItems [Mathf.Abs(val) - 1];
		var sprite = item.ImageLoc;

		_player = GameObject.Find("FirstPerson");
		var pic = Img2Sprite.LoadNewSprite(C.InputDirectory + sprite);
		print(pic.pivot);
		GetComponent<SpriteRenderer>().sprite = pic;
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
