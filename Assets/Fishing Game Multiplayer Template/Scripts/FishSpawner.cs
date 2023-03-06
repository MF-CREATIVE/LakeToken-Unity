using UnityEngine;
using Mirror;

public class FishSpawner : MonoBehaviour {

	[SerializeField] private GameObject _fishEntityBasePrefab;

	public static FishSpawner instance;
	public FishSpawner() {
		if (instance == null) {
			instance = this;
		}
		else {
			throw new UnityException("instance != null");
		}
	}

	public void Spawn(Vector3 position, int fishUniqueId) { // (Server)
		GameObject fishEntityObj = Instantiate(_fishEntityBasePrefab);
		fishEntityObj.transform.position = position;
		FishEntity fishEntity = fishEntityObj.GetComponent<FishEntity>();
		fishEntity.fishUniqueId = fishUniqueId;
		NetworkServer.Spawn(fishEntityObj);
	}
}
