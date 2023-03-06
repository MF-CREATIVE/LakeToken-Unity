using UnityEngine;
using Mirror;
using System.Collections;

public class FishEntity : NetworkBehaviour {

	[SyncVar] public int fishUniqueId;

	private static FishScriptable[] _fishScriptables;
	private FishScriptable _scriptable;
	public FishAIController controller;
    public GameObject FishCaughtMessage;
    public GameObject FishModel;

    private void Awake() {
		if (_fishScriptables == null) {
			_fishScriptables = Resources.LoadAll<FishScriptable>("Fish");
		}
	}

	private void Start() {
		for (int i = 0; i < _fishScriptables.Length; i++) {
			if (_fishScriptables[i].uniqueId == fishUniqueId) {
				_scriptable = _fishScriptables[i];
				break;
			}
		}
		if (_scriptable == null) {
			throw new UnityException("_scriptable == null");
		}

        FishModel = Instantiate(_scriptable.modelPrefab, transform);

		if (isServer) {
			controller = gameObject.AddComponent<FishAIController>();
			controller.Setup(_scriptable);
		}

		StartCoroutine(BiteLoop());
	}

#pragma warning disable IDE0051
	private void HookedChanged(FishingFloat oldValue, FishingFloat newValue) {
#pragma warning restore IDE0051
		if (oldValue != null) {
			oldValue.GetComponent<NetworkTransform>().clientAuthority = false;
			oldValue.transform.SetParent(null);
			oldValue._collider.enabled = true;
			oldValue._interactor.enabled = false;
			oldValue._rb.isKinematic = true;
			oldValue._rb.useGravity = false;
		}
		if (newValue != null) {
			newValue.GetComponent<NetworkTransform>().clientAuthority = false;
			newValue.transform.SetParent(transform);
			newValue._collider.enabled = false;
			newValue._interactor.enabled = false;
			newValue._rb.isKinematic = true;
			newValue._rb.useGravity = false;
		}
	}

	public Rigidbody rb;
	[SyncVar(hook = "HookedChanged")] private FishingFloat _hookedTo;
	private void Bite(FishingFloat _targetFloat) {
		if (_hookedTo == null) {
			_targetFloat.GetComponent<NetworkTransform>().clientAuthority = false;
			_targetFloat._collider.enabled = false;
			_targetFloat._interactor.enabled = false;
			_targetFloat._rb.isKinematic = true;
			_targetFloat._rb.useGravity = false;
			_targetFloat.transform.SetParent(transform);
            Vector3 NewFloatPosition = new Vector3(transform.position.x, transform.position.y + 0.0f, transform.position.z);
            _targetFloat.transform.position = NewFloatPosition;
            _hookedTo = _targetFloat;
			_hookedTo.fish = this;
			rb = gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;
			controller.stamina = 1f;
			controller.doNotUpdateTarget = true;
			controller.fearfulness = 1f;
            _hookedTo._owner.GetComponent<PlayerFishing>().SpawnedFloatSimulation.GetComponent<FloatSimulation>().SimulateBite();
        }
	}

	private IEnumerator BiteLoop() {
		while (true) {
			yield return new WaitForSeconds(1f);
			if (_hookedTo == null && controller != null) {
				if (controller.target != null && Random.Range(.0f, 1f) > .4f) {
					Bite(controller.target.gameObject.GetComponent<FishingFloat>());
				}
			}
		}
	}

	private void Update() {
		if (isServer) {
			if (_hookedTo == null && rb != null) {
				Destroy(rb);
				rb = null;
				controller.doNotUpdateTarget = false;
				controller.fearfulness = .0f;
			}

			if (_hookedTo != null) {
                Vector3 NewFishModelPosition = new Vector3(_hookedTo.Hook.transform.position.x, _hookedTo.Hook.transform.position.y - 0.15f, _hookedTo.Hook.transform.position.z);
                FishModel.transform.position = NewFishModelPosition;
                if (controller.stamina < 0.7f)
                {
                    FishModel.transform.LookAt(_hookedTo._owner._rodEndPoint.position);
                    /*Quaternion NewFishModelRotation = new Quaternion(FishModel.transform.rotation.x, 180, 180, 0);
                    FishModel.transform.rotation = Quaternion.Lerp(FishModel.transform.rotation, NewFishModelRotation, 5f);*/
                }
                if (controller.stamina > 0.7f)
                {
                    FishModel.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                if (Vector3.Distance(transform.position, _hookedTo._owner._rodEndPoint.position) < 2.3f) {
                    Debug.Log("Fish with ID " + this.GetComponent<FishAIController>()._scriptable.uniqueId + " caught!");
                    _hookedTo._owner.GetComponent<Animator>().Play(_hookedTo._owner.GetComponent<Inventory>().FishHolderAnimationName);
                    _hookedTo._owner.GetComponent<Inventory>().HoldCaughtFish(this.GetComponent<FishAIController>()._scriptable.uniqueId);
                    _hookedTo._owner.GetComponent<Inventory>().AddFishItem(this.GetComponent<FishAIController>()._scriptable.uniqueId, this.GetComponent<FishAIController>()._scriptable.FishName, this.GetComponent<FishAIController>()._scriptable.FishLength, "Weight: " + this.GetComponent<FishAIController>()._scriptable.FishWeight, this.GetComponent<FishAIController>()._scriptable.FishRetailValue, this.GetComponent<FishAIController>()._scriptable.FishSprite);
                    RpcHoldCaughtFish(this.GetComponent<FishAIController>()._scriptable.uniqueId);
                    Instantiate(FishCaughtMessage).GetComponent<FishCaughtMessage>().Message.text = "<color=orange>" + _hookedTo._owner.GetComponent<Inventory>().PlayerName + "</color>" + " caught a " + "<color=green>" + this.GetComponent<FishAIController>()._scriptable.FishWeight + "</color>" + " " + "<color=green>" + this.GetComponent<FishAIController>()._scriptable.FishName + "</color>";
                    _hookedTo._owner.GetComponent<PlayerFishing>().DestroyFloatSimulation();
                    NetworkServer.Destroy(gameObject);
				}
			}
		}
	}

    [ClientRpc]
    public void RpcHoldCaughtFish(int uniqueId)
    {
        _hookedTo._owner.GetComponent<Inventory>().HoldCaughtFish(uniqueId);
        _hookedTo._owner.GetComponent<Animator>().Play(_hookedTo._owner.GetComponent<Inventory>().FishHolderAnimationName);

        GameObject SpawnedInventoryFish;

        SpawnedInventoryFish = Instantiate(_hookedTo._owner.GetComponent<Inventory>().InventoryFishPrefab);
        SpawnedInventoryFish.transform.SetParent(_hookedTo._owner.GetComponent<Inventory>().Content);
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishName.text = _scriptable.FishName;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishLength.text = _scriptable.FishLength;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishWeight.text = "Weight: " + _scriptable.FishWeight;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishRetailValue.text = _scriptable.FishRetailValue;
        SpawnedInventoryFish.GetComponent<InventoryFish>().FishImage.sprite = _scriptable.FishSprite;

        SpawnedInventoryFish = null;

        _hookedTo._owner.GetComponent<Inventory>().CheckForItems();
    }

    private void OnGUI() {
		if (isServer) {
			if (_hookedTo != null) {
				GUI.Label(new Rect(1820, 0, 100, 100), "Server Fish Stamina: " + controller.stamina + "\n hooked!");
			}
		}
	}
}
