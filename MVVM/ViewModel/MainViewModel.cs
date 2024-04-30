using SharpYTDWPF.Core;
using SharpYTDWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpYTDWPF.MVVM.ViewModel
{
    internal class MainViewModel : Core.ViewModel
    {
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

        private IUpdateService _updateService;

        public IUpdateService UpdateService
        {
            get => _updateService;
            set
            {
                _updateService = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateDownloadCommand { get; set; }
        public RelayCommand NavigateConvertCommand { get; set; }
        public RelayCommand NavigateStatusCommand { get; set; }
        public RelayCommand NavigateSettingsCommand { get; set; }

        public MainViewModel(INavigationService navService, IUpdateService updateService)
        {
            Navigation = navService;
            UpdateService = updateService;
            NavigateDownloadCommand = new RelayCommand(o => { Navigation.NavigateTo<DownloadViewModel>(); }, o => true);
            NavigateConvertCommand = new RelayCommand(o => { Navigation.NavigateTo<ConvertViewModel>(); }, o => true);
            NavigateStatusCommand = new RelayCommand(o => { Navigation.NavigateTo<StatusViewModel>(); }, o => true);
            NavigateSettingsCommand = new RelayCommand(o => { Navigation.NavigateTo<SettingsViewModel>(); }, o => true);
            Navigation.NavigateTo<DownloadViewModel>();
            UpdateService.CheckFiles();
        }

        
    }
}
