using NAudio.Wave;
using SoundSpreader.Windows.NAudio.Waveable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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

        public void UnregisterWaveable(BaseWaveable waveable)
        {
            lock(waveables)
            {
                waveables.Remove(waveable);
            }
        }

        public void Load(string path)
        {
            if(!File.Exists(path))
            {
                return;
            }
            lock (waveables) {
                foreach (var data in File.ReadAllLines(path))
                {
                    if(data.StartsWith("LocalWaveable"))
                    {
                        waveables.Add(LocalWaveable.Load(data));
                    }
                }
            }
        }

        public void Save(string path)
        {
            var writer = new StreamWriter(path);
            foreach(var waveable in waveables)
            {
                writer.WriteLine(waveable.Save());
            }
            writer.Close();
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
