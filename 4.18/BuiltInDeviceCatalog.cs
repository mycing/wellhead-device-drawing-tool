using System.Drawing;
using _4._18.Properties;

namespace _4._18
{
    internal static class BuiltInDeviceCatalog
    {
        // 與 Form1 原有索引保持一致：_useUncoloredDevices == false 時使用這組預覽圖。
        private static readonly Image[] PreviewImagesColoredMode =
        {
            Resource.转盘面,
            Resource.喇叭口,
            Resource.密封盘根升高短节,
            Resource.旋转控制头,
            Resource.临时井口头,
            Resource.万能防喷器,
            Resource.闸板防喷器,
            Resource.双闸板防喷器,
            Resource.钻井四通,
            Resource.油管四通,
            Resource.油管四通,
            Resource.法兰,
            Resource.法兰,
            Resource.升高立管,
            Resource.套管头,
            Resource.井口平台,
            Resource.分流器,
            Resource.单层套管,
            Resource.双层套管,
            Resource.三层套管,
            Resource.單筒雙井,
            Resource.隔水導管,
            Resource.節流壓井管匯
        };

        // 與 Form1 原有索引保持一致：_useUncoloredDevices == true 時使用這組預覽圖 + SVG。
        private static readonly Image[] PreviewImagesUncoloredMode =
        {
            Resource.轉盤面,
            Resource.喇叭口1,
            Resource.密封盤根升高短節,
            Resource.旋轉控制頭,
            Resource.臨時京口頭,
            Resource.萬能,
            Resource.閘板防噴器,
            Resource.雙閘版防噴器,
            Resource.鑽井四通,
            Resource.鑽井四通,
            Resource.鑽井四通,
            Resource.法蘭,
            Resource.法蘭,
            Resource.升高1,
            Resource.套管頭,
            Resource.井口平臺,
            Resource.分流器11,
            Resource.單層套管,
            Resource.雙層套管,
            Resource.三層套管,
            Resource.單筒雙井,
            Resource.隔水導管,
            Resource.節流壓井管匯
        };

        private static readonly byte[][] SvgByIndex =
        {
            Resource.转盘面1,
            Resource.喇叭口2,
            Resource.密封盘根升高短节1,
            Resource.精细控压旋转控制头,
            Resource.临时井口头1,
            Resource.环形防喷器,
            Resource.闸板防喷器1,
            Resource.双闸板防喷器1,
            Resource.钻井四通1,
            Resource.钻井四通1,
            Resource.钻井四通1,
            Resource.变径法兰,
            Resource.变径法兰,
            Resource.升高立管1,
            Resource.套管头1,
            Resource.井口平台1,
            Resource.分流器2,
            Resource.单层套管1,
            Resource.双层套管1,
            Resource.三层套管1,
            Resource.單筒雙井1,
            Resource.隔水導管1,
            Resource.变径法兰
        };

        public static bool IsBuiltInIndex(int index)
        {
            return index >= 0 && index < LocalizationManager.BuiltInDeviceCount;
        }

        public static Image GetPreviewImage(int index, bool useUncoloredDevices)
        {
            if (!IsBuiltInIndex(index))
            {
                return null;
            }

            return useUncoloredDevices
                ? PreviewImagesUncoloredMode[index]
                : PreviewImagesColoredMode[index];
        }

        public static byte[] GetSvgData(int index)
        {
            if (!IsBuiltInIndex(index))
            {
                return null;
            }
            return SvgByIndex[index];
        }
    }
}
