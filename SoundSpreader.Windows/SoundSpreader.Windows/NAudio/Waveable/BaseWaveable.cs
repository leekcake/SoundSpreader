using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundSpreader.Windows.NAudio.Waveable
{
    public abstract class BaseWaveable
    {
        public abstract void PushData(byte[] b, int length, WaveFormat format);

        public abstract int Latency
        {
            get; set;
        }

        public abstract float Volume
        {
            get; set;
        }

        public abstract string Summary
        {
            get;
        }

        protected abstract string RestoreData
        {
            get;
        }

        protected abstract string RestoreHeader
        {
            get;
        }

        protected static string[] Split(string data)
        {
            var split = data.Split(new string[] { "<a>" }, StringSplitOptions.RemoveEmptyEntries);
            return split;
        }

        public string Save()
        {
            return $"{RestoreHeader}<a>{Latency}<a>{Volume}<a>{RestoreData}";
        }
    }
}
