namespace Models
{
    public class SelectList
    {
        public string SelectText { get; set; }
        public string SelectValue { get; set; }
        public bool Selected { get; set; }
    }

    public class OriginDestinationLoction
    {
        public string SelectText { get; set; }
        public string SelectValue { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string RegionName { get; set; } 
    }
}