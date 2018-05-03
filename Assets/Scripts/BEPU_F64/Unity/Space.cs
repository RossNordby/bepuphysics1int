using UnityEngine;

namespace BEPUUnity
{
    public class Space : MonoBehaviour
    {
        private BEPUphysics.Space m_space;

        private void Awake()
        {
            m_space = new BEPUphysics.Space();
        }

        private void Start()
        {
            var findShapes = GetComponentsInChildren<ShapeBase>();
            for(int i=0; i<findShapes.Length; i++)
            {
                findShapes[i].OnRegister(this);
            }
        }
    }
}
