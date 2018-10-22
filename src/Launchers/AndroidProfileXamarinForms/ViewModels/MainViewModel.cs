using System;
using System.Threading.Tasks;
using XamarinForms3DCarSample.Helpers;
using XamarinForms3DCarSample.ViewModels.Base;

namespace XamarinForms3DCarSample.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isInit;

        public MainViewModel()
        {
            _isInit = false;
        }

        public bool IsInit
        {
            get { return _isInit; }
            set
            {
                _isInit = value;
                RaisePropertyChanged(() => IsInit);
            }
        }

        public override Task InitializeAsync(object navigationData)
        {
            WaveEngineFacade.Initialized += OnInitialized;

            return Task.FromResult(true);
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            IsInit = true;
        }
    }
}