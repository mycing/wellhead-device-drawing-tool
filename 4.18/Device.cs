using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4._18
{
    public class Device
    {
        //默認添加的井控裝置
        private string[] defult_device = new string[] { "转盘面", "喇叭口", "密封盘根升高短节", "精细控压旋转控制头","临时井口头","环形防喷器","闸板防喷器", "双闸板防喷器","钻井四通"
        ,"油管四通","套管四通","变径法兰","变压法兰","升高立管","套管头","井口平台","分流器","单层套管","双层套管","三层套管","單筒雙井","隔水導管","節流壓井管匯"};
        public string[] Defult_device
        {
            get { return defult_device; }
            set { defult_device = value; }
        }

        //用戶自定義添加的井控裝置
        public Dictionary<string, string> custome_device = new Dictionary<string, string>();
        public Dictionary<string, string> Costumer_device
        {
            get { return custome_device; }
            set { custome_device = value; }
        }

        /// <summary>
        /// 添加一个方法来检索某个文件目录下的图片并将图片名和地址写入字典
        /// </summary>
        /// <param name="folderPath"></param>
        public void RetrieveImagesFromDirectory(string folderPath)
        {
            // 检查文件夹是否存在
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("用戶文件夾無法訪問");
                return;
            }

            // 获取文件夹中的所有图片文件
            string[] imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string file in imageFiles)
            {
                // 检查文件扩展名是否为图片格式
                if (file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".tiff", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        // 获取文件名
                        string fileName = Path.GetFileName(file);
                        // 将图片文件名和路径添加到字典中
                        if (!custome_device.ContainsKey(fileName))
                        {
                            custome_device.Add(fileName, file);
                        }
                        else
                        {
                            Console.WriteLine($"图片文件名 {fileName} 已存在于字典中。");
                        }
                    }
                    catch (Exception ex)
                    {
                        // 记录错误日志并显示错误消息
                        Console.WriteLine($"无法处理图片: {file}\n错误: {ex.Message}");
                        MessageBox.Show($"无法处理图片: {file}\n错误: {ex.Message}");
                    }
                }
            }
        }
    }
}



















