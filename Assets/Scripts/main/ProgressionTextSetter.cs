using System;
using data;
using UnityEngine;
using UnityEngine.UI;
using DS = data.DataSingleton;
using E = main.Loader;
using C = data.Constants;
namespace main
{
	public class ProgressionTextSetter : MonoBehaviour
	{
		//Sets the image of the loading screen.
		public void Start()
		{
			Debug.Log("Entering Loading Screen: " + E.Get().CurrTrial.TrialID);
			var filePath = C.InputDirectory + E.Get().CurrTrial.Value.FileLocation;
			GetComponent<RawImage>().texture = Img2Sprite.LoadTexture(filePath);
		}		
	}
}