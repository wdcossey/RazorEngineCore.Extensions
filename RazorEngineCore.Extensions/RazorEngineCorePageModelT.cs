namespace RazorEngineCore
{
    public abstract class RazorEngineCorePageModel<T> : RazorEngineCorePageModel, IRazorEngineTemplate
    {
        public new T Model { get; set; }
    }
}