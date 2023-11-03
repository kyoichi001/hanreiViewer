using UnityEngine;

namespace Akak
{
    public class LOD : MonoBehaviour
    {
        Camera camera;
        void Awake()
        {
            camera = FindObjectOfType<Camera>();
        }
        public int GetLOD()
        {
            var scale = Mathf.Abs(camera.transform.position.z - transform.position.z);
            if (scale >= 1500)//距離が離れているほど大きい
            {
                return 3;
            }
            else if (scale >= 800)
            {
                return 2;
            }
            else if (scale >= 700)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}