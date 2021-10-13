using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using EncryptDecryptWidget.Resources.crypto;
using System;

namespace EncryptDecryptWidget
{
    [Service]
    internal class EncryptDecryptWidgetService : Service, View.IOnTouchListener
    {
        #region Declare variables
        private readonly CryptoHelper cryptoHelper = new CryptoHelper();
        private readonly string encryptionKey = "YourPasswordKey";
        private bool expanded;
        private IWindowManager windowManager;
        private WindowManagerLayoutParams layoutParams;
        private View floatingView, expandedView;
        private Button decryptMessage, encryptMessage;
        private ImageView close, icon;
        private ClipboardManager clipboard;
        private int initialX, initialY;
        private float initialTouchX, initialTouchY;
        #endregion


        public override void OnCreate()
        {
            base.OnCreate();

            #region Declare views
            floatingView = LayoutInflater.From(this).Inflate(Resource.Layout.expanded_layout, null);
            expandedView = floatingView.FindViewById(Resource.Id.flyout);
            icon = floatingView.FindViewById<ImageView>(Resource.Id.icon);
            close = floatingView.FindViewById<ImageView>(Resource.Id.close);
            encryptMessage = floatingView.FindViewById<Button>(Resource.Id.encryptMessage);
            decryptMessage = floatingView.FindViewById<Button>(Resource.Id.decryptMessage);
            #endregion

            clipboard = (ClipboardManager)GetSystemService(ClipboardService);

            SetTouchListener();

            layoutParams = new WindowManagerLayoutParams(
            ViewGroup.LayoutParams.WrapContent,
            ViewGroup.LayoutParams.WrapContent,
            WindowManagerTypes.ApplicationOverlay,
            WindowManagerFlags.NotFocusable,
            Format.Translucent)
            { Gravity = GravityFlags.Center | GravityFlags.Left };

            windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            windowManager.AddView(floatingView, layoutParams);

            #region Declare click actions
            encryptMessage.Click += delegate
            {
                expandedView.Visibility = ViewStates.Gone;
                EncryptMsg();
            };

            decryptMessage.Click += delegate
            {
                expandedView.Visibility = ViewStates.Gone;
                DecryptMsg();
            };

            close.Click += delegate
            {
                StopService(new Intent(this, typeof(EncryptDecryptWidgetService)));
            };
            #endregion
        }

        private void EncryptMsg()
        {
            try
            {
                var encryptText = cryptoHelper.PasswordEncrypt(clipboard.PrimaryClip.GetItemAt(0).Text, encryptionKey);
                var msg = ClipData.NewPlainText("text", encryptText); //get and encrypt string from clipboard
                clipboard.PrimaryClip = msg;

                Toast.MakeText(this, "Encrypted : )", ToastLength.Short).Show();
            }

            catch (Exception ex)
            {
                Toast.MakeText(this, $"Error:{ex}", ToastLength.Long).Show();
            };
        }
        private void DecryptMsg()
        {
            try
            {
                var decryptText = cryptoHelper.PasswordDecrypt(clipboard.PrimaryClip.GetItemAt(0).Text, encryptionKey);
                Toast.MakeText(this, decryptText, ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Error:{ex}", ToastLength.Long).Show();
            };
        }

        private void SetTouchListener()
        {
            var mainContainer = floatingView.FindViewById<RelativeLayout>(Resource.Id.root);
            mainContainer.SetOnTouchListener(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (floatingView != null)
            {
                windowManager.RemoveView(floatingView);
            }
        }

        /// <summary>
        /// Gesture to move widget
        /// </summary>
        public bool OnTouch(View view, MotionEvent motion)
        {
            switch (motion.Action)
            {
                case MotionEventActions.Down:
                    {
                        //initial position
                        initialX = layoutParams.X;
                        initialY = layoutParams.Y;

                        //touch point
                        initialTouchX = motion.RawX;
                        initialTouchY = motion.RawY;

                        return true;
                    }

                case MotionEventActions.Up:
                    {
                        if (expanded == false)
                        {
                            expandedView.Visibility = ViewStates.Gone;
                            expanded = true;
                        }
                        else
                        {
                            expandedView.Visibility = ViewStates.Visible;
                            expanded = false;
                        }
                    }
                    return true;

                case MotionEventActions.Move:
                    {
                        layoutParams.X = initialX + (int)(motion.RawX - initialTouchX);
                        layoutParams.Y = initialY + (int)(motion.RawY - initialTouchY);

                        expandedView.Visibility = ViewStates.Gone;

                        windowManager.UpdateViewLayout(floatingView, layoutParams);
                        return true;
                    }
            }
            return false;
        }
        public override IBinder OnBind(Intent intent) { return null; }
    }
}
