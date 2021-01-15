namespace SeattleTouristAttractions.Components
{
    public static class TitleExtension
    {
        public static string WithSuffix(string title, bool nosuffix = false)
        {
            return nosuffix ? title : title + " - Seattle Tourist Attractions";
        }
    }
}
