using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubForDynamoWPF.Attributes
{
    // https://www.codeproject.com/Tips/783463/Enum-With-Text-Value-in-Csharp
    public class CustomEnumAttribute : Attribute
    {
        public bool IsCustomEnum { get; set; }
        public CustomEnumAttribute(bool isCustomEnum) : this()
        {
            IsCustomEnum = isCustomEnum;
        }

        private CustomEnumAttribute()
        {
            IsCustomEnum = false;
        }
    }
    [AttributeUsage(AttributeTargets.Field)]
    class TextValueAttribute : CustomEnumAttribute
    {
        public String Value { get; set; }
        public TextValueAttribute(string textValue) : this()
        {
            Value = textValue ?? throw new NullReferenceException("Null not allowed in textValue at TextValue attribute");
        }

        private TextValueAttribute() : base(true)
        {
            Value = string.Empty;
        }
    }
}
