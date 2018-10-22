using Xamarin.Forms;

namespace XamarinForms3DCarSample.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
		{
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            WaveEngineSurface.Game = App.Game;

            ForceLayout();
        }
    }
}