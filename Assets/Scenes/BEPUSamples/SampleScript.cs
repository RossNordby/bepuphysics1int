using UnityEngine;
using BEPUUnity;

public class SampleScript : MonoBehaviour
{
    [SerializeField] private BEPUUnity.Space m_space = null;

	void Start ()
    {
        Application.targetFrameRate = 60;

        var shapeList = m_space.gameObject.GetComponentsInChildren<ShapeBase>();
        for(int i=0; i<shapeList.Length; i++)
        {
            m_space.Add(shapeList[i]);
        }
    }
}
