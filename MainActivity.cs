using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace EncryptDecryptWidget
{
    [Activity(MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            PermissionCheck();
            StartService(new Intent(this, typeof(EncryptDecryptWidgetService)));
            Finish();
        }

        private void PermissionCheck()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                if (!Settings.CanDrawOverlays(this))
                {
                    StartActivityForResult(new Intent(Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + PackageName)), 0);
                }
                else if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ForegroundService, Manifest.Permission.SystemAlertWindow }, 5469);
                }
                else { return; }
            }

        }

    }
}
