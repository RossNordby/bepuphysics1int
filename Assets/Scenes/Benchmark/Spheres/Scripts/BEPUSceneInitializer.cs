using BEPUUnity;
using UnityEngine;

public class BEPUSceneInitializer : SceneInitializer {
	[SerializeField] private BEPUUnity.Space m_space = null;
	
	public BEPUSphere SpherePrefab;
	
	protected override void Setup() {
		var shapeList = m_space.gameObject.GetComponentsInChildren<ShapeBase>();
		for (int i = 0; i < shapeList.Length; i++) {
			m_space.Add(shapeList[i]);
		}
	}

	protected override void SpawnSphere(Vector3 startPos) {
		BEPUSphere sphere = Instantiate(SpherePrefab, startPos, Quaternion.identity);
		sphere.GetEntity().Position = new BEPUutilities.Vector3(
			startPos.x.ToFix32(),
			startPos.y.ToFix32(),
			startPos.z.ToFix32()
			);
		m_space.Add(sphere);
	}
}
