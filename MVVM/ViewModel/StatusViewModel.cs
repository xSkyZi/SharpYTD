﻿using SharpYTDWPF.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpYTDWPF.MVVM.ViewModel
{
    public class StatusViewModel : Core.ViewModel
    {

        public ObservableCollection<VideoFile> ActiveFileQueue { get; set; }
        public StatusViewModel()
        {
            ActiveFileQueue = QueueManager.GetCurrentQueue();
        }
    }
}
