using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {
    [Header("Settings")]
    public float Speed = 1.0f;

	void Update () {

		this.transform.Rotate(0, Speed/2, 0);
	}
}
