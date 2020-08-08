using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SoundSpreader.Windows.NAudio;
using SoundSpreader.Windows.NAudio.Waveable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SoundSpreader.Windows
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private NWaveSharer sharer;

        private DispatcherTimer timer = new DispatcherTimer();
        private WasapiLoopbackCapture capture;

        public MainWindow()
        {
            InitializeComponent();
            capture = new WasapiLoopbackCapture();
            capture.DataAvailable += Capture_DataAvailable;
            capture.StartRecording();

            sharer = new NWaveSharer();

            RefreshDeviceList();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var list = new string[sharer.waveables.Count];
            for(int i = 0; i < list.Length; i++)
            {
                list[i] = sharer.waveables[i].Summary;
            }
            ReceiverListBox.ItemsSource = list;
        }

        public void RefreshDeviceList()
        {
            var list = new List<string>();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All))
            {
                try
                {
                    if(device.State == DeviceState.Active)
                    {
                        list.Add(device.FriendlyName);
                    }
                }
                catch
                {

                }
            }
            DeviceListComboBox.ItemsSource = list;
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            sharer.PushData(e.Buffer, e.BytesRecorded, capture.WaveFormat);
        }

        private void RegisterDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            if(DeviceListComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("장치를 선택하지 않은것으로 보입니다.");
                return;
            }

            var device = LocalWaveable.FindDeviceByData(friendlyName: DeviceListComboBox.Text);
            if(device == null)
            {
                MessageBox.Show("선택한 장치가 현재는 없는것 같습니다");
                return;
            }
            var waveable = new LocalWaveable(device.ID, device.FriendlyName);
            sharer.RegisterWaveable(waveable);
        }

        private void RefreshDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDeviceList();
        }
    }
}
