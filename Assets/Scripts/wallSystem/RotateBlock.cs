using UnityEngine;
using DS = data.DataSingleton;
namespace wallSystem
{
	public class RotateBlock : MonoBehaviour {
	
	
	
	
	
		// Update is called once per frame
		private void Update () {
			// ...also rotate around the World's Y axis
			transform.Rotate(0, 10 * Time.deltaTime * DS.GetData().CharacterData.PickupRotationSpeed, 0, Space.World);
		}
	}
}
