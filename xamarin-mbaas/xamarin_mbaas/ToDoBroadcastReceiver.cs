using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

//2. �ȉ��� using �� ToDoBroadcastReceiver �N���X�ɒǉ����܂��F
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
//2. �����܂�

//3. permission �̃��N�G�X�g�� using �X�e�[�g�����g�� namespace �錾�̊Ԃɒǉ����Ă��������F
[assembly: Permission(Name = "<�A�v���̃p�b�P�[�W��>.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "<�A�v���̃p�b�P�[�W��>.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS �� Android 4.0.3 �ȉ��ŕK�v�ł��B
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//3. �����܂�

namespace xamarin_mbaas
{
    //4. ������ ToDoBroadcastReceiver �N���X�̒�`���ȉ��Œu�������܂��F
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE },
        Categories = new string[] { "<�A�v���̃p�b�P�[�W��>" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
        Categories = new string[] { "<�A�v���̃p�b�P�[�W��>" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
        Categories = new string[] { "<�A�v���̃p�b�P�[�W��>" })]

    // 4. �y���Ӂz�h�L�������g�ł� <TIntentService> �� <GcmService> �ł����A�������� <PushHandlerService> �ł��B
    public class ToDoBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
    {
        // �uGoogle Cloud Messaging ��L���ɂ���v�ōT���Ă����� �v���W�F�N�g�ԍ� ���L�q���Ă��������B
        public static string[] senderIDs = new string[] { "xxxxxxxxxxxx" };
    }
    //4. �����܂�

    //5. ToDoBroadcastReceiver.cs ���� PushHandlerService �N���X���`����ȉ��̃R�[�h��ǉ����܂��F
    // ���̃T�[�r�X��`�͂��̃N���X�ɓK�p���Ȃ���΂����܂���B
    [Service]
    public class PushHandlerService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }

        public PushHandlerService() : base(ToDoBroadcastReceiver.senderIDs) { }

        //6. PushHandlerService �N���X�� OnRegistered �C�x���g�n���h���[�� override ���܂��B
        // �y���Ӂz�h�L�������g�ł� ToDoBroadcastReceiver �N���X�� override ���Ă��܂����A�������� PushHandlerService �N���X���ł��B
        protected override void OnRegistered(Context context, string registrationId)
        {
            System.Diagnostics.Debug.WriteLine("The device has been registered with GCM.", "Success!");

            // Get the MobileServiceClient from the current activity instance.
            MobileServiceClient client = ToDoActivity.CurrentActivity.CurrentClient;
            var push = client.GetPush();

            List<string> tags = null;

            //// (Optional) Uncomment to add tags to the registration.
            //var tags = new List<string>() { "myTag" }; // create tags if you want

            try
            {
                // Make sure we run the registration on the same thread as the activity, 
                // to avoid threading errors.
                ToDoActivity.CurrentActivity.RunOnUiThread(
                    async () => await push.RegisterNativeAsync(registrationId, tags));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("Error with Azure push registration: {0}", ex.Message));
            }
        }
        //6. �����܂�

        //7. PushHandlerService �N���X�� OnMessage ���\�b�h�� override ���܂��F
        protected override void OnMessage(Context context, Intent intent)
        {
            string message = string.Empty;

            // Extract the push notification message from the intent.
            if (intent.Extras.ContainsKey("message"))
            {
                message = intent.Extras.Get("message").ToString();
                var title = "�A�C�e�����ǉ�����܂���:";

                // Create a notification manager to send the notification.
                var notificationManager =
                    GetSystemService(Context.NotificationService) as NotificationManager;

                // Create a new intent to show the notification in the UI. 
                PendingIntent contentIntent =
                    PendingIntent.GetActivity(context, 0,
                    new Intent(this, typeof(ToDoActivity)), 0);

                // Create the notification using the builder.
                var builder = new Notification.Builder(context);
                builder.SetAutoCancel(true);
                builder.SetContentTitle(title);
                builder.SetContentText(message);
                builder.SetSmallIcon(Resource.Drawable.ic_launcher);
                builder.SetContentIntent(contentIntent);
                var notification = builder.Build();

                // Display the notification in the Notifications Area.
                notificationManager.Notify(1, notification);

            }
        }
        //7. �����܂�

        //8. OnUnRegistered() �� OnError() ���\�b�h�� override ���܂�
        protected override void OnUnRegistered(Context context, string registrationId)
        {
            throw new NotImplementedException();
        }

        protected override void OnError(Context context, string errorId)
        {
            System.Diagnostics.Debug.WriteLine(
                string.Format("Error occurred in the notification: {0}.", errorId));
        }
        //8. �����܂�
    }



}