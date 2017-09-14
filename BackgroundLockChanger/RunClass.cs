namespace BackgroundLockChanger
{
    using System;
    using Windows.ApplicationModel.Background;
    using Windows.Data.Xml.Dom;
    using Windows.UI.Notifications;
    using NotificationsExtensions;
    using NotificationsExtensions.Toasts;
    using ShareClass.ViewModel.StartGroup;

    public sealed class BackgroundLockChanger : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        private StartViewModel _startVm;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //Get the deferral object from task instance.
            //Deferral used to run background task 
            _deferral = taskInstance.GetDeferral();
            //https://msdn.microsoft.com/en-us/library/windows/apps/mt299100.aspx?cs-save-lang=1&cs-lang=csharp#code-snippet-3

            //insert code here to do asynchronous method

            _startVm = new StartViewModel();

            bool success = await _startVm.ChangeCurrentBackgroundTask();

            if (success)
            {
                //SendToast();
            }

            _deferral.Complete();
        }

        public void SendToast()
        {
            //https://blogs.msdn.microsoft.com/tiles_and_toasts/2016/05/23/notificationsextensions-updated-for-anniversary-update-of-windows-10/
            ToastContent content = new ToastContent
            {
                Launch = "TuanTran",

                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = "Task run from background. Time: " + DateTime.Now
                            }
                        }
                    }
                },

                Audio = new ToastAudio
                {
                    Src = new Uri("ms-winsoundevent:Notification.IM")
                }
            };

            XmlDocument doc = content.GetXml();

            // Generate WinRT notification
            var toast = new ToastNotification(doc)
            {
                ExpirationTime = DateTime.Now.AddDays(1),
                Tag = "1",
                Group = "database"
            };

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}