using UnityEngine;

public abstract class SceneInitializer : MonoBehaviour {
	public int SphereCount = 50;
	public float SpawnDistance = 10;

	protected abstract void Setup();

	void Start() {
		Setup();

		// Spawn spheres random
		for (int i = 0; i < SphereCount; i++) {
			SpawnSphereWrapper();
		}
	}

	private void SpawnSphereWrapper() {
		Vector3 startPos = new Vector3(
				transform.position.x + Random.Range(-SpawnDistance, SpawnDistance),
				transform.position.y + Random.Range(-SpawnDistance, SpawnDistance),
				transform.position.z + Random.Range(-SpawnDistance, SpawnDistance)
				);
		SpawnSphere(startPos);
		spawnedSpheres++;
	}

	private int spawnedSpheres = 0;
	protected abstract void SpawnSphere(Vector3 startPos);

	private void Update() {
		if (Input.GetKey(KeyCode.Space)) {
			SpawnSphereWrapper();
		}
	}

	private void OnGUI() {
		GUI.color = Color.black;
		GUILayout.Label("Sphere count: " + spawnedSpheres);
	}
}
