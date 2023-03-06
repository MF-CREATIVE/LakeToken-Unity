using UnityEngine;

public class Fluid : MonoBehaviour {

	public float density = 1f;
	public float drag = 1f;
	public float angularDrag = 1f;

	private void OnTriggerEnter(Collider other) {
		if (other.TryGetComponent(out FluidInteractorBase interactor)) {
			interactor.EnterFluid(this);
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.TryGetComponent(out FluidInteractorBase interactor)) {
			interactor.ExitFluid();
		}
	}
}
