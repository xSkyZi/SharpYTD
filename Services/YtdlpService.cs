using SharpYTDWPF.Core;
using SharpYTDWPF.MVVM.Model;
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
using YoutubeDLSharp.Metadata;
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
        SemaphoreSlim semaphore = new SemaphoreSlim(10);
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
                    DownloadFile(line);
                }
            }
        }

        private async Task DownloadFile(string Uri)
        {
            Debug.WriteLine("DownloadFile()");

            await semaphore.WaitAsync();

            YoutubeDL ytdl = new YoutubeDL();
            ytdl.FFmpegPath = Directory.GetCurrentDirectory() + "\\ffmpeg.exe";
            ytdl.YoutubeDLPath = Directory.GetCurrentDirectory() + "\\yt-dlp.exe";
            ytdl.OutputFileTemplate = "%(title)s.%(ext)s";
            ytdl.OutputFolder = _currentPath;

            CancellationTokenSource cts = new CancellationTokenSource();
            VideoFile vid = new VideoFile();
            QueueManager.AddToActiveQueue(vid);

            var VideoProgress = new Progress<DownloadProgress>(p => ProgressUpdate(p, vid));
            
            try
            {
                await FetchVideoTitle(Uri, vid, ytdl);
                var res = await ytdl.RunVideoDownload(Uri, ct: cts.Token, progress: VideoProgress);
                if (!res.Success) vid.Status = "Error";
            } catch (Exception ex)
            {
                vid.Status = "Error";
            } finally
            {
                semaphore.Release();
            }
        }

        private async void ProgressUpdate(DownloadProgress p, VideoFile vid)
        {
            if (p.State != DownloadState.Success)
            {
                vid.Progress = (int)(p.Progress * 100);
            }

            vid.Speed = p.DownloadSpeed;
            vid.Eta = p.ETA;
            if (p.State == DownloadState.None || p.State == DownloadState.Error || p.State == DownloadState.Success || p.State == DownloadState.PreProcessing)
            {
                vid.Eta = "--";
                vid.Speed = "--";
            }
            if (p.State != DownloadState.Error) vid.Status = p.State.ToString();
            OnPropertyChanged();
        }

        private async Task FetchVideoTitle(string Uri, VideoFile vid, YoutubeDL ytdl)
        {
            try
            {
                var res = await ytdl.RunVideoDataFetch(Uri);
                VideoData data = res.Data;
                vid.Title = data.Title;
            } catch (Exception ex)
            {

            }
        }
    }
}
