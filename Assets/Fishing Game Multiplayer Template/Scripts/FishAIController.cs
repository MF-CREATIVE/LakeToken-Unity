using System.Collections;
using UnityEngine;

public class FishAIController : MonoBehaviour {

	public FishScriptable _scriptable;
	private Bounds _bounds;

	public Transform target;
	public float pullForce;

	public bool doNotUpdateTarget;
	public float stamina = 1f;

    private Vector3 CalculateBoundsVector() {
		return _bounds.Contains(transform.position) ? Vector3.zero : (_bounds.center - transform.position).normalized;
	}

	private static readonly Vector3[] _directions = { Vector3.left, Vector3.right, Vector3.up, Vector3.down };
	private Vector3 _currentAvoidanceVector;
	private Vector3 CalculateAvoidanceDirVector() {
		if (_currentAvoidanceVector != Vector3.zero) {
			if (!Physics.Raycast(transform.position, transform.forward, _scriptable.avoidanceDist, _scriptable.avoidanceMask)) {
				return _currentAvoidanceVector;
			}
		}
		float maxDistance = int.MinValue;
		Vector3 result = Vector3.zero;
		for (int i = 0; i < _directions.Length; i++) {
			Vector3 currentDirection = transform.TransformDirection(_directions[i].normalized);
			if (Physics.Raycast(transform.position, currentDirection, out RaycastHit hitInfo, _scriptable.avoidanceDist, _scriptable.avoidanceMask)) {
				float distance = (hitInfo.point - transform.position).sqrMagnitude;
				if (distance > maxDistance) {
					maxDistance = distance;
					result = currentDirection;
				}
			}
			else {
				result = currentDirection;
				_currentAvoidanceVector = currentDirection.normalized;
				return result.normalized;
			}
		}
		return result.normalized;
	}

	public float fearfulness;

	private Vector3 CalculateFearVector() {
		return target != null ? ((target.position + transform.position) * fearfulness).normalized : Vector3.zero;
	}

	private Vector3 CalculateTargetVector() {
		return (target != null && fearfulness < .1f) ? (target.position - transform.position).normalized : Vector3.zero;
	}

	private Vector3 CalculateAvoidanceVector() {
		Vector3 result = Vector3.zero;
		if (Physics.Raycast(transform.position, transform.forward, _scriptable.avoidanceDist, _scriptable.avoidanceMask)) {
			result = CalculateAvoidanceDirVector();
		}
		else {
			_currentAvoidanceVector = Vector3.zero;
		}
		return result;
	}

	private float CalculateSpeed() {
		return 2f;
	}

	private bool inWater = true;
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Water")) {
			inWater = true;
		}
	}
	private void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == LayerMask.NameToLayer("Water")) {
			inWater = false;
		}
	}

	private Vector3 _currentVelocity;
	private void Update() {
		if (pullForce > .0f) {
			pullForce -= Time.deltaTime * 1.6f;
			stamina -= Time.deltaTime * .04f;
        }
		else {
			stamina += Time.deltaTime * .08f;
		}
		pullForce = Mathf.Clamp(pullForce, .0f, 1f);
		stamina = Mathf.Clamp(stamina, .0f, 1f);
		fearfulness = Mathf.Clamp(fearfulness, .0f, 1f);
		Vector3 movementVector = (CalculateBoundsVector() * _scriptable.boundsVectorWeight) +
			(CalculateAvoidanceVector() * _scriptable.avoidanceVectorWeight) +
			(CalculateTargetVector() * 2f) +
			(CalculateFearVector() * 6f);
		if (!inWater) {
			movementVector = Vector3.down;
			movementVector = Vector3.SmoothDamp(transform.forward, movementVector, ref _currentVelocity, .1f);
			movementVector = movementVector.normalized * (CalculateSpeed() * (doNotUpdateTarget ? 1.5f : 1f) * 1.4f);
		}
		else {
			movementVector = Vector3.SmoothDamp(transform.forward, movementVector, ref _currentVelocity, _scriptable.smoothTime);
			movementVector = movementVector.normalized * (CalculateSpeed() * (doNotUpdateTarget ? 1.5f : 1f));
		}
		transform.forward = movementVector;
		transform.position += ((movementVector * stamina) + (pullForce > .0f ? (target.position - transform.position) * (.2f * pullForce / stamina / stamina) : Vector3.zero)) * Time.deltaTime;
	}

	private IEnumerator CustomUpdateLoop() { // (Server)
		while (true) {
			yield return new WaitForSeconds(.4f);

			UpdateTarget(); // Vision
		}
	}

	public void Setup(FishScriptable fishScriptable) {
		_scriptable = fishScriptable;
		_bounds = new Bounds(transform.position, _scriptable.boundsSize);
		StartCoroutine(CustomUpdateLoop());
	}

	private void UpdateTarget() {
		if (doNotUpdateTarget) {
			return;
		}

		target = null;
		Collider[] result = Physics.OverlapSphere(transform.position, _scriptable.visionDistance, _scriptable.targetMask);
		for (int i = 0; i < result.Length; i++) {
			Transform target = result[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (!Physics.Raycast(transform.position, dirToTarget, Vector3.Distance(transform.position, target.position), _scriptable.avoidanceMask)) {
				this.target = target;
				break;
			}
		}
	}

#if UNITY_EDITOR // (Editor)
	private void OnDrawGizmos() {
		if (target != null) {
			Gizmos.DrawLine(transform.position, target.transform.position);
		}
		Gizmos.DrawWireCube(_bounds.center, _bounds.size);
	}
#endif
}
