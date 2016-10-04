using System;
using Windows.Storage;

namespace ShareClass.Utilities.Helpers
{
    public class SettingsHelper
    {
        public static void SetSetting<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public static T GetSetting<T>(string key)
        {
            object data;
            ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out data);
            if (data == null)
            {
                data = default(T);
            }

            return (T) data;
        }
    }

    public enum SettingKey
    {
        SaveMode,
        SavePath,
        SaveToken,
        DatabaseVersion,
        ImageService,
        OldImageService,
        BingLanguage,
        IsDisplayWeather,
        IsFixedLocation,
        UserLocation,
        IsFahrenheit,
        IsDisplayQuote,
        IsOfflineQuote,
        IsDisplayNote,
        Note,
        OfflineQuote,
        BingIsShowInfo,
        IsDisplayRss,
        IsDisplayRssDescription,
        RssLink,
        RssItemNum,
        WindowsResolution,
        DrawPosition
    }

    public class SettingManager
    {
        public static int GetSaveMode()
        {
            return SettingsHelper.GetSetting<int>(SettingKey.SaveMode.ToString());
        }

        /// <summary>
        /// Set save mode
        /// 1: Save in the same folder
        /// 2: Choose where to save
        /// 3: Save in specific folder
        /// </summary>
        /// <param name="saveMode">save mode</param>
        /// <param name="path">path to specific folder. Apply for 3</param>
        /// <param name="token">token to access later. Apply for 3</param>
        public static void SetSaveMode(int saveMode, string path = null, string token = null)
        {
            SettingsHelper.SetSetting(SettingKey.SaveMode.ToString(), saveMode);
            SettingsHelper.SetSetting(SettingKey.SavePath.ToString(), path);
            SettingsHelper.SetSetting(SettingKey.SaveToken.ToString(), token);
        }

        public static string GetSavePath()
        {
            return SettingsHelper.GetSetting<string>(SettingKey.SavePath.ToString());
        }

        public static string GetSaveToken()
        {
            return SettingsHelper.GetSetting<string>(SettingKey.SaveToken.ToString());
        }

        #region Image Service

        public static void SetImageService(int service)
        {
            SettingsHelper.SetSetting(SettingKey.ImageService.ToString(), service);
        }

        public static int GetImageService()
        {
            return SettingsHelper.GetSetting<int>(SettingKey.ImageService.ToString());
        }

        #endregion

        #region Bing

        public static void BingSetLanguage(string bingLanguageCode)
        {
            SettingsHelper.SetSetting(SettingKey.BingLanguage.ToString(), bingLanguageCode);
        }

        public static string BingGetLanguage()
        {
            return SettingsHelper.GetSetting<string>(SettingKey.BingLanguage.ToString());
        }

        public static void BingSetShowInfo(bool isShow)
        {
            SettingsHelper.SetSetting(SettingKey.BingIsShowInfo.ToString(), isShow);
        }

        public static bool BingGetShowInfo()
        {
            return SettingsHelper.GetSetting<bool>(SettingKey.BingIsShowInfo.ToString());
        }

        #endregion

        #region Offline Quote

        public static void SetOfflineQuote(string quote)
        {
            SettingsHelper.SetSetting(SettingKey.OfflineQuote.ToString(), quote);
        }

        public static string GetOfflineQuote()
        {
            return SettingsHelper.GetSetting<string>(SettingKey.OfflineQuote.ToString());
        }

        #endregion

        #region Note

        public static void SetNote(string note)
        {
            SettingsHelper.SetSetting(SettingKey.Note.ToString(), note);
        }

        public static string GetNote()
        {
            return SettingsHelper.GetSetting<string>(SettingKey.Note.ToString());
        }

        #endregion

        #region RSS

        public static Tuple<bool, bool> GetIsDisplayRss()
        {
            bool isDisplayRss = SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayRss.ToString());
            bool isDisplayRssDescription = SettingsHelper.GetSetting<bool>(SettingKey.IsDisplayRssDescription.ToString());

            Tuple<bool, bool> result = new Tuple<bool, bool>(isDisplayRss, isDisplayRssDescription);

            return result;
        }

        public static void SetRssLink(string link)
        {
            SettingsHelper.SetSetting(SettingKey.RssLink.ToString(), link);
        }

        public static string GetRssLink()
        {
            return SettingsHelper.GetSetting<string>(SettingKey.RssLink.ToString());
        }

        public static void SetRssItemNum(int num)
        {
            SettingsHelper.SetSetting(SettingKey.RssItemNum.ToString(), num);
        }

        public static int GetRssItemNum()
        {
            int result = SettingsHelper.GetSetting<int>(SettingKey.RssItemNum.ToString());

            return result == 0 ? 3 : result;
        }

        #endregion

        #region Windows Size

        public static void SetWindowResolution(Windows.Foundation.Size size)
        {
            SettingsHelper.SetSetting(SettingKey.WindowsResolution.ToString(), size);
        }

        public static Windows.Foundation.Size GetWindowsResolution()
        {
            return SettingsHelper.GetSetting<Windows.Foundation.Size>(SettingKey.WindowsResolution.ToString());
        }

        #endregion

        #region User's Draw Position

        public static void SetDrawPosition(string drawPosition)
        {
            SettingsHelper.SetSetting(SettingKey.DrawPosition.ToString(), drawPosition);
        }

        public static string GetDrawPosition()
        {
            string result = SettingsHelper.GetSetting<string>(SettingKey.DrawPosition.ToString());
            if (result == null)
            {
                var size = GetWindowsResolution();
                var drawPosition = "R-30||WN-30|Q-" + size.Height + "|";
                SetDrawPosition(drawPosition);
                result = drawPosition;
            }
            else
            {
                var size = GetWindowsResolution();
                var area = result.Split('|');
                for (var i = 0; i < area.Length; i++)
                {
                    if ((i >= 3) && (area[i] != ""))
                    {
                        var strArr = area[i].Split('-');
                        var strHeight = size.Height.ToString();
                        if (strArr[1] != strHeight)
                        {
                            area[i] = string.Format("{0}-{1}", strArr[0], strHeight);
                        }
                    }
                }
                var newDrawPosition = string.Format("{0}|{1}|{2}|{3}|{4}", area[0], area[1], area[2], area[3], area[4]);
                SetDrawPosition(newDrawPosition);
            }         
            return result;
        }

        #endregion

        #region Weather Location

        public static void SetUserLocation(string address)
        {
            SettingsHelper.SetSetting(SettingKey.UserLocation.ToString(), address);    
        }

        public static string GetUserLocation()
        {
            return SettingsHelper.GetSetting<string>(SettingKey.UserLocation.ToString());
        }

        #endregion

    }
}
