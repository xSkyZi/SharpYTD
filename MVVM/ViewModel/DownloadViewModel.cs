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
        private IYtdlpService _ytdlp;
        public IYtdlpService Ytdlp
        {
            get => _ytdlp;
            set
            {
                _ytdlp = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ChangePathCommand { get; set; }
        public RelayCommand LoadFromFileCommand {  get; set; }
        public RelayCommand StartDownloadCommand { get; set; }
        public DownloadViewModel(IYtdlpService downService)
        {
            Ytdlp = downService;
            ChangePathCommand = new RelayCommand(o => Ytdlp.ChangePath(), o => true);
            LoadFromFileCommand = new RelayCommand(o => Ytdlp.LoadFromFile(), o => true);
            StartDownloadCommand = new RelayCommand(o => Ytdlp.CheckForValidUris(), o => true);
        }
    }
}
