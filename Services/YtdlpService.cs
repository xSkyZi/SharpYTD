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
        SemaphoreSlim downloadSemaphore = new SemaphoreSlim(5);
        SemaphoreSlim fetchSemaphore = new SemaphoreSlim(5);
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

        private INavigationService _navigation;
        public INavigationService Navigation
        {
            get => _navigation;
            set
            {
                _navigation = value;
                OnPropertyChanged();
            }
        }

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
            Navigation.NavigateTo<StatusViewModel>();
            List<string> urlBoxContent = UrlBox.Split('\n').ToList();
            UrlBox = string.Empty;
            foreach (string line in urlBoxContent)
            {
                if (Uri.TryCreate(line, UriKind.RelativeOrAbsolute, out Uri result))
                {
                    VideoFile vid = new VideoFile();
                    DownloadFile(line, vid);
                }
            }

        }
        private async Task DownloadFile(string Uri, VideoFile vid)
        {
            QueueManager.AddToActiveQueue(vid);

            YoutubeDL ytdl = new YoutubeDL();
            ytdl.FFmpegPath = Directory.GetCurrentDirectory() + "\\ffmpeg.exe";
            ytdl.YoutubeDLPath = Directory.GetCurrentDirectory() + "\\yt-dlp.exe";
            ytdl.OutputFileTemplate = "%(title)s.%(ext)s";
            ytdl.OutputFolder = _currentPath;

            if (vid.Title == "Fetching Video Title") if (!await FetchVideoTitle(Uri, vid, ytdl)) return;

            await downloadSemaphore.WaitAsync();

            CancellationTokenSource cts = new CancellationTokenSource();

            var VideoProgress = new Progress<DownloadProgress>(p => ProgressUpdate(p, vid));
            
            try
            {
                var res = await ytdl.RunVideoDownload(Uri, ct: cts.Token, progress: VideoProgress);
                if (!res.Success)
                {
                    vid.Status = "Error";
                    foreach (string line in res.ErrorOutput)
                    {
                        Debug.WriteLine(line);
                    }
                }
            } catch (Exception ex)
            {
                vid.Status = "Error";
            } finally
            {
                downloadSemaphore.Release();
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

        private async Task<bool> FetchVideoTitle(string Uri, VideoFile vid, YoutubeDL ytdl)
        {

            await fetchSemaphore.WaitAsync();

            try
            {
                var res = await ytdl.RunVideoDataFetch(Uri);
                if (!res.Success)
                {
                    vid.Status = "Error";
                    fetchSemaphore.Release();
                    return false;
                }
                VideoData data = res.Data;
                if (data.ResultType == MetadataType.Playlist)
                {
                    GetPlaylistSongs(data.Entries, data.Title);
                    fetchSemaphore.Release();
                    QueueManager.RemoveFromActiveQueue(vid);
                    return false;
                }
                vid.Title = data.Title;
                fetchSemaphore.Release();
                return true;
            } catch (Exception ex)
            {
                vid.Status = "Error";
                fetchSemaphore.Release();
                return false;
            }
        }

        private async void GetPlaylistSongs(VideoData[] videos, string playlistName)
        {
            foreach (VideoData vid in videos)
            {
                VideoFile vidFile = new VideoFile();
                vidFile.Title = vid.Title + $" [From Playlist \"{playlistName}\"]";
                DownloadFile(vid.Url, vidFile);
            }
        }

        public YtdlpService(INavigationService navService)
        {
            Navigation = navService;
        }
    }
}
