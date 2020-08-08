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

        protected override string RestoreData => $"{DeviceID}/-_-\\{FriendlyName}";

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

            if(output == null)
            {
                output = new WasapiOut(device, AudioClientShareMode.Shared, false, 10);
                output.Init(bufferedWaveProvider);
                output.Play();
            }

            if(output != null && output.PlaybackState == PlaybackState.Playing)
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
