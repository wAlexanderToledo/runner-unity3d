using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
	public static int difficulty;
	private string[] difficultyNames = { "NaN", "Easy", "Medium", "Hard" };

	private float score;
	public Text scoreUI;
	private int highscore;
	public Text highscoreUI;

	public Transform player;
	public PlayerMovement movement;

	public GameObject obstaclePrefab;
	public Transform obstacles;
	public int obstacleStartX = 100;

	public GameObject deathOverlayUI;

	public Fade fade;
	public static bool isTesting;

	public static void SetIsTestingTrue()
	{
		isTesting = true;
	}

	IEnumerator DeathOverlayTransition()
	{
		yield return new WaitForSeconds(1);
		yield return new WaitForSeconds(fade.BeginFade(1));

		deathOverlayUI.SetActive(true);

		yield return new WaitForSeconds(fade.BeginFade(-1));
	}

	IEnumerator SceneTransition(int scene)
	{
		yield return new WaitForSeconds(fade.BeginFade(1));

		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}

	public void SwitchScene(int scene)
	{
		StartCoroutine(SceneTransition(scene));
	}

	public void InitiateDeath()
	{
		CancelInvoke("Spawn");

		FindObjectOfType<PlayerMovement>().enabled = false;

		foreach (Transform obstacle in obstacles)
		{
			obstacle.gameObject.GetComponent<ObstacleMovement>().enabled = false;
		}

		UpdateHighscore();
		highscoreUI.text = difficultyNames[difficulty] + " highscore: " + highscore;

		StartCoroutine(DeathOverlayTransition());
	}

	private void UpdateHighscore()
	{
		highscore = PlayerPrefs.GetInt("Highscore" + difficulty);

		if (score > highscore)
		{
			highscore = (int)score;
			PlayerPrefs.SetInt("Highscore" + difficulty, highscore);
		}
	}

	private void Spawn()
	{
		/* int i;
		//MISTAKE, THEY CAN COLLIDE FROM THE START
		// Spawn 2 new obstacles
		for (i = -7; i < 7; i += 7)
		{
			Instantiate(obstaclePrefab,
			            new Vector3(Mathf.Floor(Random.Range(i, i + 7)), 1, obstacleStartX),
			            Quaternion.identity, obstacles);
		}*/
		int x1 = Random.Range(-6, 1);
		GameObject obs1 = Instantiate(obstaclePrefab,
			            new Vector3(x1, 1, obstacleStartX),
			            Quaternion.identity, obstacles);
		obs1.tag = "Obstacle";
		Rigidbody obs1RB = obs1.GetComponent<Rigidbody>();
		obs1RB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		obs1RB.interpolation = RigidbodyInterpolation.Interpolate;
		int x2;
		if(x1 < 0)
		{
			x2 = Random.Range(1, 7);
		}
		else
		{
			x2 = Random.Range(2, 7);
		}
		GameObject obs2 = Instantiate(obstaclePrefab,
					new Vector3(x2, 1, obstacleStartX),
					Quaternion.identity, obstacles);
		obs2.tag = "Obstacle";
		Rigidbody obs2RB = obs2.GetComponent<Rigidbody>();
		obs2RB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		obs2RB.interpolation = RigidbodyInterpolation.Interpolate;
		//----------------------------------------------------------------------------------------------------------------------
		// Determine the best available approximation of the number 
		// of bytes currently allocated in managed memory.
		//Debug.Log("Total Memory: " + ((System.GC.GetTotalMemory(false)/1024.0f)/1024.0f).ToString("n2") + " MB");

	}

	private void Update()
	{
		if (FindObjectOfType<PlayerMovement>().enabled)
		{
			score += Time.deltaTime * 10;
			scoreUI.text = "Score: " + (int)score;
		}

		if (Input.GetKey("r"))
		{
			SwitchScene(1);
			return;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (deathOverlayUI.activeSelf)
			{
				SwitchScene(0);
			}
			else
			{
				InitiateDeath();
			}
			return;
		}
	}

	private void Start()
	{
		fade.BeginFade(-1);

		// Invoke obstacle spawning, frequency depends on difficulty
		InvokeRepeating("Spawn", 1f, 0.75f/difficulty);
		if (isTesting)
		{
			movement.isTesting = true;
		}
	}
}
