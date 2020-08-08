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

        public abstract string Summary
        {
            get;
        }

        protected abstract string RestoreData
        {
            get;
        }
    }
}
