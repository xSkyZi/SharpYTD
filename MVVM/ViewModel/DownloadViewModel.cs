using SharpYTDWPF.Core;
using SharpYTDWPF.MVVM.Model;
using SharpYTDWPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpYTDWPF.MVVM.ViewModel
{
    internal class DownloadViewModel : Core.ViewModel
    {
        private IDownloadService _download;
        public IDownloadService Download
        {
            get => _download;
            set
            {
                _download = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ChangePathCommand { get; set; }
        public RelayCommand LoadFromFileCommand {  get; set; }
        public RelayCommand StartDownloadCommand { get; set; }
        public DownloadViewModel(IDownloadService downService)
        {
            Download = downService;
            ChangePathCommand = new RelayCommand(o => Download.ChangePath(), o => true);
            LoadFromFileCommand = new RelayCommand(o => Download.LoadFromFile(), o => true);
            StartDownloadCommand = new RelayCommand(o => Download.StartDownload(), o => true);
        }
    }
}
