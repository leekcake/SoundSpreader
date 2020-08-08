using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundSpreader.Windows.NAudio.Waveable
{
    public class LocalWaveable : BaseWaveable
    {
        private MMDevice device;
        
        private WasapiOut output;
        private BufferedWaveProvider bufferedWaveProvider;

        private int creationLatency = 0;
        public override int Latency { get; set; } = 10;
        public override float Volume { get; set; } = 1;

        public readonly string DeviceID;
        public readonly string FriendlyName;

        public LocalWaveable(string deviceId, string friendlyName)
        {
            DeviceID = deviceId;
            FriendlyName = friendlyName;
        }

        public override string Summary {
            get
            {
                if (bufferedWaveProvider == null)
                {
                    return $"{FriendlyName}: 초기화중";
                }

                if (device == null)
                {
                    return $"{FriendlyName}: 장치에 연결할 수 없음";
                }

                if(output == null)
                {
                    return $"{FriendlyName}: 이 장치를 누군가 사용하고 있는것 같습니다";
                }

                return $"{FriendlyName}: 작동중";
            }
        }

        protected override string RestoreData => $"{DeviceID}<b>{FriendlyName}";

        protected override string RestoreHeader => "LocalWaveable";

        public static LocalWaveable Load(string data)
        {
            var split = Split(data);
            var resplit = split[3].Split(new string[] { "<b>" }, StringSplitOptions.RemoveEmptyEntries);
            var result = new LocalWaveable(resplit[0], resplit[1]);

            result.Latency = int.Parse(split[1]);
            result.Volume = float.Parse(split[2]);

            return result;
        }

        public override void PushData(byte[] b, int length, WaveFormat format)
        {
            if (bufferedWaveProvider == null) {
                bufferedWaveProvider = new BufferedWaveProvider(format);
            }
            if (device == null)
            {
                device = FindDeviceByData(DeviceID, FriendlyName);
                if(device == null)
                {
                    return;
                }
            }

            if(creationLatency != Latency && output != null)
            {
                try
                {
                    output.Dispose();
                }
                catch
                {

                }
                output = null;
                return;
            }

            if(output == null)
            {
                creationLatency = Latency;
                output = new WasapiOut(device, AudioClientShareMode.Shared, false, Latency);
                output.Init(bufferedWaveProvider);
                output.Play();
            }

            if (Volume != output.Volume)
            {
                output.Volume = Volume;
            }

            if (output != null && output.PlaybackState == PlaybackState.Playing)
            {
                bufferedWaveProvider.AddSamples(b, 0, length);
            }
        }

        public static MMDevice FindDeviceByData(string id = null, string friendlyName = null)
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All))
            {
                try
                {
                    if (device.State == DeviceState.Active)
                    {
                        if(device.ID == id)
                        {
                            return device;
                        }
                        else if(device.FriendlyName == friendlyName)
                        {
                            return device;
                        }
                    }
                }
                catch
                {

                }
            }
            return null;
        }
    }
}
