using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kvant;
using Reaktion;

[AddComponentMenu("Reaktion/Gear/Transform Gear")]
public class RotatingObject : MonoBehaviour {

	//Assigned in inspector
	public Spray leaves;
	public Spray rocks;
	public ReaktorLink reaktor;

	//Logic
	bool animating = false;
	float animationTimer = 0f;

	//Building and tree game objects
	public GameObject building;
	public GameObject tree;

	void Awake() {
		//Init the audio plugin
		reaktor.Initialize(this);

		//Set the tree to inactive and building to active
		tree.SetActive (false);
		building.SetActive (true);

		rocks.scale = 0f;

		//Logic
		animating = false;
	}
	
	void Update () {

		//Rotate the entire object slowly
		transform.Rotate(transform.up, Time.deltaTime * 5);

		//Update based on db level. It's a float 0-1f
		float ro = reaktor.Output;

		#region Starting animation
		//Animation timing logic
		if (ro > 0.1f) {
			//Begin animating
			if (animationTimer <= 0f) {
				animating = true;
			}

			//Set scale of leaves according to animation
			if (animating && animationTimer == 0f) {

				if (ro < 0.5f) {
					leaves.scale = ro * 1f;
				} else {
					leaves.scale = ro * 2.5f;
				}

			}

			//Animation reaches peak db
			if (ro >= 0.95f && animationTimer == 0f) {
				animationTimer = 6f;
				//Set fixed leaves position
				leaves.gameObject.GetComponent<TransformGear> ().position.min = -1;
				rocks.scale = 0f;

				//change to tree
				tree.SetActive(true);
				building.SetActive (false);
			}
		}
		#endregion

		#region Ending Animation
		if (animationTimer > 0f) { 
			animationTimer -= Time.deltaTime;

			//Slowly decrease scale of leaves
			if (animationTimer < 3f && leaves.scale > 0) {
				leaves.scale -= Time.deltaTime * 0.5f;

				if (leaves.scale < 0f) { 
					leaves.scale = 0f;
				}
			}

			if (animationTimer < 3f) {
				rocks.scale += Time.deltaTime * 0.4f;
			}

			if (animationTimer <= 0f) {
				//Set leaves scale to zero and reset animation logic
				animating = false;
				leaves.scale = 0f;
				animationTimer = 0f;

				//Set building to active
				tree.SetActive(false);
				building.SetActive (true);

				//Set leaves position to bottom
				leaves.gameObject.GetComponent<TransformGear> ().position.min = -3;
			}
		}

		//Scale rocks down if animation is done
		if (rocks.scale > 0f && animationTimer == 0f) {
			rocks.scale -= Time.deltaTime * 1f;

			if (rocks.scale < 0f) {
				rocks.scale = 0f;
			}
		}
		#endregion
	}
}
