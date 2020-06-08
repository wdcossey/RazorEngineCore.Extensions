namespace RazorEngineCore
{
    public class RazorEngineCorePageModel<T> : RazorEngineCorePageModel, IRazorEngineTemplate
    {
        public new T Model { get; set; }
    }
}