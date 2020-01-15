using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Tests;

namespace MyGame
{
	public class MainMenu : MonoBehaviour
	{
		public Fade fade;
		public NewTestScript newTestScript;

		IEnumerator SceneTransition(int scene)
		{
			yield return new WaitForSeconds(fade.BeginFade(1));

			SceneManager.LoadScene(scene, LoadSceneMode.Single);
		}

		public void ExitGame()
		{
			Application.Quit();
		}
		//añadir parámetros para el isTesting
		public void StartGame(int difficulty, bool isTesting)
		{
			GameManager.difficulty = difficulty;
			GameManager.SetIsTestingTrue();
			StartCoroutine(SceneTransition(1));

			/*if (isTesting)
			{
				newTestScript.PreSetupScene2();
			}*/
		}

		public void StartGame(int difficulty)
		{
			GameManager.difficulty = difficulty;

			StartCoroutine(SceneTransition(1));
		}

		private void Start()
		{
			fade.BeginFade(-1);
			//StartGame(1);
		}
	}
}

