using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fishing/Fish")]
public class FishScriptable : ScriptableObject {

	public int uniqueId;

	[Space]
	public GameObject modelPrefab;

	[Header("Smooth")]
	public float smoothTime;

	[Header("Bounds")]
	public float boundsVectorWeight;
	public Vector3 boundsSize;

	[Header("Avoidance")]
	public float avoidanceVectorWeight;
	public LayerMask avoidanceMask;
	public float avoidanceDist;

	[Header("Vision")]
	public float visionDistance;
	public LayerMask targetMask;

    [Header("Fish Statistics")]
    public string FishName;
    public string FishLength;
    public string FishWeight;
    public string FishRetailValue;
    public Sprite FishSprite;
}
