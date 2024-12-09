using System;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Configurations;
using AvatarSpeaker.Infrastructures.SpeakerSources;
using AvatarSpeaker.Infrastructures.Voicevoxs;
using AvatarSpeaker.Infrastructures.VrmSpeakers;
using AvatarSpeaker.StartUp;
using VContainer;
using VContainer.Unity;

namespace AvatarSpeaker.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // EntryPoint
            builder.RegisterEntryPoint<EntryPoint>();
            
            // SpeakerSource
            builder.Register<LocalSpeakerSourceProvider>(Lifetime.Scoped)
                .As<ISpeakerSourceProvider, IDisposable>();
            //
            builder.Register<VoicevoxVrmSpeakerProvider>(Lifetime.Scoped).As<ISpeakerProvider, IDisposable>();
            builder.Register<CurrentConfigurationRepository>(Lifetime.Scoped)
                .WithParameter(new VoiceControlConnectionSettings("http://localhost:50021"));
            builder.Register<VoicevoxSynthesizerProvider>(Lifetime.Scoped).AsSelf().As<IDisposable>();
            
        }
    }
}