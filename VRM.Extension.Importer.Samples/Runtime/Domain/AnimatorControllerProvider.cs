using UnityEngine;

namespace VRM.Extension.Samples
{
    public class AnimatorControllerProvider : MonoBehaviour
    {
        [SerializeField] RuntimeAnimatorController _AnimatorController;

        public RuntimeAnimatorController GetAnimatorController()
        {
            return _AnimatorController;
        }
    }
}