using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyFx.ObjectModel
{

    public abstract class FxClass
    {
        [JsonConstructor]
        public FxClass()
        {
            TypeName = this.GetType().Name;
            ExtensionProperties = new Dictionary<string, object>();
        }

//Question:  Should I add a Dictionary<string, (string, object)> here for extension properties?  THis will give us
//  a place to put "child class" properties in our serialization when they don't fit onto a base class, or perhaps
// when the schema of a service-local model idiom doesn't include thoe aspects but the must be retained.

        protected FxClass(string? instanceJson = null):this()
        {
            
            InstanceJson = instanceJson;
        }

        public string TypeName { get; protected set; }

        [JsonIgnore]
        public string? InstanceJson{get; private set;}

        [JsonExtensionData]
        public Dictionary<string, object> ExtensionProperties {get; set;}

        public string AsJson()
        {
            string json = string.Empty;

            json = JsonSerializer.Serialize(this, this.GetType());
            
            InstanceJson = json;
            return json;
        }

        public static T? Rebuild<T>(string json) where T:FxClass
        {
            T? rebuilt = JsonSerializer.Deserialize<T>(json);
            return rebuilt;
        }
    }
}