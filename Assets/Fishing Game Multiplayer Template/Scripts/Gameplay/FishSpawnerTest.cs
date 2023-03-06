using UnityEngine;

public class FishSpawnerTest : MonoBehaviour {

	[SerializeField] private Vector3 _position;
	[SerializeField] public int _fishUniqueId;

	private void Update() {
		if (Input.GetKeyDown(KeyCode.P)) {
			if (Mirror.NetworkServer.active) {
				FishSpawner.instance.Spawn(_position, _fishUniqueId);
			}
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Gizmos.DrawWireSphere(_position, 1f);
	}
#endif
}
