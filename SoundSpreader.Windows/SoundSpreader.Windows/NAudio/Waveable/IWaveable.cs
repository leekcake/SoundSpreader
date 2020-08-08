using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundSpreader.Windows.NAudio.Waveable
{
    public interface IWaveable
    {
        void PushData(byte[] b, int length, WaveFormat format);

        string Summary
        {
            get;
        }

        string RestoreData
        {
            get;
        }
    }
}
