using UnityEngine;

namespace Khynan_Coding
{
    [CreateAssetMenu(fileName = "BulletTrailConfig_", menuName = "Configs/Bullet Trail Config")]
    public class TrailConfig : ScriptableObject
    {
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float time;
        [SerializeField] private float minVertexDistance;
        [SerializeField] private Gradient gradient;
        [SerializeField] private Material material;
        [Range(0, 90)][SerializeField] private int cornerCapVertices;
        [Range(0, 90)][SerializeField] private int endCapVertices;

        public void Apply(TrailRenderer trailRenderer)
        {
            trailRenderer.widthCurve = animationCurve;
            trailRenderer.time = time;
            trailRenderer.minVertexDistance = minVertexDistance;

            trailRenderer.colorGradient = gradient;
            trailRenderer.material = material;

            trailRenderer.numCornerVertices = cornerCapVertices;
            trailRenderer.numCapVertices = endCapVertices;
        }
    }
}