using SharpYTDWPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SharpYTDWPF.MVVM.Model
{
    public class VideoFile : ObservableObject
    {
        public VideoFile()
        {
            
        }

        private string _title = "Fetching Video Title";
        public string? Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
        private string _speed;
        public string? Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                OnPropertyChanged();
            }
        }
        private string _status = "In Queue";
        public string? Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }
        private string _eta;
        public string? Eta
        {
            get => _eta;
            set
            {
                _eta = value;
                OnPropertyChanged();
            }
        }
    }
}
