using NAudio.Wave;
using SoundSpreader.Windows.NAudio.Waveable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundSpreader.Windows.NAudio
{
    /// <summary>
    /// 받은 Wave를 다른 리시버로 발송합니다
    /// </summary>
    public class NWaveSharer
    {
        public readonly List<BaseWaveable> waveables = new List<BaseWaveable>();
        public void RegisterWaveable(BaseWaveable waveable)
        {
            lock(waveables)
            {
                waveables.Add(waveable);
            }
        }

        public void Load(string path)
        {

        }

        public void Save(string path)
        {

        }

        public void PushData(byte[] b, int len, WaveFormat format)
        {
            lock(waveables)
            {
                foreach(var waveable in waveables)
                {
                    waveable.PushData(b, len, format);
                }
            }
        }
    }
}
