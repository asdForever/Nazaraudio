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

namespace Nuraudio
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<Chapter> chapterList = new List<Chapter>();
        public MainPage()
        {
            InitializeComponent();
            ConstructUI();
        }

        private void ConstructUI()
        {
            addContentToListBox();
            Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
        }

        private void addContentToListBox()
        {
            listBoxChapterName.Items.Clear();
            chapterList.Clear();
            chapterList.Add(new Chapter(1, "Жалғыз Отан – тәуелсіз Қазақстан", "/files/01_kz.mp3", "Единая Отчизна - независимый Казахстан", "/files/01_ru.mp3"));
            chapterList.Add(new Chapter(2, "Жер тағдыры — ел тағдыры", "/files/02_kz.mp3", "Судьба земли - судьба страны", "/files/02_ru.mp3"));
            chapterList.Add(new Chapter(3, "Садағаң кетейін, айналайын халқым!", "/files/03_kz.mp3", "На алтарь твой я жертвой взойду...", "/files/03_ru.mp3"));
            chapterList.Add(new Chapter(4, "Тәуелсіздік – тәуекелі тұратындардың еншісі", "/files/04_kz.mp3", "Независимость - удел сильных духом", "/files/04_ru.mp3"));
            chapterList.Add(new Chapter(5, "Ана тілі – ұлтымыздың айнасы", "/files/05_kz.mp3", "Родной язык - зеркало нации", "/files/05_ru.mp3"));
            chapterList.Add(new Chapter(6, "Тарихтан тағылым ала алсақ", "/files/06_kz.mp3", "Уроки истории", "/files/06_ru.mp3"));
            chapterList.Add(new Chapter(7, "Ел бірлігі – асыл қасиет", "/files/07_kz.mp3", "Единство народа - высшая добродетель", "/files/07_ru.mp3"));
            chapterList.Add(new Chapter(8, "Ұлттық намыс – ұлы ұғым", "/files/08_kz.mp3", "Национальное достоинство - великая ценность", "/files/08_ru.mp3"));
            chapterList.Add(new Chapter(9, "Демократия жарлықпен орнатылмайды", "/files/09_kz.mp3", "Демократия не устанавливается декретом", "/files/09_ru.mp3"));
            chapterList.Add(new Chapter(10, "Сенгенің игілікті еңбегің болсын", "/files/10_kz.mp3", "Надеяться только на доблестный труд", "/files/10_ru.mp3"));
            chapterList.Add(new Chapter(11, "Дәстүріңді баққаның – үмітіңді жаққаның", "/files/11_kz.mp3", "Следуя традициям, устремляться в будущее", "/files/11_ru.mp3"));
            chapterList.Add(new Chapter(12, "Өрелі рухсыз өркениет жоқ", "/files/12_kz.mp3", "Без гуманизма нет цивилизации", "/files/12_ru.mp3"));

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
                    PlaySound(
                        chapterList[StaticVariables.currentChapterID].titleList[StaticVariables.langID],
                        chapterList[StaticVariables.currentChapterID].pathToMp3List[StaticVariables.langID]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during MainPage.LayoutRoot_Loaded: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        void PlaySound(string fileName, string pathToMp3)
        {
            try
            {
                Song song = Song.FromUri("name", new Uri(pathToMp3, UriKind.Relative));
                Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                StaticVariables.currentPlayPauseImageUri = "music_pause_not_pressed.png";
                setImageSource(playImage, StaticVariables.currentPlayPauseImageUri);
                MediaPlayer.Play(song);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during MainPage.LayoutRoot_Loaded: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            try { while (NavigationService.RemoveBackEntry() != null) ; }
            catch (System.NullReferenceException ex)
            {
                MessageBox.Show("NullReferenceException during MainPage.LayoutRoot_Loaded: " + ex.Message, "Error", MessageBoxButton.OK);
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
                PlaySound(
                         chapterList[StaticVariables.currentChapterID].titleList[StaticVariables.langID],
                         chapterList[StaticVariables.currentChapterID].pathToMp3List[StaticVariables.langID]);
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
            }
            else
            {
                StaticVariables.currentChapterID = chapterList.Count - 1;
            }
            PlaySound(
                chapterList[StaticVariables.currentChapterID].titleList[StaticVariables.langID],
                chapterList[StaticVariables.currentChapterID].pathToMp3List[StaticVariables.langID]);

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
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)sender;
            if (img != null)
            {
                if (StaticVariables.currentPlayPauseImageUri.Contains("play"))
                {
                    PlaySound(
                        chapterList[StaticVariables.currentChapterID].titleList[StaticVariables.langID],
                        chapterList[StaticVariables.currentChapterID].pathToMp3List[StaticVariables.langID]);
                }
                else if (StaticVariables.currentPlayPauseImageUri.Contains("pause"))
                {
                    StaticVariables.currentPlayPauseImageUri = "music_play_not_pressed.png";
                    MediaPlayer.Pause();
                }
                setImageSource(img, StaticVariables.currentPlayPauseImageUri);
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
            }
            else
            {
                StaticVariables.currentChapterID = 0;
            }
            PlaySound(
                chapterList[StaticVariables.currentChapterID].titleList[StaticVariables.langID],
                chapterList[StaticVariables.currentChapterID].pathToMp3List[StaticVariables.langID]);
        }

        private void setImageSource(System.Windows.Controls.Image sender, string imgName)
        {
            System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation;
            bi.UriSource = new Uri("/Images/" + imgName, UriKind.Relative);
            sender.Source = bi;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                Slider slider = (Slider)sender;
                if (slider != null && MediaPlayer.Queue.ActiveSong != null)
                {
                    Song song = Song.FromUri("name", new Uri(chapterList[StaticVariables.currentChapterID].pathToMp3List[StaticVariables.langID], UriKind.Relative));


                    //long maxDuration = (long)MediaPlayer.NaturalDuration.TimeSpan.Seconds;
                    //long nextPosition = (long)maxDuration * (long)slider.Value;
                    //MediaPlayer.controls.currentPosition
                    //MediaPlayer.PlayPosition.Add(new TimeSpan((long)nextPosition));
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("NullReferenceException during MainPage.LayoutRoot_Loaded: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            double duration = (sender as Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer).Track.Duration.TotalSeconds;
            double currentPosition = (sender as Microsoft.Phone.BackgroundAudio.BackgroundAudioPlayer).Position.TotalSeconds;
            audioSlider.Value = currentPosition / duration * 100;
        }
    }
}