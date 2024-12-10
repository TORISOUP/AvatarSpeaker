using System;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Configurations;
using AvatarSpeaker.Infrastructures.RoomSpaces;
using AvatarSpeaker.Infrastructures.SpeakerSources;
using AvatarSpeaker.Infrastructures.Voicevoxs;
using AvatarSpeaker.Infrastructures.VrmSpeakers;
using AvatarSpeaker.UseCases;
using VContainer;
using VContainer.Unity;

namespace AvatarSpeaker.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
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
            builder.Register<VoicevoxVrmSpeakerProvider>(Lifetime.Scoped).As<ISpeakerProvider, IDisposable>();

            
            builder.Register<CurrentConfigurationRepository>(Lifetime.Scoped)
                .WithParameter(new VoiceControlConnectionSettings("http://localhost:50021"));
            builder.Register<VoicevoxSynthesizerProvider>(Lifetime.Scoped).AsSelf().As<IDisposable>();
            
        }
    }
}