using SharpYTDWPF.Core;
using SharpYTDWPF.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SharpYTDWPF.Services
{
    public interface IDownloadService
    {
        string CurrentPath { get; }
        void ChangePath();
        void LoadFromFile();
        void StartDownload();
    }

    public class DownloadService : ObservableObject, IDownloadService
    {
        private string _currentPath;

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

        public DownloadService()
        {
            CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
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

        public async void StartDownload()
        {
            foreach (string line in UrlBox.Split('\n'))
            {
                if (Uri.TryCreate(line, UriKind.RelativeOrAbsolute, out Uri result))
                {
                    _queuedUrls.Add(result.ToString());
                }
            }
        }
    }
}
