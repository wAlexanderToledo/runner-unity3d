using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MyGame;

namespace Tests
{
    public class NewTestScript
    {
		GameObject mainMenu;
		// A Test behaves as an ordinary method
		/*[Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }*/
		void SetupScene()
		{
			MonoBehaviour.Instantiate(Resources.Load<GameObject>("Root"));
		}

		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
		// `yield return null;` to skip a frame.
		[UnityTest]
		[Timeout(100000000)]
		public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
			SetupScene();
			mainMenu = GameObject.FindWithTag("MainMenu");
			mainMenu.GetComponent<MainMenu>().newTestScript = this;
			mainMenu.GetComponent<MainMenu>().StartGame(1, true);
			// Use the Assert class to test conditions.
			// Use yield to skip a frame.
			yield return new WaitForSeconds(300);
        }
    }
}
