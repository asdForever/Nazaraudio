using System;
using System.Windows;
using Microsoft.Phone.BackgroundAudio;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows.Resources;

namespace MyAudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        public static int currentTrackNumber = 0; // Current track number
        private static volatile bool _classInitialized;

        private static List<Track> _trackList = new List<Track>
        {
            new Track("01_kz.mp3", "Тарау 1. Жалғыз Отан - тәуелсiз Қазақстан"),
            new Track("02_kz.mp3", "Тарау 2. Жер тағдыры - ел тағдыры"),
            new Track("03_kz.mp3", "Тарау 3. Садағаң кетейiн, айналайын халқым!"),
            new Track("04_kz.mp3", "Тарау 4. Тәуелсiздiк - тәуекелi тұратындардың еншiсi"),
            new Track("05_kz.mp3", "Тарау 5. Ана тiлi - ұлтымыздың айнасы"),
            new Track("06_kz.mp3", "Тарау 6. Тарихтан тағылым ала алсақ"),
            new Track("07_kz.mp3", "Тарау 7. Eл бiрлiгi - асыл қасиет"),
            new Track("08_kz.mp3", "Тарау 8. Ұлттық намыс - ұлы ұғым"),
            new Track("09_kz.mp3", "Тарау 9. Демократия жарлықпен орнатылмайды"),
            new Track("10_kz.mp3", "Тарау 10. Сенгенiң игiлiктi еңбегiң болсын"),
            new Track("11_kz.mp3", "Тарау 11. Дәстүрiңдi баққаның - үмiтiңдi жаққаның"),
            new Track("12_kz.mp3", "Тарау 12. Өрелi рухсыз өркениет жоқ"),

            new Track("01_ru.mp3", "Глава 1. Eдиная Отчизна - независимый Казахстан"),
            new Track("01_ru.mp3", "Глава 2. Судьба земли - судьба страны"),
            new Track("01_ru.mp3", "Глава 3. На алтарь твой я жертвой взойду"),
            new Track("01_ru.mp3", "Глава 4. Независимость - удел сильных духом"),
            new Track("01_ru.mp3", "Глава 5. Родной язык - зеркало нации"),
            new Track("01_ru.mp3", "Глава 6. Уроки истории"),
            new Track("01_ru.mp3", "Глава 7. Eдинство народа-высшая добродетель"),
            new Track("01_ru.mp3", "Глава 8. Национальное достоинство - великая ценность"),
            new Track("01_ru.mp3", "Глава 9. Демократия не устанавливается декретом"),
            new Track("01_ru.mp3", "Глава 10. Надеяться только на доблестный труд"),
            new Track("01_ru.mp3", "Глава 11. Следуя традициям,устремляться в будущее"),
            new Track("01_ru.mp3", "Глава 12. Без гуманизма нет цивилизации"),
        };

        // A playlist made up of AudioTrack items.
        private static List<AudioTrack> _playList = new List<AudioTrack>();

        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += AudioPlayer_UnhandledException;
                });
                try
                {
                    foreach (Track item in _trackList)
                    {
                        _playList.Add(new AudioTrack(item.uri, item.trackName, item.artist, item.album, null));
                    }
                }
                catch (Exception e) { }
            }
        }

        private void PlayNextTrack(BackgroundAudioPlayer player)
        {
            if (++currentTrackNumber >= _playList.Count)
            {
                currentTrackNumber = 0;
            }

            PlayTrack(player);
        }

        private void PlayPreviousTrack(BackgroundAudioPlayer player)
        {
            if (--currentTrackNumber < 0)
            {
                currentTrackNumber = _playList.Count - 1;
            }

            PlayTrack(player);
        }
        private void PlayTrack(BackgroundAudioPlayer player)
        {
            try
            {
                if ((player.Track == null) || (player.Track.Title != _playList[currentTrackNumber].Title))
                {
                    // If it's a new track, set the track
                    using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!storage.FileExists(_trackList[currentTrackNumber].fileName))
                        {
                            string _filePath = "Audio/" + _trackList[currentTrackNumber].fileName;
                            StreamResourceInfo resource = Application.GetResourceStream(new Uri(_filePath, UriKind.Relative));

                            using (IsolatedStorageFileStream file = storage.CreateFile(_trackList[currentTrackNumber].fileName))
                            {
                                int chunkSize = 4096;
                                byte[] bytes = new byte[chunkSize];
                                int byteCount;

                                while ((byteCount = resource.Stream.Read(bytes, 0, chunkSize)) > 0)
                                {
                                    file.Write(bytes, 0, byteCount);
                                }
                            }
                        }
                    }
                    player.Track = _playList[currentTrackNumber];
                }

                // Play it
                if ((player.Track != null) && (player.PlayerState != PlayState.Playing))
                {
                    player.Play();
                }
            }
            catch (Exception e) { }
        }

        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    // An unhandled exception has occurred; break into the debugger
                    System.Diagnostics.Debugger.Break();
                }
                // Force the track to stop 
                // http://blogs.msdn.com/b/wpukcoe/archive/2012/02/11/background-audio-in-windows-phone-7-5-part-3.aspx
                Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.Track = null;
            }
            catch (System.Exception ex) { }
        }

        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent 
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            if (isFatal)
            {
                Abort();
            }
            else
            {
                try
                {
                    // Force the track to stop 
                    // http://blogs.msdn.com/b/wpukcoe/archive/2012/02/11/background-audio-in-windows-phone-7-5-part-3.aspx
                    player.Track = null;
                }
                catch (System.Exception e) { }
                NotifyComplete();
            }
        }


        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        /// 
        /// Notable playstate events: 
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            switch (playState)
            {
                case PlayState.TrackEnded:
                    PlayNextTrack(player);
                    break;
                case PlayState.TrackReady:
                    player.Play();
                    break;
                case PlayState.Shutdown:
                    // TODO: Handle the shutdown state here (e.g. save state)
                    break;
                case PlayState.Unknown:
                    break;
                case PlayState.Stopped:
                    break;
                case PlayState.Paused:
                    break;
                case PlayState.Playing:
                    break;
                case PlayState.BufferingStarted:
                    break;
                case PlayState.BufferingStopped:
                    break;
                case PlayState.Rewinding:
                    break;
                case PlayState.FastForwarding:
                    break;
            }

            NotifyComplete();
        }


        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            switch (action)
            {
                case UserAction.Play:
                    if (player.PlayerState != PlayState.Playing)
                    {
                        PlayTrack(player);
                    }
                    break;
                case UserAction.Stop:
                    player.Stop();
                    break;
                case UserAction.Pause:
                    player.Pause();
                    break;
                case UserAction.FastForward:
                    player.FastForward();
                    break;
                case UserAction.Rewind:
                    player.Rewind();
                    break;
                case UserAction.Seek:
                    player.Position = (TimeSpan)param;
                    break;
                case UserAction.SkipNext:
                    PlayNextTrack(player);
                    break;
                case UserAction.SkipPrevious:
                    PlayPreviousTrack(player);
                    break;
            }

            NotifyComplete();
        }


        /// <summary>
        /// Implements the logic to get the next AudioTrack instance.
        /// In a playlist, the source can be from a file, a web request, etc.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if the playback is completed</returns>
        private AudioTrack GetNextTrack()
        {
            // TODO: add logic to get the next audio track

            AudioTrack track = null;

            // specify the track

            return track;
        }


        /// <summary>
        /// Implements the logic to get the previous AudioTrack instance.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if previous track is not allowed</returns>
        private AudioTrack GetPreviousTrack()
        {
            // TODO: add logic to get the previous audio track

            AudioTrack track = null;

            // specify the track

            return track;
        }

        /// <summary>
        /// Called when the agent request is getting cancelled
        /// </summary>
        /// <remarks>
        /// Once the request is Cancelled, the agent gets 5 seconds to finish its work,
        /// by calling NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {

        }
    }
}
