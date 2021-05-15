namespace RazorEngineCore
{
    public abstract class RazorEngineCorePageModel<T> : RazorEngineCorePageModel, IRazorEngineCorePageModel
        where T: class
    {
        public new T Model { get; set; }
    }
}