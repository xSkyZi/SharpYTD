using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SharpYTDWPF.MVVM.Model
{
    public class VideoFile
    {
        public VideoFile()
        {
            
        }

        public string? Title { get; set; }
        public int? Progress { get; set; }
        public string? Speed { get; set; }
        public string? Status { get; set;}
        public string? Eta { get; set;}
    }
}
