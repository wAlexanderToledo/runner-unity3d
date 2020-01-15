using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class PlayerMovement : MonoBehaviour
{
	public float force = 400.0f;
	private Rigidbody player;
	GameManager myGameManager;
	BoxCollider myTrigger;
	int triggerCounter = 0;
	int triggerExitCounter = 0;
	float otherXHolder1 = 0;
	float otherXHolder2 = 0;
	int orderedXHolder1 = 0;
	int orderedXHolder2 = 0;
	int randomHolder = 0;

	//Más grande, más estúpido es:
	public int inteligenceBias = 2;

	int[] holders;
	float[] probabilities;

	Dictionary <string, float> successProbability;

	//----------------------------------------------
	public bool isTesting = false;
	int debugCount = 0;

	private void Start()
	{
		player = this.GetComponent<Rigidbody>();
		player.drag = 0.25f;
		player.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		player.interpolation = RigidbodyInterpolation.Interpolate;
		force = 350;
		myGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		myTrigger = gameObject.AddComponent<BoxCollider>();
		myTrigger.isTrigger = true;
		myTrigger.size = new Vector3(30, 2, 25);
		myTrigger.center = new Vector3(0, 0, 8);
		//LOAD IF EXISTS:
		using (StreamReader file = File.OpenText(@"d:\successProbability.json"))
        {
			if(file != null)
			{
				JsonSerializer serializer = new JsonSerializer();
				successProbability = (Dictionary<string, float>)serializer.Deserialize(file, typeof(Dictionary<string, float>));
			}
			if (successProbability == null)
			{
				successProbability = new Dictionary <string, float>();
			}
        }
		holders =  new int[14];
		probabilities =  new float[14];

	}

	private void OnTriggerEnter(Collider other)
    {
		if (isTesting)
		{
			if (other.tag == "Obstacle")
			{
				triggerCounter++;
				if (triggerCounter < 2)
				{
					otherXHolder1 = other.transform.position.x;
				}
				if (triggerCounter == 2)
				{
					otherXHolder2 = other.transform.position.x;
					if (otherXHolder1 < otherXHolder2)
					{
						orderedXHolder1 = Mathf.RoundToInt(otherXHolder1);
						orderedXHolder2 = Mathf.RoundToInt(otherXHolder2);
					}
					else
					{
						orderedXHolder1 = Mathf.RoundToInt(otherXHolder2);
						orderedXHolder2 = Mathf.RoundToInt(otherXHolder1);
					}
					//ESCOGER DEL DICCIONARIO y/o al azar
					int arrayCount = 0;
					string tempKey;
					for (int i = -7; i < 8; i++)
					{
						tempKey = orderedXHolder1.ToString() + "," + orderedXHolder2.ToString() + "," + i.ToString();
						if (successProbability.ContainsKey(tempKey) && successProbability[tempKey] > 0.3f)
						{
							holders[arrayCount] = i;
							probabilities[arrayCount] = successProbability[tempKey];
							arrayCount++;
						}
					}
					int randomArray;
					if (arrayCount > inteligenceBias)
					{
						randomArray = Random.Range(0, arrayCount);
						randomHolder = holders[randomArray];
					}
					else
					{
						randomHolder = Random.Range(-7, 8);
					}
					triggerCounter = 0;
				}
			}
		}
    }
	private void OnTriggerExit(Collider other)
    {
		if (isTesting)
		{
			if (triggerExitCounter < 2)
			{
				triggerExitCounter++;
			}
			if (triggerExitCounter == 2)
			{
				//Guardar éxito en el diccionario
				//SHOULD ALWAYS STAY ON:
				//--------------------------------------------------------------------------------------------------------------------------------------------
				//Debug.Log(debugCount + ") YAY x1: " + orderedXHolder1.ToString() + " x2: " + orderedXHolder2.ToString() + " x3: " + randomHolder.ToString());
				//debugCount++;
				//---------------------------
				float tempProbability = 0;
				string tempKey = orderedXHolder1.ToString() + "," + orderedXHolder2.ToString() + "," + randomHolder.ToString();
				if (successProbability.ContainsKey(tempKey))
				{
					tempProbability = successProbability[tempKey];
					tempProbability = Mathf.Clamp01(tempProbability + 0.3f);
					successProbability.Remove(tempKey);
					successProbability.Add(tempKey, tempProbability);
				}
				else
				{
					successProbability.Add(tempKey, 0.8f);
				}
				//--------------------------------------------------------------------------------------------
				//Debug.Log(debugCount + ") Key: " + tempKey + " probability" + successProbability[tempKey] );		
				//debugCount++;
				//--------------------------------------------------------------------------------------------
				triggerExitCounter = 0;
				//Debug.Break();
			}
		}
    }

	private void FixedUpdate()
	{
		if (isTesting)
		{
			if (Mathf.Abs(player.position.x - randomHolder) <= 0.27f)
			{
				player.velocity = Vector3.zero;
				player.position = new Vector3(randomHolder, player.position.y, player.position.z);
			}
			else if (player.position.x < randomHolder)
			{
				GoRight();
			}
			else if (player.position.x > randomHolder)
			{
				GoLeft();
			}
		}
		else
		{
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				GoRight();
			}

			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				GoLeft();
			}
			else if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow))
			{
				player.velocity = Vector3.zero;
			}
		}
	}

	private void GoRight()
	{
		player.AddForce(force * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
	}
	private void GoLeft()
	{
		player.AddForce(-force * Time.deltaTime, 0f, 0f, ForceMode.VelocityChange);
	}

	void OnDisable()
    {
		myTrigger.enabled = false;
		if (isTesting)
		{
			//SHOULD ALWAYS STAY ON:
			//--------------------------------------------------------------------------------------------------------------------------------------------
			//Debug.Log(debugCount + ") FAIL x1: " + orderedXHolder1.ToString() + " x2: " + orderedXHolder2.ToString() + " x3: " + randomHolder.ToString());
			//debugCount++;
			//---------------
			float tempProbability = 0;
			string tempKey = orderedXHolder1.ToString() + "," + orderedXHolder2.ToString() + "," + randomHolder.ToString();
			if (successProbability.ContainsKey(tempKey))
			{
				tempProbability = successProbability[tempKey];
				tempProbability = Mathf.Clamp01(tempProbability - 0.1f);
				successProbability.Remove(tempKey);
				successProbability.Add(tempKey, tempProbability);
			}
			else
			{
				successProbability.Add(tempKey, 0.4f);
			}
			//Debug.Log("Key: " + tempKey + " probability" + successProbability[tempKey] );
			//Mandar error al diccionario		
			using (StreamWriter file = File.CreateText(@"d:\successProbability.json"))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(file, successProbability);
			}
			//Debug.Break();
			if (myGameManager != null)
			{
				myGameManager.SwitchScene(1);
			}
		}
    }
}
