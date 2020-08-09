using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SoundSpreader.Windows.NAudio
{
    public class SimpleWaveProvider : IWaveProvider
    {
        public SimpleWaveProvider(WaveFormat waveFormat)
        {
            WaveFormat = waveFormat;
        }

        public WaveFormat WaveFormat { get; }

        private byte[] received;
        private int roffset;
        private int rcount;

        public void Set(byte[] buffer, int offset, int count)
        {
            received = buffer;
            roffset = offset;
            rcount = count;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var workCount = count;
            if(workCount > rcount)
            {
                workCount = rcount;
            }
            if(workCount > buffer.Length - offset)
            {
                workCount = buffer.Length - offset;
            }
            Buffer.BlockCopy(received, roffset, buffer, offset, workCount);
            return workCount;
        }
    }
}
