using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpYTDWPF.MVVM.Model
{
    public class QueueManager
    {
        public static ObservableCollection<VideoFile> _activeQueue = new ObservableCollection<VideoFile>();

        public static ObservableCollection<VideoFile> GetCurrentQueue()
        {
            return _activeQueue;
        }

        public static void AddToActiveQueue(VideoFile videoFile)
        {
            _activeQueue.Add(videoFile);
        }

        public static void RemoveFromActiveQueue(VideoFile videoFile)
        {
            _activeQueue.Remove(videoFile);
        }
    }
}
