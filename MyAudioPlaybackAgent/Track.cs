using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyAudioPlaybackAgent
{
    class Track
    {
        public Uri uri;
        public string fileName;
        public string trackName;
        public string artist;
        public string album;

        public Track(String fileNameIn, string trackNameIn)
        {
            uri = new Uri(fileNameIn, UriKind.Relative);
            fileName = fileNameIn;
            trackName = trackNameIn;
            artist = "Нурсултан Назарбаев";
            album = "Ой бөлістім халқыммен";
        }
    }
}
