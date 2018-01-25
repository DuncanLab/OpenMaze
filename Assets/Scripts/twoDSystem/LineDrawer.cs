using data;
using UnityEngine;
using UnityEngine.UI;
using wallSystem;
using E = main.Loader;
using DS = data.DataSingleton;
namespace twoDSystem
{
	public class LineDrawer : MonoBehaviour
	{
		public GameObject Wall;

		private void Start()
		{
			GameObject.Find("CountDown").GetComponent<Text>().text = "";
			
			var goalText = GameObject.Find("Goal").GetComponent<Text>();
			goalText.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 30);
			goalText.text = E.Get().CurrTrial.Value.Header ?? "Test";
			goalText.color = Color.black;
			
			GenerateWalls();
			var previousTrial = E.Get().CurrTrial.TrialProgress.PreviousTrial;

			foreach (var p in DS.GetData().Pillars)
			{
				
				
				var cylin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			
				cylin.transform.position = new Vector3(p.X, 4.5F, p.Y);
				cylin.transform.localScale = new Vector3(p.Radius, 1F, p.Radius);
				cylin.GetComponent<Renderer>().material.color = Data.GetColour(previousTrial.Value.PillarColor);
				cylin.GetComponent<Renderer>().material.color = Color.black;
			}

		}

		
		private void GenerateWalls()
		{
			var previousTrial = E.Get().CurrTrial.TrialProgress.PreviousTrial;
			//This computes the current interior angle of the given side.
			var interiorAngle = 360f / previousTrial.Value.Sides; //This is, of course, given as 360 / num sides

			//This sets the initial angle to the one given in the preset
			float currentAngle = 0;
		
	
			//Here we interate through all the sides
			for (var i = 0; i < previousTrial.Value.Sides; i++)
			{
				//We compute the sin and cos of the current angle (essentially plotting points on a circle
				var x = GenerateWall.Cos(currentAngle) * previousTrial.Value.Radius;
				var y = GenerateWall.Sin(currentAngle) * previousTrial.Value.Radius;
			
				//This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
				//Such as:
				//Gaps appearing in large wall numbers
				//Desealing some stuff. so, bad.
				var length = 2 * previousTrial.Value.Radius * GenerateWall.Tan(180f / previousTrial.Value.Sides);
				
			
				//Here we create the wall
				var obj = Instantiate(Wall,
					new Vector3(x, 0.1F, y),
					Quaternion.identity
				);


				//So we add 10 because the end user won't be able to notice it anyways
				obj.transform.localScale = new Vector3(length, 4F, 0.5f);

				//This rotates the walls by the current angle + 90
				obj.transform.Rotate(Quaternion.Euler(0, - currentAngle - 90, 0).eulerAngles);

				//And we add the wall to the created list as to remove it later

				//And of course we increment the interior angle.
				currentAngle += interiorAngle;
			}
		}
		
	}
}
