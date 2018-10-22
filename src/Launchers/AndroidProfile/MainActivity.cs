using System;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using WaveEngine.Common;
using WaveEngine.Adapter;
using WaveEngine.Common.Input;
using XamarinForms3DCarSample.Helpers;
using Xamarin.Forms;
using WaveEngine.Framework.Services;

namespace XamarinForms3DCarSample.Droid
{
    [Activity(
        Label = "XamarinForms3DCarSample",
        Icon = "@drawable/icon", MainLauncher = true,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsApplicationActivity, IAndroidApplication
    {
        private IGame game;
        private bool isGameInitialized = false;

        public GLView View { get; set; }

        public Activity Activity
        {
            get
            {
                return this;
            }
        }

        public int LayoutId { get; set; }

        private bool fullScreen;

        public IAdapter Adapter
        {
            get
            {
                if (View == null)
                {
                    return null;
                }
                else
                {
                    return View.Adapter;
                }
            }
        }

        public string WindowTitle
        {
            get { return string.Empty; }
        }

        public bool FullScreen
        {
            get
            {
                return fullScreen;
            }

            set
            {
                if (fullScreen != value)
                {
                    fullScreen = value;

                    if (Window != null)
                    {
                        if (value)
                        {
                            Window.AddFlags(WindowManagerFlags.Fullscreen);
                        }
                        else
                        {
                            Window.ClearFlags(WindowManagerFlags.Fullscreen);
                        }
                    }
                }
            }
        }

        public int Width
        {
            get { return Adapter.Width; }
        }

        public int Height
        {
            get { return Adapter.Height; }
        }

        private DisplayOrientation defaultOrientation;

        public DisplayOrientation SupportedOrientations { get; set; }

        public DisplayOrientation DefaultOrientation
        {
            get
            {
                return defaultOrientation;
            }

            set
            {
                if (defaultOrientation != value)
                {
                    defaultOrientation = value;
                }
            }
        }

        public bool SkipDefaultSplash
        {
            get; set;
        }

        public bool IsEditor => false;

        public ExecutionMode ExecutionMode => ExecutionMode.Standalone;

        public void Initialize()
        {
        }

        public void Initialize(IGame theGame)
        {
            MessagingCenter.Subscribe<MyScene>(this, MessengerKeys.SceneInitialized, OnSceneLoaded);

            game = theGame;
            isGameInitialized = false;
        }

        public void Update(TimeSpan elapsedTime)
        {
            if (game != null)
            {
                if (!isGameInitialized)
                {
                    game.Initialize(this);
                    isGameInitialized = true;
                }

                game.UpdateFrame(elapsedTime);
            }
        }

        public void OnSceneLoaded(MyScene scene)
        {
            WaveEngineFacade.Initialize(scene);
        }

        public void Draw(TimeSpan elapsedTime)
        {
            if (game != null)
            {
                game.DrawFrame(elapsedTime);
            }
        }

        public virtual void Exit()
        {
            Finish();
            Java.Lang.JavaSystem.Exit(0);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);

            HideSystemUI();

            base.OnCreate(savedInstanceState);

            this.VolumeControlStream = Android.Media.Stream.Music;

            Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (View != null)
            {
                View.Pause();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (View != null)
            {
                View.Resume();
            }
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && WaveServices.Platform != null)
            {
                WaveServices.Platform.Exit();
                return true;
            }

            if (View != null)
            {
                return View.OnKeyDown(keyCode, e);
            }

            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
        {
            bool handled = false;

            if (View != null)
            {
                handled = View.OnKeyUp(keyCode, e);
            }

            return handled;
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if(hasFocus)
            {
                HideSystemUI();
            }
        }

        private void HideSystemUI()
        {
            int uiOptions = (int)Window.DecorView.SystemUiVisibility;
            uiOptions |= (int)SystemUiFlags.Immersive;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
        }
    }
}