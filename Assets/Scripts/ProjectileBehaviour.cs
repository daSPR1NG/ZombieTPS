using UnityEngine;

namespace Khynan_Coding
{
    public class ProjectileBehaviour : MonoBehaviour
    {
        public float Speed = 5;
        public float Damage = 0;
        public GameObject HitFX;
        public Transform Target;

        private void Start()
        {
            if ( !Target ) 
            { 
                Destroy( gameObject );
                return;
            }

            transform.LookAt( new Vector3( Target.position.x, transform.position.y, Target.position.z ) );
        }

        private void Update()
        {
            if ( !Target )
            {
                Destroy( gameObject );
                return;
            }

            transform.position += transform.forward * Time.deltaTime * Speed;
        }

        private void OnTriggerEnter( Collider other )
        {
            Debug.Log( "HIT " + other.name, other );

            GlobalCharacterParameters globalCharacterParameters = other.GetComponent<GlobalCharacterParameters>();

            ApplyDamageOnTarget( other.transform );

            if ( globalCharacterParameters && globalCharacterParameters.CharacterType == CharacterType.IA_Enemy
                || other.gameObject.layer == LayerMask.NameToLayer( "Bullet" ) ) return;

            Instantiate( HitFX, transform.position, transform.rotation );
            Destroy( gameObject );
        }

        void ApplyDamageOnTarget( Transform target )
        {
            if ( !target ) return;

            StatsManager statsManager = target.GetComponent<StatsManager>();
            ThirdPersonController controller = target.GetComponent<ThirdPersonController>();
            Animator animator = null;

            if ( !controller ) return;

            animator = controller.Animator;

            if ( statsManager ) statsManager.ApplyDamageToTarget( transform, target.transform, Damage );

            if ( !statsManager.IsCharacterDead() && animator ) { AnimatorHelper.HandleThisAnimation( animator, "GotHit", true, 1, 1 ); }
        }
    }
}