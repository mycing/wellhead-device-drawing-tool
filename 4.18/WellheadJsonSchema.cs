using Newtonsoft.Json;
using System.Collections.Generic;

namespace _4._18
{
    /// <summary>
    /// JSON 匯入的頂層結構
    /// </summary>
    public class WellheadJsonSchema
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("devices")]
        public List<WellheadDeviceEntry> Devices { get; set; }

        [JsonIgnore]
        public int SourceLine { get; set; }

        [JsonIgnore]
        public int SourceLinePosition { get; set; }
    }

    /// <summary>
    /// 單個裝置條目
    /// </summary>
    public class WellheadDeviceEntry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("flange")]
        public string Flange { get; set; }

        [JsonIgnore]
        public int SourceLine { get; set; }

        [JsonIgnore]
        public int SourceLinePosition { get; set; }
    }
}
