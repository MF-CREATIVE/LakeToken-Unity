using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public abstract class FluidInteractorBase : MonoBehaviour {

	protected Rigidbody _rb;
	private Collider _collider;

	private float _airDrag;
	private float _airAngularDrag;

	protected float _volume;
	[SerializeField, Range(.0f, 8f)] private float _customVolume;

	protected Fluid _fluid;

	protected virtual void Start() {
		_rb = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();

		_airDrag = _rb.drag;
		_airAngularDrag = _rb.angularDrag;

		_volume = _customVolume > .0f ? _customVolume : _collider.bounds.size.x * _collider.bounds.size.y * _collider.bounds.size.z;
	}

	protected abstract void FluidUpdate();

	protected virtual void FixedUpdate() {
		if (_fluid != null) {
			FluidUpdate();
		}
	}

	public virtual void EnterFluid(Fluid fluid) {
		_fluid = fluid;

		_rb.drag = fluid.drag;
		_rb.angularDrag = fluid.angularDrag;
	}

	public virtual void ExitFluid() {
		_fluid = null;

		_rb.drag = _airDrag;
		_rb.angularDrag = _airAngularDrag;
	}
}
