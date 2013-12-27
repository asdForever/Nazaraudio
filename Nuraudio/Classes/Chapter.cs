using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NazarAudio.Classes
{
    class Chapter
    {
        public int chapterID;
        public List<string> titleList;
        public List<string> pathToMp3List;

        public Chapter(int chapterIDIn, string chapterNameKzIn, string pathToMp3KzIn, string chapterNameRuIn, string pathToMp3RuIn)
        {
            chapterID = chapterIDIn;

            titleList = new List<string>();
            titleList.Add(chapterNameKzIn);
            titleList.Add(chapterNameRuIn);

            pathToMp3List = new List<string>();
            pathToMp3List.Add(pathToMp3KzIn);
            pathToMp3List.Add(pathToMp3RuIn);
        }
    }
}
