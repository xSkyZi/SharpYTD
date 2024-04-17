using SharpYTDWPF.Core;
using SharpYTDWPF.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using YoutubeDLSharp;
namespace SharpYTDWPF.Services
{
    public interface IYtdlpService
    {
        string UrlBox {  get; }
        string CurrentPath { get; }
        void ChangePath();
        void LoadFromFile();
        void CheckForValidUris();
    }

    public class YtdlpService : ObservableObject, IYtdlpService
    {
        private string _currentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        private string _urlBox;
        public string UrlBox
        {
            get => _urlBox;
            set
            {
                _urlBox = value;
                OnPropertyChanged();
            }
        }

        private List<string> _queuedUrls = new List<string>();
        private bool _isDownloading = false;

        public async void ChangePath()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    CurrentPath = fbd.SelectedPath;
                }
            }
        }

        public async void LoadFromFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt";
                DialogResult dialogResult = ofd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    using (StreamReader sr = new StreamReader(ofd.OpenFile()))
                    {
                        UrlBox = await sr.ReadToEndAsync();
                    }
                }
            }
        }

        public async void CheckForValidUris()
        {
            foreach (string line in UrlBox.Split('\n'))
            {
                if (Uri.TryCreate(line, UriKind.RelativeOrAbsolute, out Uri result))
                {
                    _queuedUrls.Add(result.ToString());
                }
            }
            if (!_isDownloading) StartDownload();
        }

        private async void StartDownload()
        {
            Debug.WriteLine("StartDownload()");
            List<Task> tasks = new List<Task>();
            SemaphoreSlim semaphore = new SemaphoreSlim(10);

            while (_queuedUrls.Count > 0)
            {
                await semaphore.WaitAsync();

                tasks.Add(DownloadFile(_queuedUrls[0], semaphore));
                _queuedUrls.RemoveAt(0);
            }

            await Task.WhenAll(tasks);

            _isDownloading = false;
        }

        private async Task DownloadFile(string Uri, SemaphoreSlim semaphore)
        {
            Debug.WriteLine("DownloadFile()");
            YoutubeDL ytdl = new YoutubeDL();
            ytdl.FFmpegPath = Directory.GetCurrentDirectory() + "\\ffmpeg.exe";
            ytdl.YoutubeDLPath = Directory.GetCurrentDirectory() + "\\yt-dlp.exe";
            ytdl.OutputFileTemplate = "%(title)s.%(ext)s";
            ytdl.OutputFolder = _currentPath;
            CancellationTokenSource cts = new CancellationTokenSource();

            try
            {
                await ytdl.RunVideoDownload(Uri, ct: cts.Token);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            } finally
            {
                semaphore.Release();
            }
        }
    }
}
