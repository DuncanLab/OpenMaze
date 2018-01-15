using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DS = data.DataSingleton;

namespace main
{
	
	
	
	public class CommitTextBoxData : MonoBehaviour
	{
		public InputField[] Fields;
		
		// Update is called once per frame
		private void Update () {
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Loader.LogData("", false);

				foreach (var textBox in Fields)
				{
					var arr = textBox.transform.GetComponentsInChildren<Text>();
					Loader.LogData(arr[0].text + ": " + arr[1].text);
					DS.GetData().CharacterData.OutputFile = arr[1].text + "_" + DS.GetData().CharacterData.OutputFile;
				}				
					
			}
		}
	}
}
