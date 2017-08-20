using UnityEngine;
using UnityEngine.SceneManagement;
using DS = data.DataSingleton;
using E = systems.ExperimentManager;
using C = data.Constants;

namespace systems
{
	public class Loader : MonoBehaviour
	{

		// Use this for initialization
		private void Start () {
			DS.Load ();
			E.Init();
		
			DontDestroyOnLoad(this);	
			SceneManager.LoadScene(C.LoadingScreen);
		}


		private void Update()
		{
			if (Input.GetKey(KeyCode.Space))
			{
				E.Get().CatchEvent(E.State.SpaceDown);
			}
		}
	}
}
