using System.Threading.Tasks;

namespace SeattleTouristAttractions.Data
{
    public class PlacesService
    {
        public Task<Place[]> GetPlacesAsync()
        {
            return Task.FromResult(new[]{
                new Place{
                    Id= 1,
                    Title= "Space Needle",
                    Description= "The Space Needle is an observation tower in Seattle.",
                    WikipediaUrl= "https://en.wikipedia.org/wiki/Space_Needle"
                },
                new Place{
                    Id= 2,
                    Title= "Lake Union Park",
                    Description= "Lake Union Park is a 12-acre (4.9 ha) park located at the south end of Lake Union in Seattle.",
                    WikipediaUrl= "https://en.wikipedia.org/wiki/Lake_Union_Park"
                },
                new Place{
                    Id= 3,
                    Title= "Museum of Pop Culture",
                    Description= "The Museum of Pop Culture, or MoPOP (previously called EMP Museum) is a nonprofit museum dedicated to contemporary popular culture.",
                    WikipediaUrl= "https://en.wikipedia.org/wiki/Museum_of_Pop_Culture"
                },
            });
        }
    }
}
