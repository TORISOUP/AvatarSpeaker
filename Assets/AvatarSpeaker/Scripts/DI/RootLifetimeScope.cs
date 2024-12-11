using System;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Configurations;
using AvatarSpeaker.Infrastructures.RoomSpaces;
using AvatarSpeaker.Infrastructures.SpeakerSources;
using AvatarSpeaker.Infrastructures.Voicevoxes;
using AvatarSpeaker.Infrastructures.VoicevoxSpeakers;
using AvatarSpeaker.Scripts.Externals;
using AvatarSpeaker.UseCases;
using VContainer;
using VContainer.Unity;

namespace AvatarSpeaker.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // External
            builder.Register<GameObjectRepository>(Lifetime.Scoped).AsSelf().As<IDisposable>();
            
            // SpeakerSource
            builder.Register<LocalSpeakerSourceProvider>(Lifetime.Scoped)
                .As<ISpeakerSourceProvider, IDisposable>();
            
            // RoomSpace
            builder.Register<SingletonRoomSpaceProvider>(Lifetime.Scoped)
                .As<IRoomSpaceProvider, IDisposable>();
            
            // UseCases
            builder.Register<SpeakerUseCase>(Lifetime.Scoped);
            builder.Register<RoomSpaceUseCase>(Lifetime.Scoped);
            
            //
            builder.Register<VoicevoxSpeakerProvider>(Lifetime.Scoped).As<ISpeakerProvider, IDisposable>();

            
            builder.Register<CurrentConfigurationRepository>(Lifetime.Scoped)
                .WithParameter(new VoiceControlConnectionSettings("http://localhost:50021"));
            builder.Register<VoicevoxSynthesizerProvider>(Lifetime.Scoped).AsSelf().As<IDisposable>();
            
        }
    }
}