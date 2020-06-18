using System;

namespace RazorEngineCore
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public class PrecompiledTemplate : Attribute
    {
        public PrecompiledTemplate(Type templateType, string content)
        {
            
        }
    }
}