using AvatarSpeaker.UIs;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AvatarSpeaker.DI
{
    public sealed class SceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private UiController _uiController;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_uiController);
        }
    }
}
