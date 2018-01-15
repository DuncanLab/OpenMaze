using trial;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DS = data.DataSingleton;

namespace main
{
	
	
	
	public class CommitTextBoxData : MonoBehaviour
	{
		public InputField[] Fields;

		private void Start()
		{
			Loader.Get().CurrTrial = new FieldTrial(Fields);
		}
	}
}
