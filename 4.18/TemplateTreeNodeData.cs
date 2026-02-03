using System;
using System.Collections.Generic;

namespace _4._18
{
    /// <summary>
    /// 可序列化的模板樹節點數據類
    /// </summary>
    [Serializable]
    public class TemplateTreeNodeData
    {
        /// <summary>
        /// 節點顯示名稱
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 模板數據（資料夾為 null）
        /// </summary>
        public List<PanelInfo> TemplateData { get; set; }

        /// <summary>
        /// 子節點
        /// </summary>
        public List<TemplateTreeNodeData> Children { get; set; }

        /// <summary>
        /// 原始數據引用（用於過濾模式下的更新，不序列化）
        /// </summary>
        [NonSerialized]
        public TemplateTreeNodeData OriginalData;

        /// <summary>
        /// 是否為資料夾（沒有模板數據）
        /// </summary>
        public bool IsFolder => TemplateData == null;

        /// <summary>
        /// 是否為模板（有模板數據）
        /// </summary>
        public bool IsTemplate => TemplateData != null;

        public TemplateTreeNodeData()
        {
            Children = new List<TemplateTreeNodeData>();
        }

        public TemplateTreeNodeData(string text) : this()
        {
            Text = text;
        }

        public TemplateTreeNodeData(string text, List<PanelInfo> templateData) : this()
        {
            Text = text;
            TemplateData = templateData;
        }
    }
}
