using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Genesyslab.Desktop.Modules.YoutubeWorkItem.ViewModels
{
    public class SpinnerViewModel : INotifyPropertyChanged
    {
        private bool _isSpinnerVisible;

        public bool IsSpinnerVisible
        {
            get => _isSpinnerVisible;
            set
            {
                _isSpinnerVisible = value;
                OnPropertyChanged(nameof(IsSpinnerVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}