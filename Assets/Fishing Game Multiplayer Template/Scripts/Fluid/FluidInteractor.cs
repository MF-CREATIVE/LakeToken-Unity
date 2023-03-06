using UnityEngine;

public class FluidInteractor : FluidInteractorBase {

	[SerializeField] private float _floatStrength = 2f;
	[SerializeField] private bool _simulateTurbulence;
	[SerializeField, Range(.0f, 8f)] private float _turbulenceStrength = 1f;

	private float _time;
	private float[] _randomTimeOffset;

	[SerializeField] private float _dampeningFactor = .1f;

	private void Awake() {
		_randomTimeOffset = new float[4];
		for (int i = 0; i < 4; i++) {
			_randomTimeOffset[i] = Random.Range(.0f, 4f);
		}
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();
		_time += Time.fixedDeltaTime / 4f;
	}

	private Vector3 CalculateTurbulence() {
		Vector3 turbulence = new Vector3(Mathf.PerlinNoise(_time + _randomTimeOffset[0], _time + _randomTimeOffset[1]) * 2f - 1f,
			.0f,
			Mathf.PerlinNoise(_time + _randomTimeOffset[2], _time + _randomTimeOffset[3]) * 2f - 1f);

#if UNITY_EDITOR
		Debug.DrawRay(transform.position, turbulence);
#endif

		return turbulence * _turbulenceStrength;
	}

	protected override void FluidUpdate() {
		float difference = transform.position.y - _fluid.transform.position.y;

		if (difference < .0f) {
			Vector3 buoyancy = _floatStrength * _fluid.density * _volume * Mathf.Abs(difference) * Physics.gravity.magnitude * Vector3.up;

			if (_simulateTurbulence) {
				buoyancy += CalculateTurbulence();
				_rb.AddTorque(CalculateTurbulence() * .5f);
			}

			_rb.AddForceAtPosition(buoyancy, transform.position, ForceMode.Force);
			_rb.AddForceAtPosition(_dampeningFactor * _volume * -_rb.velocity, transform.position, ForceMode.Force);
		}
	}
}
