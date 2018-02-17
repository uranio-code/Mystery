namespace Mystery.UI
{
    public class Modal
    {
        public bool animation { get; set; } = true;
        public string templateUrl { get; set; }
        public string template { get; set; }
        public string controller { get; set; }
        public bool backdrop { get; set; } = false;
        public bool keyboard { get; set; } = false;
    }
}
