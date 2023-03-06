using UnityEngine;
using Mirror;

public class FishingFloat : NetworkBehaviour {

	[SyncVar] public int floatUniqueId;

	public FluidInteractorBase _interactor;

	private static FishingFloatScriptable[] _floatScriptables;
	private FishingFloatScriptable _scriptable;

	public Rigidbody _rb;
	public Collider _collider;

	[SyncVar] public PlayerFishing _owner;

	public FishEntity fish;

    public Transform Hook;

	private void Awake() {
		_interactor = GetComponent<FluidInteractorBase>();
		_rb = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();

		if (_floatScriptables == null) {
			_floatScriptables = Resources.LoadAll<FishingFloatScriptable>("FishingFloats");
		}
    }

	public void Destroy(NetworkConnection sender) {
		if (sender != null && sender.identity != null && sender.identity.GetComponent<PlayerFishing>() == _owner) {
			NetworkServer.Destroy(gameObject);
		}
	}

	[Command(requiresAuthority = true)]
	public void Pull() {
		if (fish != null) {
			fish.controller.target = _owner._rodEndPoint;
			fish.controller.pullForce = 1f;
		}
		else {
			TargetPull();
		}
	}

	[TargetRpc]
	public void TargetPull() {
		_rb.AddForce((_owner._rodEndPoint.position - transform.position) * .2f);
	}

	private void Start() {
		if (!hasAuthority) {
			_interactor.enabled = false;
			_rb.isKinematic = true;
			_rb.useGravity = false;
		}

		for (int i = 0; i < _floatScriptables.Length; i++) {
			if (_floatScriptables[i].uniqueId == floatUniqueId) {
				_scriptable = _floatScriptables[i];
				break;
			}
		}

		// Assign your customization variables here ~
		_ = Instantiate(_scriptable.modelPrefab, transform); // Model
	}

	public override void OnStartAuthority() {
		base.OnStartAuthority();
		_interactor.enabled = true;
		_rb.isKinematic = false;
		_rb.useGravity = true;
	}

	public override void OnStopAuthority() {
		base.OnStopAuthority();
		_interactor.enabled = false;
		_rb.isKinematic = true;
		_rb.useGravity = false;
	}
}
