using SharpYTDWPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SharpYTDWPF.MVVM.Model
{
    public class VideoFileManager
    {
        public static ObservableCollection<VideoFile> _listFiles = new ObservableCollection<VideoFile>() { new VideoFile() { Title = "WPF MVVM Tutorial: Build An App with Data Binding and Commands", Progress = 45, Speed = "0.5 Kbps", Status = "Downloading", Eta = "01:23"}, new VideoFile() { Title = "Gaslighting silvers", Progress = 98, Speed = "2.1 Gbps", Status = "Downloading", Eta = "0:23" } };

        public static ObservableCollection<VideoFile> GetVideoFiles()
        {
            return _listFiles;
        }
    }
}
