using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// This script is required because the animator only works with sprite renderers
// UI uses Images which are fundamentally different from sprites....
// sourced from : https://gist.github.com/almirage/e9e4f447190371ee6ce9
// Modified slightly to be framerate independent...
public class ImageAnimation : MonoBehaviour {

	public Sprite[] sprites;
	public float TimePerFrame = 1;
	public bool loop = true;
	public bool destroyOnEnd = false;

	private int index = 0;
	private Image image;
	private float frame_time = 0;

	void Awake() {
		image = GetComponent<Image> ();
	}

	void Update () {
		if (!loop && index == sprites.Length) return;
        frame_time += Time.deltaTime;
        if (frame_time < TimePerFrame) return;
		frame_time = 0;

		image.sprite = sprites [index];
		index ++;

		if (index >= sprites.Length) {
			if (loop) index = 0;
			if (destroyOnEnd) Destroy (gameObject);
		}
	}
}