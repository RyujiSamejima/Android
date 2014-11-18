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

using Android.Support.V4.App;

namespace Wear_Sample
{
    [Activity(Label = "StartActionActivity")]
    public class StartActionActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.StartAction);

            // Wear ����� Intent ���󂯎��BgetIntent() ���\�b�h�ɑ����B
            // http://www.buildinsider.net/mobile/xamarintips/0004 �Q��
            var intent = this.Intent;
            // RemoteInput.GetResultsFromIntent ���� Android.RemoteInput �Ƌ�������̂Ńt���Ŏw�肵�Ă��܂��B
            // �����ȏ������͕s���ł��B�B
            var remoteInput = Android.Support.V4.App.RemoteInput.GetResultsFromIntent(intent);
            var reply = remoteInput.GetCharSequence("Reply");

            var textView = FindViewById<TextView>(Resource.Id.textView1);
            System.Diagnostics.Debug.WriteLine("{0} {1}", "User reply from wearable: ", reply);
            textView.Text = string.Format("{0} {1}", "User reply from wearable: ", reply);

        }
    }
}