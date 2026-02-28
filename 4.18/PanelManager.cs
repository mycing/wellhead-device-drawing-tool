using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

public class PanelManager
{
    private readonly Panel _parentPanel; // 父级 Panel，用于存储面板
    private readonly Font _defaultFont; // 默认字体，用于恢复 DrawstringPanel
    private readonly List<Panel> _dynamicPanels; // 动态面板列表
    private readonly string _binaryFilePath; // 默认面板存储的二进制文件路径
    public Dictionary<string, List<PanelInfo>> _PanelInfosDictionary { get; set; }

    public PanelManager(Panel parentPanel, Font defaultFont, List<Panel> dynamicPanels, string binaryFilePath, Dictionary<string, List<PanelInfo>> PanelInfosDictionary)
    {
        _parentPanel = parentPanel ?? throw new ArgumentNullException(nameof(parentPanel));
        _defaultFont = defaultFont ?? throw new ArgumentNullException(nameof(defaultFont));
        _dynamicPanels = dynamicPanels ?? new List<Panel>();
        _binaryFilePath = binaryFilePath ?? throw new ArgumentNullException(nameof(binaryFilePath));
        _PanelInfosDictionary = PanelInfosDictionary ?? new Dictionary<string, List<PanelInfo>>();
    }

    /// <summary>
    /// 保存父面板中所有子控件的信息
    /// </summary>
    public List<PanelInfo> SaveAllPanels()
    {
        List<PanelInfo> panelInfos = new List<PanelInfo>();

        foreach (Control control in _parentPanel.Controls)
        {
            if (control is SvgDrawPicturePanel svgPanel)
            {
                panelInfos.Add(new SvgPanelInfo
                {
                    Size = svgPanel.Size,
                    Location = svgPanel.Location,
                    SvgData = svgPanel._svgData,
                });
            }
            else if (control is ImagePicturePanel imagePanel)
            {
                panelInfos.Add(new PicturePanelInfo
                {
                    Size = imagePanel.Size,
                    Location = imagePanel.Location,
                    PictureBinaryInfo = ImageToBinaryData(imagePanel._image)
                });
            }
            else if (control is DrawstringPanel drawstringPanel)
            {
                panelInfos.Add(new TextPanelInfo
                {
                    Size = drawstringPanel.Size,
                    Location = drawstringPanel.Location,
                    Text = drawstringPanel.PanelString
                });
            }
        }

        return panelInfos;
    }

    /// <summary>
    /// 根据保存的面板信息恢复所有子控件
    /// </summary>
    public void RestorePanels(List<PanelInfo> panelInfos)
    {
        if (panelInfos == null) throw new ArgumentNullException(nameof(panelInfos));

        _parentPanel.Controls.Clear();

        foreach (var panelInfo in panelInfos)
        {
            if (panelInfo is SvgPanelInfo svgPanelInfo)
            {
               
                var svgPanel = new SvgDrawPicturePanel(svgPanelInfo.Location, svgPanelInfo.SvgData, _parentPanel, null, _dynamicPanels)
                {
                    Size = svgPanelInfo.Size
                };
                _parentPanel.Controls.Add(svgPanel);
                _dynamicPanels.Add(svgPanel);
            }
            else if (panelInfo is PicturePanelInfo picturePanelInfo)
            {
                var image = LoadImageFromBinaryData(picturePanelInfo.PictureBinaryInfo);
                var imagePanel = new ImagePicturePanel(picturePanelInfo.Location, image, _parentPanel, null, _dynamicPanels)
                {
                    Size = picturePanelInfo.Size
                };
                _parentPanel.Controls.Add(imagePanel);
                _dynamicPanels.Add(imagePanel);
            }
            else if (panelInfo is TextPanelInfo textPanelInfo)
            {
                var drawstringPanel = new DrawstringPanel(textPanelInfo.Location, textPanelInfo.Text, _defaultFont)
                {
                    Size = textPanelInfo.Size
                };
                _parentPanel.Controls.Add(drawstringPanel);
                _dynamicPanels.Add(drawstringPanel);
            }
        }
    }

