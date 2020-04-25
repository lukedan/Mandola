using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlobalPostProcessingVolume : MonoBehaviour {
	/// <summary>
	/// The global post processing volume.
	/// </summary>
	public static PostProcessVolume GlobalVolume;

	private void Start() {
		PostProcessVolume volume = GetComponent<PostProcessVolume>();
		if (volume && volume.isGlobal) {
			GlobalVolume = volume;
		}
	}
}
