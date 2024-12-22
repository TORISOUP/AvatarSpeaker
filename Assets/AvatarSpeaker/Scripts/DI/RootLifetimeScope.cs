using System;
using AvatarSpeaker.Core.Configurations;
using AvatarSpeaker.Core.Interfaces;
using AvatarSpeaker.Http;
using AvatarSpeaker.Http.Server;
using AvatarSpeaker.Infrastructures.Configurations;
using AvatarSpeaker.Infrastructures.RoomSpaces;
using AvatarSpeaker.Infrastructures.SpeakerSources;
using AvatarSpeaker.Infrastructures.Voicevoxes;
using AvatarSpeaker.Infrastructures.VoicevoxSpeakers;
using AvatarSpeaker.StartUp;
using AvatarSpeaker.UseCases;
using AvatarSpeaker.Views;
using AvatarSpeaker.Views.RoomSpaces;
using AvatarSpeaker.Views.ViewBinder;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AvatarSpeaker.DI
{
    /// <summary>
    /// アプリケーション上で共通のLifetimeScope
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private SpeakerCameraView _speakerCameraView;
        [SerializeField] private UguiRoomSpaceBackgroundView _uguiRoomSpaceBackgroundView;
        [SerializeField] private RuntimeAnimatorController _speakerAnimatorController;
        [SerializeField] private SubtitleView _subtitleViewPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            // StartUp
            builder.RegisterEntryPoint<ApplicationStartUp>();
            
            // UseCases
            builder.Register<SpeakerUseCase>(Lifetime.Singleton).As<IDisposable>().AsSelf();
            builder.Register<RoomSpaceUseCase>(Lifetime.Singleton);
            builder.Register<SpeakerCameraUseCase>(Lifetime.Singleton);
            builder.Register<ConfigurationUseCase>(Lifetime.Singleton);
            builder.Register<VoiceControlUseCase>(Lifetime.Singleton);

            // View
            builder.RegisterInstance(_speakerCameraView);
            builder.RegisterInstance(_subtitleViewPrefab);
            builder.RegisterEntryPoint<RoomSpaceViewBinder>()
                .WithParameter(_speakerAnimatorController);
            builder.RegisterInstance(_uguiRoomSpaceBackgroundView);

            // Infrastructures
            builder.Register<VoicevoxSpeakStyleProvider>(Lifetime.Singleton).As<ISpeakStyleProvider>();
            builder.Register<VoicevoxVoiceController>(Lifetime.Singleton).As<IVoiceController>();
            builder.Register<VoicevoxSpeakerProvider>(Lifetime.Singleton).As<ISpeakerProvider, IDisposable>();
            builder.Register<InMemoryConfigurationRepository>(Lifetime.Singleton)
                .WithParameter(new VoiceControlConnectionSettings("http://localhost:50021"))
                .WithParameter(new HttpServerSettings(21012, true))
                .AsImplementedInterfaces();
            builder.Register<VoicevoxProvider>(Lifetime.Singleton).AsSelf().As<IDisposable>();
            
            // SpeakerSource
            builder.Register<LocalSpeakerSourceProvider>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            // RoomSpace
            builder.Register<RoomSpaceProviderImpl>(Lifetime.Singleton)
                .AsImplementedInterfaces();

            // Http
            builder.Register<HttpServerRunner>(Lifetime.Singleton);
            builder.Register<SpeakerController>(Lifetime.Singleton).As<BaseController>();
            builder.Register<MiscController>(Lifetime.Singleton).As<BaseController>();
        }
    }
}