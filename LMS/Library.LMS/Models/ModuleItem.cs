using System.Text.Json.Serialization;

namespace Library.LMS.Models;

[JsonPolymorphic(
    TypeDiscriminatorPropertyName = "$itemType"
)]
[JsonDerivedType(
    typeof(ModulePage),
    "page"
)]
[JsonDerivedType(
    typeof(ModuleFile),
    "file"
)]
[JsonDerivedType(
    typeof(Assignment),
    "assignment"
)]
public abstract class ModuleItem
{
    public int Id { get; set; }

    public string? Name { get; set; }

    [JsonIgnore]
    public abstract string ItemType { get; }

    [JsonIgnore]
    public virtual string DisplayText
    {
        get
        {
            return $"{ItemType}: {Name}";
        }
    }
}