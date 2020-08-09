using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NAudio.Wave.Compression;
using NAudio.Wave.SampleProviders;

namespace SoundSpreader.Windows.NAudio.Waveable
{
    public class RemoteWaveable : BaseWaveable
    {
        public override int Latency { get; set; }
        public override float Volume { get; set; }

        public override string Summary
        {
            get
            {
                if (!client.Connected)
                {
                    return $"{RemoteIP} - 연결안됨";
                }
                return $"{RemoteIP} - 작동중";
            }
        }

        protected override string RestoreData => RemoteIP;

        protected override string RestoreHeader => "RemoteWaveable";

        public readonly string RemoteIP;

        private TcpClient client;

        public RemoteWaveable(string ip)
        {
            RemoteIP = ip;

            client = new TcpClient();
            connector = Task.Run(() =>
            {
                Connector();
            });
        }

        public void Connector()
        {
            while (true)
            {
                try
                {
                    client.Connect(RemoteIP, 10039);
                    break;
                }
                catch
                {

                }
            }
            connector = null;
        }

        public static RemoteWaveable Load(string data)
        {
            var split = Split(data);
            var result = new RemoteWaveable(split[3]);
            result.Latency = int.Parse(split[1]);
            result.Volume = float.Parse(split[2]);

            return result;
        }

        private RawSourceWaveStream waveBuffer;
        private MemoryStream waveBackBuffer = new MemoryStream(1024 * 100);

        private SampleToWaveProvider16 waveChanger;
        private byte[] copy = new byte[1024 * 256];
        private MemoryStream memoryStream = new MemoryStream(1024 * 512);

        private Task connector;

        public override void PushData(byte[] b, int length, WaveFormat format)
        {
            if (waveBuffer == null)
            {
                waveBuffer = new RawSourceWaveStream(waveBackBuffer, format);
                waveChanger = new SampleToWaveProvider16(waveBuffer.ToSampleProvider());
            }

            if (!client.Connected)
            {
                if (connector == null)
                {
                    connector = Task.Run(() =>
                    {
                        Connector();
                    });
                }
                return;
            }            
            waveBackBuffer.SetLength(0);
            waveBackBuffer.Write(b, 0, length);
            waveBuffer.Position = 0;

            var readed = waveChanger.Read(copy, 0, length);
            memoryStream.Write(copy, 0, readed);

            if (memoryStream.Length > 1024 * 16)
            {
                client.GetStream().Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                client.GetStream().Flush();
                memoryStream.SetLength(0);
            }
        }
    }
}
