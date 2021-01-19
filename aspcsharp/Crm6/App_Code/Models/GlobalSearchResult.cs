namespace Models
{
    public class GlobalSearchResult
    {
        public int id;
        public string name;
        public TopSearchResultInner dataObj;
    }

    public class TopSearchResultInner
    {
        public int Id;
        public string Name;
        public string Description01;
        public string Description02;
        public string DataType; 
        public string ProfilePic;
    }

}
