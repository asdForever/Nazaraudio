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
        public List<double> durationList;

        public Chapter(int chapterIDIn, string chapterNameKzIn, string chapterNameRuIn, double durationKzIn, double durationRuIn)
        {
            chapterID = chapterIDIn;

            titleList = new List<string>();
            titleList.Add(chapterNameKzIn);
            titleList.Add(chapterNameRuIn);

            durationList = new List<double>();
            durationList.Add(durationKzIn);
            durationList.Add(durationRuIn);
        }

        public int getTrackNumber()
        {
            if (StaticVariables.langID == 0)
                return chapterID - 1;
            else
                return chapterID + 11;
        }
    }
}
