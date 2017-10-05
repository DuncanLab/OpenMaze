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

		public void Start()
		{
			var filePath = C.InputDirectory + E.Get().CurrTrial.Value.FileLocation;
			GetComponent<RawImage>().texture = Img2Sprite.LoadTexture(filePath);
		}		
	}
}