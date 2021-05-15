namespace RazorEngineCore
{
    public class ViewKey
    {
        public string Key { get; }
        
        public string[] Segments { get; }
        
        public ViewKey(string key)
        {
            Key = key;
            Segments = key.Split('/');
        }
        
    }
}