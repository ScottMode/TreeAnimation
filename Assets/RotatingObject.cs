using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kvant;
using Reaktion;

[AddComponentMenu("Reaktion/Gear/Transform Gear")]
public class RotatingObject : MonoBehaviour {

	//Assigned in inspector
	public Spray leaves;
	public ReaktorLink reaktor;

	//Logic
	bool animating = false;
	float animationTimer = 0f;

	public GameObject building;
	public GameObject tree;

	void Awake() {
		reaktor.Initialize(this);

		tree.SetActive (false);
		building.SetActive (true);

		//Logic
		animating = false;
	}
	
	void Update () {

		//Rotate the object
		transform.Rotate(transform.up, Time.deltaTime * 5);

		//Update based on volume
		float ro = reaktor.Output;
		//Debug.Log ("Output: " + ro);

		//Animation timing logic
		if (ro > 0.2f) {
			//Begin animating
			if (animationTimer <= 0f) {
				animating = true;
			}

			//Scale
			if (animating && animationTimer == 0f) {
				leaves.scale = ro * 3f;
			}

			if (ro >= 0.85f) {
				animationTimer = 4f;
				leaves.gameObject.GetComponent<TransformGear> ().position.min = -1;
				//change to tree
				tree.SetActive(true);
				building.SetActive (false);
			}
		} 
		else {
			//Check to end animation
			if (animationTimer > 0f) { 
				animationTimer -= Time.deltaTime;

				Debug.Log (animationTimer);

				if (leaves.scale > 0) {
					leaves.scale -= Time.deltaTime * 0.6f;

					if (leaves.scale < 0f) { 
						leaves.scale = 0f;
					}
				}

				if (animationTimer <= 0f) {
					animating = false;
					leaves.scale = 0f;
					animationTimer = 0f;

					tree.SetActive(false);
					building.SetActive (true);

					//Change to building
					leaves.gameObject.GetComponent<TransformGear> ().position.min = -3;
				}
			}
		}



		//Radius

		//Position
	}
}
