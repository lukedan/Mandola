using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitching : MonoBehaviour {
	public List<GameObject> GunPrefabs;
	public List<string> SwitchGunKeys;

	public Vector3 LocalPos = new Vector3(0.0f, 0.9f, 0.0f);

	public void Update() {
		for (int i = 0; i < SwitchGunKeys.Count; ++i) {
			if (Input.GetKeyDown(SwitchGunKeys[i])) {
				GameObject prefab = GunPrefabs[i];
				GunBase gunInHand = GetComponentInChildren<GunBase>();
				if (prefab.GetComponent<GunBase>().Identifier != gunInHand.Identifier) {
					gunInHand.transform.parent = null;
					gunInHand.OnDestroying();
					Destroy(gunInHand.gameObject);
					GameObject newGun = Instantiate(prefab);
					newGun.transform.parent = transform;
					newGun.transform.localPosition = LocalPos;
				}
				break;
			}
		}
	}
}
