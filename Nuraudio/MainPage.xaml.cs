using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using NazarAudio.Classes;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using System.Windows.Resources;

namespace Nuraudio
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<Chapter> chapterList = new List<Chapter>();
        private DispatcherTimer timer = new DispatcherTimer();

        public MainPage()
        {
            InitializeComponent();
            ConstructUI();
        }

        private void ConstructUI()
        {
            addContentToListBox();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(2000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            // Set PlayStateChanged handler
            Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
            StaticVariables.currentChapterID = 0;

            // Set CompositionTarget.Rendering handler
            //System.Windows.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;
        }

        private void addContentToListBox()
        {
            listBoxChapterName.Items.Clear();
            chapterList.Clear();
            chapterList.Add(new Chapter(1, "Жалғыз Отан – тәуелсіз Қазақстан", "Единая Отчизна - независимый Казахстан", 710, 833));
            chapterList.Add(new Chapter(2, "Жер тағдыры — ел тағдыры", "Судьба земли - судьба страны", 376, 519));
            chapterList.Add(new Chapter(3, "Садағаң кетейін, айналайын халқым!", "На алтарь твой я жертвой взойду...", 512, 654));
            chapterList.Add(new Chapter(4, "Тәуелсіздік – тәуекелі тұратындардың еншісі", "Независимость - удел сильных духом", 495, 567));
            chapterList.Add(new Chapter(5, "Ана тілі – ұлтымыздың айнасы", "Родной язык - зеркало нации", 726, 1045));
            chapterList.Add(new Chapter(6, "Тарихтан тағылым ала алсақ", "Уроки истории", 636, 699));
            chapterList.Add(new Chapter(7, "Ел бірлігі – асыл қасиет", "Единство народа - высшая добродетель", 416, 518));
            chapterList.Add(new Chapter(8, "Ұлттық намыс – ұлы ұғым", "Национальное достоинство - великая ценность", 676, 1013));
            chapterList.Add(new Chapter(9, "Демократия жарлықпен орнатылмайды", "Демократия не устанавливается декретом", 612, 831));
            chapterList.Add(new Chapter(10, "Сенгенің игілікті еңбегің болсын", "Надеяться только на доблестный труд", 268, 421));
            chapterList.Add(new Chapter(11, "Дәстүріңді баққаның – үмітіңді жаққаның", "Следуя традициям, устремляться в будущее", 397, 576));
            chapterList.Add(new Chapter(12, "Өрелі рухсыз өркениет жоқ", "Без гуманизма нет цивилизации", 511, 729));

            foreach (Chapter chapter in chapterList)
            {
                Grid grid = new Grid();
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                grid.Width = 400;
                grid.Height = 50;

                TextBlock tb = new TextBlock();
                tb.Text = chapter.chapterID.ToString() + " " + chapter.titleList[StaticVariables.langID];
                tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                tb.TextAlignment = TextAlignment.Left;
                tb.FontSize = 20;
                tb.MaxWidth = 400;
                tb.MaxHeight = 50;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontWeight = FontWeights.Bold;

                grid.Children.Add(tb);
                grid.Tag = chapter.chapterID.ToString();
                grid.Tap += ContentGrid_Tap;

                listBoxChapterName.Items.Add(grid);
            }
        }
        private void ContentGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Grid grid = (Grid)sender;
                if (grid != null)
                {
                    StaticVariables.currentChapterID = Convert.ToInt32(grid.Tag) - 1;
                    int difference = Math.Abs(MyAudioPlaybackAgent.AudioPlayer.currentTrackNumber - chapterList[StaticVariables.currentChapterID].getTrackNumber());
                    for (int i = 0; i < difference; i++)
                    {
                        if (MyAudioPlaybackAgent.AudioPlayer.currentTrackNumber > chapterList[StaticVariables.currentChapterID].getTrackNumber())
                            Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.SkipPrevious();
                        else
                            Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.SkipNext();
                    }
                    MyAudioPlaybackAgent.AudioPlayer.currentTrackNumber = chapterList[StaticVariables.currentChapterID].getTrackNumber();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during MainPage.ContentGrid_Tap: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.PlayerState == Microsoft.Phone.BackgroundAudio.PlayState.Playing)
            {
                audioProgressBar.Value = Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.Position.TotalSeconds;
                audioProgressBar.Maximum = Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds;
            }
        }

        private void textBlockLanguage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb != null)
            {
                if (tb.Text == StaticVariables.languageArray[0])
                {
                    StaticVariables.langID = 1;
                }
                else if (tb.Text == StaticVariables.languageArray[1])
                {
                    StaticVariables.langID = 0;
                }

                tb.Text = StaticVariables.languageArray[StaticVariables.langID];

                if (StaticVariables.langID == 0)
                {
                    Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.FastForward();
                    MyAudioPlaybackAgent.AudioPlayer.currentTrackNumber = chapterList[StaticVariables.currentChapterID].getTrackNumber();
                }
                else
                {
                    Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.FastForward();
                    MyAudioPlaybackAgent.AudioPlayer.currentTrackNumber = chapterList[StaticVariables.currentChapterID].getTrackNumber();
                }

                UpdateUI();
            }
        }
        private void UpdateUI()
        {
            addContentToListBox();
        }

        private void ImagePrevious_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            if (img != null)
                setImageSource(img, "music_previous_pressed.png");
        }
        private void ImagePrevious_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            if (img != null)
                setImageSource(img, "music_previous_not_pressed.png");
            if (StaticVariables.currentChapterID > 0)
            {
                StaticVariables.currentChapterID--;
                Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.SkipPrevious();
            }
            else
            {
                StaticVariables.currentChapterID = chapterList.Count - 1;
                Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.SkipPrevious();
            }
            MyAudioPlaybackAgent.AudioPlayer.currentTrackNumber = chapterList[StaticVariables.currentChapterID].getTrackNumber();
        }

        private void ImagePlay_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            if (img != null)
            {
                if (StaticVariables.currentPlayPauseImageUri.Contains("play"))
                {
                    StaticVariables.currentPlayPauseImageUri = "music_play_pressed.png";
                }
                else if (StaticVariables.currentPlayPauseImageUri.Contains("pause"))
                {
                    StaticVariables.currentPlayPauseImageUri = "music_pause_pressed.png";
                }
                setImageSource(img, StaticVariables.currentPlayPauseImageUri);
            }
        }
        private void ImagePlay_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            try
            {
                System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
                if (img != null)
                {
                    if (StaticVariables.currentPlayPauseImageUri.Contains("play"))
                    {
                        StaticVariables.currentPlayPauseImageUri = "music_pause_not_pressed.png";
                        Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.Play();
                    }
                    else if (StaticVariables.currentPlayPauseImageUri.Contains("pause"))
                    {
                        StaticVariables.currentPlayPauseImageUri = "music_play_not_pressed.png";
                        if (Microsoft.Phone.BackgroundAudio.PlayState.Playing == Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.PlayerState)
                        {
                            Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.Pause();
                        }
                    }
                    setImageSource(img, StaticVariables.currentPlayPauseImageUri);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during MainPage.ImagePlay_ManipulationCompleted: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void ImageNext_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            if (img != null)
                setImageSource(img, "music_next_pressed.png");
        }
        private void ImageNext_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            if (img != null)
                setImageSource(img, "music_next_not_pressed.png");
            if (StaticVariables.currentChapterID < chapterList.Count - 1)
            {
                StaticVariables.currentChapterID++;
                Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.SkipNext();
            }
            else
            {
                StaticVariables.currentChapterID = 0;
                Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.SkipNext();
            }
            MyAudioPlaybackAgent.AudioPlayer.currentTrackNumber = chapterList[StaticVariables.currentChapterID].getTrackNumber();
        }

        private void setImageSource(System.Windows.Controls.Image sender, string imgName)
        {
            System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation;
            bi.UriSource = new Uri("/Images/" + imgName, UriKind.Relative);
            sender.Source = bi;
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            try
            {
                switch (Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.PlayerState)
                {
                    case Microsoft.Phone.BackgroundAudio.PlayState.Playing:
                        StaticVariables.currentPlayPauseImageUri = "music_pause_not_pressed.png";
                        audioProgressBar.Maximum = Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds;
                        break;

                    case Microsoft.Phone.BackgroundAudio.PlayState.Paused:
                        StaticVariables.currentPlayPauseImageUri = "music_play_not_pressed.png";
                        break;
                }
                setImageSource(playImage, StaticVariables.currentPlayPauseImageUri);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during MainPage.Instance_PlayStateChanged: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void audioProgressBar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var pos = e.GetPosition(audioProgressBar).X;
            var width = audioProgressBar.ActualWidth;
            audioProgressBar.Value = (pos / width) * audioProgressBar.Maximum;

            try
            {
                if (Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.PlayerState == Microsoft.Phone.BackgroundAudio.PlayState.Playing)
                {
                    TimeSpan ts = new TimeSpan(0, 0, (int)audioProgressBar.Value);
                    Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.Position = ts;
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("InvalidOperationException during MainPage.UIElement_OnTap: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }
    }
}