    /// <summary>
    /// 将图片转换为二进制数据
    /// </summary>
    private byte[] ImageToBinaryData(Image image)
    {
        if (image == null) return null;
        using (var ms = new MemoryStream())
        {
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// 从二进制数据加载图片
    /// </summary>
    private Image LoadImageFromBinaryData(byte[] imageData)
    {
        if (imageData == null) return null;
        using (var ms = new MemoryStream(imageData))
        {
            return Image.FromStream(ms);
        }
    }





    /// <summary>
    /// 保存 PanelInfosDictionary 到指定二进制文件
    /// </summary>
    public void SaveDictionaryToBinaryFile(string dictionaryFilePath)
    {
        using (FileStream stream = new FileStream(dictionaryFilePath, FileMode.Create))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, _PanelInfosDictionary);
        }
    }

    /// <summary>
    /// 從二進制文件中加載字典，如果地址文件不存在則創建
    /// </summary>
    /// <param name="dictionaryFilePath"></param>
    /// <returns></returns>
    public Dictionary<string, List<PanelInfo>> LoadDictionaryFromBinaryFile(string dictionaryFilePath)
    {
        if (File.Exists(dictionaryFilePath) && new FileInfo(dictionaryFilePath).Length > 0)
        {
            // 如果文件存在且非空，从文件中读取字典数据
            using (FileStream stream = new FileStream(dictionaryFilePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, List<PanelInfo>>)formatter.Deserialize(stream);
            }
        }
        else
        {
            // 如果文件不存在或为空，创建空字典并保存到文件
            var emptyDictionary = new Dictionary<string, List<PanelInfo>>();
            using (FileStream stream = new FileStream(dictionaryFilePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, emptyDictionary);
            }
            return emptyDictionary;
        }
    }



    /// <summary>
    /// 保存 PanelInfosDictionary 到指定 JSON 文件
    /// </summary>
    public void SaveDictionaryToJsonFile(string jsonFilePath)
    {
        try
        {
            // 将字典序列化为 JSON 字符串
            string jsonString = JsonConvert.SerializeObject(_PanelInfosDictionary, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonString); // 写入 JSON 文件
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON 保存失败: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 从 JSON 文件中加载字典，如果地址文件不存在则创建
    /// </summary>
    /// <param name="jsonFilePath"></param>
    /// <returns></returns>
    public Dictionary<string, List<PanelInfo>> LoadDictionaryFromJsonFile(string jsonFilePath)
    {
        try
        {
            // 检查文件是否存在且不为空
            if (File.Exists(jsonFilePath) && new FileInfo(jsonFilePath).Length > 0)
            {
                // 从 JSON 文件中反序列化字典
                string jsonString = File.ReadAllText(jsonFilePath);
                return JsonConvert.DeserializeObject<Dictionary<string, List<PanelInfo>>>(jsonString);
            }
            else
            {
                // 如果文件不存在或为空，创建空字典并保存到文件
                var emptyDictionary = new Dictionary<string, List<PanelInfo>>();
                SaveDictionaryToJsonFile(jsonFilePath); // 保存空字典
                return emptyDictionary;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JSON 加载失败: {ex.Message}");
            throw;
        }
    }

}





#region PanelInfo Definitions

[Serializable]
public abstract class PanelInfo
{
    public Point Location { get; set; }
    public Size Size { get; set; }
}

[Serializable]
public class SvgPanelInfo : PanelInfo
{
    public byte[] SvgData { get; set; }
}

[Serializable]
public class PicturePanelInfo : PanelInfo
{
    public byte[] PictureBinaryInfo { get; set; }
}

[Serializable]
public class TextPanelInfo : PanelInfo
{
    public string Text { get; set; }
}

#endregion
