using System;
using System.Collections.ObjectModel;
using ShareClass.Model;

namespace ShareClass.Utilities.Helpers.SourceDataHelper
{
    public static class PositionHelper
    {
        public static void GetPositionItems(ref ObservableCollection<ImageSourceItem> positionItemsCollection)
        {

            ImageSourceItem i = new ImageSourceItem
            {
                Name = "Top Left",
                Number = 0
            };
            positionItemsCollection.Add(i);

            i = new ImageSourceItem
            {
                Name = "Top Mid",
                Number = 1
            };
            positionItemsCollection.Add(i);

            i = new ImageSourceItem
            {
                Name = "Top Right",
                Number = 2
            };
            positionItemsCollection.Add(i);

            i = new ImageSourceItem
            {
                Name = "Bottom Mid",
                Number = 3
            };
            positionItemsCollection.Add(i);

            i = new ImageSourceItem
            {
                Name = "Bottom Right",
                Number = 4
            };
            positionItemsCollection.Add(i);

        }

        public static int GetElementPosition(string element)
        {
            var drawPosition = SettingManager.GetDrawPosition();
            var area = drawPosition.Split('|');
            for (int i = 0; i < area.Length; i++)
            {
                if (area[i].Contains(element))
                {
                    return i;
                }
            }
            return 2;
        }

        public static void SetElementPosition(string element, int number)
        {
            var drawPosition = SettingManager.GetDrawPosition();
            var area = drawPosition.Split('|');
            for (int i = 0; i < area.Length; i++)
            {
                if (area[i].Contains(element))
                {
                    var pos = area[i].IndexOf(element, StringComparison.Ordinal);
                    area[i] = area[i].Remove(pos, 1);
                    var strArr = area[i].Split('-');
                    if (strArr[0] == "")
                    {
                        area[i] = "";
                    }
                }
            }
            if (area[number] == "")
            {
                var screenSize = SettingManager.GetWindowsResolution();
                var height = number <= 2 ? "30" : screenSize.Height.ToString();
                
                area[number] = string.Format("{0}-{1}", element,height);
            }
            else
            {
                var strArr = area[number].Split('-');
                strArr[0] += element;

                area[number] = string.Format("{0}-{1}", strArr[0], strArr[1]);
            }

            var newDrawPosition = string.Format("{0}|{1}|{2}|{3}|{4}", area[0], area[1], area[2], area[3], area[4]);
            SettingManager.SetDrawPosition(newDrawPosition);
        }
    }
}