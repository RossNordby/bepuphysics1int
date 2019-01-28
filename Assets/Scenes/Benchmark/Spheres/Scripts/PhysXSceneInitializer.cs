using UnityEngine;

public class PhysXSceneInitializer : SceneInitializer {
	public Rigidbody SpherePrefab;

	protected override void Setup() { }
	
	protected override void SpawnSphere(Vector3 startPos) {
		Rigidbody sphere = Instantiate(SpherePrefab, startPos, Quaternion.identity);
	}
}
