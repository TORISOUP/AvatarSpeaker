using System;
using AvatarSpeaker.Core;
using AvatarSpeaker.Core.Configurations;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.Infrastructures.Https;
using AvatarSpeaker.Infrastructures.RoomSpaces;
using AvatarSpeaker.Infrastructures.SpeakerSources;
using AvatarSpeaker.Infrastructures.Voicevoxes;
using AvatarSpeaker.Infrastructures.VoicevoxSpeakers;
using AvatarSpeaker.Scripts.Views;
using AvatarSpeaker.Scripts.Views.ViewBinder;
using AvatarSpeaker.StartUp;
using AvatarSpeaker.StartUp.Services;
using AvatarSpeaker.UseCases;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AvatarSpeaker.DI
{
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private SpeakerCameraView _speakerCameraView;
        [SerializeField] private UguiRoomSpaceBackgroundView _uguiRoomSpaceBackgroundView;

        protected override void Configure(IContainerBuilder builder)
        {
            // StartUp
            builder.RegisterEntryPoint<ApplicationStartUp>();
            builder.RegisterEntryPoint<ExternalSpeakRequestService>();
            
            
            // SpeakerSource
            builder.Register<LocalSpeakerSourceProvider>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // RoomSpace
            builder.Register<RoomSpaceProviderImpl>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // UseCases
            builder.Register<SpeakerUseCase>(Lifetime.Singleton).As<IDisposable>().AsSelf();
            builder.Register<RoomSpaceUseCase>(Lifetime.Singleton);
            builder.Register<SpeakerCameraUseCase>(Lifetime.Singleton);

            // View
            builder.RegisterInstance(_speakerCameraView);
            builder.RegisterEntryPoint<RoomSpaceViewBinder>();
            builder.RegisterInstance(_uguiRoomSpaceBackgroundView);

            //
            builder.Register<VoicevoxSpeakerProvider>(Lifetime.Singleton).As<ISpeakerProvider, IDisposable>();


            // Infrastructures
            builder.Register<VoicevoxSpeakStyleProvider>(Lifetime.Singleton).As<ISpeakStyleProvider>();
            builder.Register<MiniHttpServer<SpeakRequestDto>>(Lifetime.Singleton)
                .As<IDisposable>()
                .AsSelf()
                .WithParameter(21012);
            
            builder.Register<HttpSpeakRequestProvider>(Lifetime.Singleton).As<ISpeakRequestProvider, IDisposable>();


            builder.Register<CurrentConfigurationRepository>(Lifetime.Singleton)
                .WithParameter(new VoiceControlConnectionSettings("http://localhost:50021"));
            builder.Register<VoicevoxSynthesizerProvider>(Lifetime.Singleton).AsSelf().As<IDisposable>();
        }
    }
}