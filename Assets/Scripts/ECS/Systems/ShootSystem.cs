using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using Game.Components;

namespace Game.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ShootSystem))]
    public sealed class ShootSystem : UpdateSystem
    {
        [SerializeField]
        private float _startVelocity;
        private Filter _gunFilter;
        private Filter _bulletsFilter;

        public override void OnAwake()
        {
            _gunFilter = this.World.Filter.With<Gun>().Build();
            _bulletsFilter = this.World.Filter.With<Bullet>().Without<IsActive>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var item in _gunFilter)
            {
                ref var gunComponent = ref item.GetComponent<Gun>();
                gunComponent.Timer += deltaTime;

                if(gunComponent.Timer > gunComponent.Recharge)
                {
                    var bullet = _bulletsFilter.First();
                    var position = gunComponent.ShootPosition.position;
                    ref var bulletComponent = ref bullet.GetComponent<Bullet>();
                    var transform = bulletComponent.Transform;
                    transform.position = position;
                    transform.gameObject.SetActive(true);
                    bulletComponent.Rigidbody.AddRelativeForce(gunComponent.Direction * _startVelocity);
                    bullet.SetComponent(new IsActive());

                    gunComponent.Timer = 0;
                }
            }
        }
    }
}