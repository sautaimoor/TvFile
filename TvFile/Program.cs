using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;


namespace VideoLinkGrabber
{
    class Program
    {
        //static async Task Main(string[] args)
        //{
        //    string baseUrl = "";
        //    string contentType = GetUserContentType();
        //    if (contentType == "Movies")
        //    {
        //        baseUrl = "http://www.dmasti.pk/movies/";
        //    }
        //    else if (contentType == "KidsMovies")
        //    {
        //        baseUrl = "http://www.dmasti.pk/view/kid/";
        //    }
        //    else if (contentType == "TVShows")
        //    {
        //        baseUrl = "http://www.dmasti.pk/view/tvshow/";
        //    }

        //    int startId = 27295;
        //    int endId = 100;
        //    string date = DateTime.Now.ToString("yyyy-MM-dd");
        //    string logFile = $"{contentType}_visited_links.log";
        //    string errorLogFile = "error_log.txt";
        //    string[] videoExtensions = { ".mp4", ".mkv", ".avi", ".mov", ".flv" }; // Updated list of video extensions

        //    // Load the visited links log
        //    HashSet<string> visitedLinks = new HashSet<string>();
        //    if (File.Exists(logFile))
        //    {
        //        var loggedLinks = File.ReadAllLines(logFile);
        //        foreach (var link in loggedLinks)
        //        {
        //            visitedLinks.Add(link);
        //        }
        //    }

        //    using (HttpClient client = new HttpClient())
        //    {
        //        List<string> videoLinks = new List<string>();

        //        for (int id = startId; id >= endId; id--)
        //        {
        //            string url = $"{baseUrl}{id}";
        //            if (visitedLinks.Contains(url))
        //            {
        //                Console.WriteLine($"Skipping already visited URL: {url}");
        //                continue;
        //            }

        //            try
        //            {
        //                HttpResponseMessage response = await client.GetAsync(url);
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    string responseBody = await response.Content.ReadAsStringAsync();
        //                    HtmlDocument doc = new HtmlDocument();
        //                    doc.LoadHtml(responseBody);

        //                    ProcessHtml(responseBody, contentType); // or "KidsMovies", "TVShows" based on selection

        //                    string videoLink = null;
        //                    var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
        //                    if (linkNodes != null)
        //                    {
        //                        foreach (var linkNode in linkNodes)
        //                        {
        //                            string href = linkNode.GetAttributeValue("href", string.Empty);
        //                            if (!string.IsNullOrEmpty(href) && Array.Exists(videoExtensions, ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
        //                            {
        //                                videoLink = href.StartsWith("http") ? href : new Uri(new Uri(url), href).ToString();
        //                                break;
        //                            }
        //                        }
        //                    }

        //                    if (videoLink != null)
        //                    {
        //                        if (contentType == "TVShows")
        //                        {
        //                            string title = ReplaceUnderscoresWithSpaces(GetCleanTitle(doc));
        //                            string season = ReplaceUnderscoresWithSpaces(GetSeason(doc)); 
        //                            string episode = ReplaceUnderscoresWithSpaces(GetEpisode(doc)); 

        //                            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(season) && !string.IsNullOrEmpty(episode))
        //                            {
        //                                string folderPath = Path.Combine(contentType, title, season);
        //                                string fileName = $"{episode}.strm";
        //                                SaveStreamFile(folderPath, fileName, videoLink, logFile, url);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            string country = GetCountry(doc);
        //                            int rating = GetRating(doc);

        //                            if (rating > 0)
        //                            {
        //                                string ratingFolder = Path.Combine(contentType, rating.ToString());
        //                                string regionFolder = GetRegionFolder(ratingFolder, country);
        //                                string fileName = $"{Path.GetFileNameWithoutExtension(videoLink)}.strm";

        //                                SaveStreamFile(regionFolder, fileName, videoLink, logFile, url);
        //                            }
        //                        }

        //                        videoLinks.Add(videoLink);
        //                    }
        //                }
        //            }
        //            catch (HttpRequestException e)
        //            {
        //                LogError(errorLogFile, $"Request error for URL {url}: {e.Message}");
        //            }
        //        }

        //        try
        //        {
        //            string outputFile = $"{contentType}_links_{date}.txt";
        //            File.WriteAllLines(outputFile, videoLinks);
        //            Console.WriteLine($"All video links have been saved to {outputFile}");
        //        }
        //        catch (Exception ex)
        //        {
        //            LogError(errorLogFile, $"Error writing output file: {ex.Message}");
        //        }
        //    }
        //}

        public static async Task Main(string[] args)
        {
            int startId = 30000;
            int endId = 100;
            string baseUrl = "";
            string contentType = GetUserContentType();
            if (contentType == "Movies")
            {
                baseUrl = "http://www.dmasti.pk/movies/";
            }
            else if (contentType == "KidsMovies")
            {
                startId = 10000;
                baseUrl = "http://www.dmasti.pk/view/kid/";
            }
            else if (contentType == "TVShows")
            {
                baseUrl = "http://www.dmasti.pk/view/tvshow/";
            }

            
            int totalIds = startId - endId + 1; // Total number of iterations

            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string logFile = $"{contentType}_visited_links.log";
            string errorLogFile = "error_log.txt";
            string[] videoExtensions = { ".mp4", ".mkv", ".avi", ".mov", ".flv" };

            // Load the visited links log
            HashSet<string> visitedLinks = new HashSet<string>();
            if (File.Exists(logFile))
            {
                var loggedLinks = File.ReadAllLines(logFile);
                foreach (var link in loggedLinks)
                {
                    visitedLinks.Add(link);
                }
            }

            using (HttpClient client = new HttpClient())
            {
                List<string> videoLinks = new List<string>();

                for (int id = startId; id >= endId; id--)
                {
                    int currentIteration = startId - id + 1;
                    double percentage = (currentIteration * 100.0) / totalIds;

                    Console.WriteLine($"Processing ID: {id}, Progress: {percentage:F2}%");

                    string url = $"{baseUrl}{id}";
                    if (visitedLinks.Contains(url))
                    {
                        Console.WriteLine($"Skipping already visited URL: {url}");
                        continue;
                    }

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(responseBody);

                            //// Process HTML content (Movie, TVShows, KidsMovies)
                            //ProcessHtml(responseBody, contentType);

                            string videoLink = null;
                            var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
                            if (linkNodes != null)
                            {
                                foreach (var linkNode in linkNodes)
                                {
                                    string href = linkNode.GetAttributeValue("href", string.Empty);
                                    if (!string.IsNullOrEmpty(href) && Array.Exists(videoExtensions, ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        videoLink = href.StartsWith("http") ? href : new Uri(new Uri(url), href).ToString();
                                        break;
                                    }
                                }
                            }

                            string fileNameText = Path.GetFileNameWithoutExtension(videoLink);
                            // create new string concat title with poster and fanart
                            //string moviePoster = fileNameText + "_poster.jpg";
                            //string movieFanart = fileNameText + "_fanart.jpg";
                            string strmPath="";
                            string fileTitle = "";

                            if (videoLink != null)
                            {
                                if (contentType == "TVShows")
                                {
                                    string title = ReplaceUnderscoresWithSpaces(GetCleanTitle(doc));
                                    string season = ReplaceUnderscoresWithSpaces(GetSeason(doc));
                                    string episode = ReplaceUnderscoresWithSpaces(GetEpisode(doc));
                                    title = RemoveInvalidCharacters(title);

                                    if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(season) && !string.IsNullOrEmpty(episode))
                                    {
                                        fileTitle = title;
                                        string folderPath = Path.Combine(contentType, title, season);

                                        strmPath = folderPath;

                                        // Download and save poster and fanart in the season folder
                                        DownloadImage(GetPosterUrl(doc), Path.Combine(folderPath, "poster.jpg"));
                                        DownloadImage(GetFanartUrl(doc), Path.Combine(folderPath, "fanart.jpg"));

                                        string fileName = $"{episode}.strm";
                                        fileName = RemoveInvalidCharacters(fileName);
                                        SaveStreamFile(folderPath, fileName, videoLink, logFile, url);
                                    }
                                }
                                else // Movies or KidsMovies
                                {
                                    string country = GetCountry(doc);
                                    int rating = GetRating(doc);
                                    
                                    if (rating==0)
                                    {
                                        // comments on console in red that rating zero found
                                        Console.WriteLine($"Rating is zero for {url}");
                                    }
                                    if (rating > -1)
                                    {                                        
                                        string ratingFolder = Path.Combine(contentType, rating.ToString());
                                        string regionFolder = GetRegionFolder(ratingFolder, country);

                                        strmPath = regionFolder;

                                        // Download and save poster and fanart in the movie folder
                                        //DownloadImage(GetPosterUrl(doc), Path.Combine(regionFolder, moviePoster));
                                        //DownloadImage(GetFanartUrl(doc), Path.Combine(regionFolder, movieFanart));
                                        fileTitle = Path.GetFileNameWithoutExtension(videoLink);
                                        string fileName = $"{Path.GetFileNameWithoutExtension(videoLink)}.strm";
                                        fileName = RemoveInvalidCharacters(fileName);
                                        SaveStreamFile(regionFolder, fileName, videoLink, logFile, url);
                                    }
                                }
                                // Process HTML content (Movie, TVShows, KidsMovies)
                                ProcessHtml(responseBody, contentType, strmPath, RemoveInvalidCharacters(fileTitle));

                                videoLinks.Add(videoLink);
                            }
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        LogError(errorLogFile, $"Request error for URL {url}: {e.Message}");
                    }
                }

                try
                {
                    string outputFile = $"{contentType}_links_{date}.txt";
                    File.WriteAllLines(outputFile, videoLinks);
                    Console.WriteLine($"All video links have been saved to {outputFile}");
                }
                catch (Exception ex)
                {
                    LogError(errorLogFile, $"Error writing output file: {ex.Message}");
                }
            }
        }



        static string ReplaceUnderscoresWithSpaces(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Input cannot be null");
            }

            // Remove the trailing underscore if it exists
            if (input.EndsWith("_"))
            {
                input = input.TrimEnd('_');
            }

            // Replace remaining underscores with spaces
            return input.Replace('_', ' ');
        }

        static string GetUserContentType()
        {
            Console.WriteLine("What you want to fetch? Options: Movies, KidsMovies, TVShows");
            string contentType = Console.ReadLine().Trim();
            while (contentType != "Movies" && contentType != "KidsMovies" && contentType != "TVShows")
            {
                Console.WriteLine("Invalid option. Please enter one of the following: Movies, KidsMovies, TVShows");
                contentType = Console.ReadLine().Trim();
            }
            return contentType;
        }

        static string GetCountry(HtmlDocument doc)
        {
            var countryNode = doc.DocumentNode.SelectSingleNode("//div[contains(text(),'Country:')]/following-sibling::div");
            return countryNode != null ? CleanString(countryNode.InnerText.Trim()) : "Others";
        }

        static int GetRating(HtmlDocument doc)
        {
            var ratingNode = doc.DocumentNode.SelectSingleNode("//div[@class='post-ratings']//span[contains(text(),'IMDb')]");
            if (ratingNode != null)
            {
                string ratingText = ratingNode.ParentNode.InnerText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (double.TryParse(ratingText, out double rating))
                {
                    return (int)Math.Floor(rating);
                }
            }
            return GetRating_IMDb(doc);            
        }
        static int GetRating_IMDb(HtmlDocument doc)
        {
            var ratingNode = doc.DocumentNode.SelectSingleNode("//div[@class='post-ratings']//span[contains(text(),'IMDb')]");
            if (ratingNode != null)
            {
                // Extract the text content of the parent node
                string ratingText = ratingNode.ParentNode.InnerText;

                // Find the position of "IMDb" and extract the rating that follows
                int imdbIndex = ratingText.IndexOf("IMDb");
                if (imdbIndex != -1)
                {
                    string ratingSubstring = ratingText.Substring(imdbIndex + "IMDb".Length).Trim();
                    string[] parts = ratingSubstring.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0 && double.TryParse(parts[0], out double rating))
                    {
                        return (int)Math.Floor(rating);
                    }
                }
            }
            return 0;
        }

        static string GetRegionFolder(string ratingFolder, string country)
        {
            string region = country switch
            {
                "United States" => "United States",
                "India" => "India",
                "China" => "China",
                "Pakistan" => "Pakistan",
                _ => "Others"
            };
            return Path.Combine(ratingFolder, region);
        }

        static string GetCleanTitle(HtmlDocument doc)
        {
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='page_title event']");
            if (titleNode != null)
            {
                string fullTitle = titleNode.InnerText.Trim();
                string title = Regex.Match(fullTitle, @"^(.*?):").Groups[1].Value;
                return CleanString(title); 
            }
            return null;
        }
        /*Updating information*/
        public static string GetTitle(HtmlDocument doc)
        {
            // Find the div with class "page_title event"
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='page_title event']");

            // Check if the title node exists
            if (titleNode != null)
            {
                // Extract the inner text
                string fullTitle = titleNode.InnerText.Trim();

                // Extract the title part before the year (assuming the year is in parentheses)
                int yearIndex = fullTitle.LastIndexOf('(');
                if (yearIndex > 0)
                {
                    // Extract the title by removing the year part
                    string titleWithYear = fullTitle.Substring(0, yearIndex).Trim();

                    // Split on " : " to get the main title
                    string[] titleParts = titleWithYear.Split(new[] { " : " }, StringSplitOptions.None);
                    string title = titleParts[0].Trim();

                    return title;
                }
            }

            return null; // Return null if title is not found
        }
        public static string GetYear(HtmlDocument doc)
        {
            // Find the div with class "page_title event"
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='page_title event']");

            // Check if the title node exists
            if (titleNode != null)
            {
                // Extract the inner text
                string fullTitle = titleNode.InnerText.Trim();

                // Extract the year part within the parentheses
                int yearStartIndex = fullTitle.LastIndexOf('(');
                int yearEndIndex = fullTitle.LastIndexOf(')');
                if (yearStartIndex > 0 && yearEndIndex > yearStartIndex)
                {
                    // Extract the year by taking the substring between the parentheses
                    string year = fullTitle.Substring(yearStartIndex + 1, yearEndIndex - yearStartIndex - 1).Trim();
                    return year;
                }
            }

            return null; // Return null if year is not found
        }
        public static string GetPlot(HtmlDocument doc)
        {
            // Find the div with class "wpb_wrapper2" containing the plot
            //var plotNode = doc.DocumentNode.SelectSingleNode("//div[@class='wpb_wrapper2']//p");
            var plotNode = doc.DocumentNode.SelectSingleNode("//div[@class='wpb_wrapper2']/h1[text()='Plot']/following-sibling::p[1]");

            // Check if the plot node exists
            if (plotNode != null)
            {
                // Extract and return the plot text, trimming any extra spaces
                string plot = plotNode.InnerText.Trim();
                return plot;
            }

            return null; // Return null if plot is not found
        }
        public static string GetDirector(HtmlDocument doc)
        {
            // Find the div with class "info event_director" and the <a> tag inside it
            var directorNode = doc.DocumentNode.SelectSingleNode("//div[@class='info event_director']//a");

            // Check if the director node exists
            if (directorNode != null)
            {
                // Extract and return the director's name, trimming any extra spaces
                string director = directorNode.InnerText.Trim();
                return director;
            }

            return null; // Return null if director is not found
        }
        public static List<string> GetActors(HtmlDocument doc)
        {
            // Create a list to store actor names
            List<string> actors = new List<string>();

            // Find all divs with class "actor-name-block" and the <a> tag inside it
            var actorNodes = doc.DocumentNode.SelectNodes("//div[@class='actor-name-block']//a");

            // Check if actor nodes are found
            if (actorNodes != null)
            {
                // Loop through all actor nodes and extract the actor names
                foreach (var actorNode in actorNodes)
                {
                    string actor = actorNode.InnerText.Trim();
                    actors.Add(actor);
                }
            }

            return actors; // Return the list of actors
        }
        public static string GetGenres(HtmlDocument doc)
        {
            // Create a list to store genre names
            List<string> genres = new List<string>();

            // Find all <a> tags inside the div with class "info event_category"
            var genreNodes = doc.DocumentNode.SelectNodes("//div[@class='info event_category']//a");

            // Check if genre nodes are found
            if (genreNodes != null)
            {
                // Loop through each genre node and extract the text (genre name)
                foreach (var genreNode in genreNodes)
                {
                    string genre = genreNode.InnerText.Trim().TrimEnd(',');
                    genres.Add(genre);
                }
            }

            // Return the genres as a comma-separated string
            return string.Join(", ", genres);
        }
        public static string GetPosterUrl(HtmlDocument doc)
        {
            // Find the div with class "image_wrapper2 event" and the <img> tag inside it
            var imgNode = doc.DocumentNode.SelectSingleNode("//div[@class='image_wrapper2 event']//img");
            if (imgNode == null) { imgNode = doc.DocumentNode.SelectSingleNode("//div[@class='image_wrapper2 event shadows']//img"); }
            // Check if the image node exists
            if (imgNode != null)
            {
                
                // Extract the 'src' attribute, which contains the URL of the image
                string posterUrl = imgNode.GetAttributeValue("src", "").Trim();
                return posterUrl;
            }

            return null; // Return null if the poster URL is not found
        }
        public static string GetFanartUrl(HtmlDocument doc)
        {
            // Find the meta tag with property "og:image"
            var metaNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");

            // Check if the meta node exists
            if (metaNode != null)
            {
                // Extract the 'content' attribute, which contains the URL of the fanart image
                string fanartUrl = metaNode.GetAttributeValue("content", "").Trim();
                return fanartUrl;
            }

            return GetFanartUrl_IMG(doc);
        }

        public static string GetFanartUrl_IMG(HtmlDocument doc)
        {
            // Find the node with the style attribute containing the image URL
            var node = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'columns-container')]");

            if (node != null)
            {
                // Extract the style attribute value
                var style = node.GetAttributeValue("style", string.Empty);

                // Find the URL within the style attribute
                var urlStart = style.IndexOf("url('") + 5;
                var urlEnd = style.IndexOf("')", urlStart);

                if (urlStart > 4 && urlEnd > urlStart)
                {
                    return style.Substring(urlStart, urlEnd - urlStart);
                }
            }

            return null;
        }

        public static string RemoveInvalidCharacters(string fileName)
        {
            // Define a regular expression to match invalid characters
            string invalidCharsPattern = @"[\\/:*?""<>|]";

            // Replace invalid characters with an empty string
            return ExtractTitle(Regex.Replace(fileName, invalidCharsPattern, string.Empty));
        }
        //public static string ExtractTitle(string input)
        //{
        //    // Remove the year and format (e.g., BRRip, HDRip, WEBRip)
        //    string pattern = @"-\d{4}-(BRRip|HDRip|WEBRip|CAMRip)";
        //    string cleaned = Regex.Replace(input, pattern, "");

        //    // Replace hyphens with spaces
        //    cleaned = cleaned.Replace("-", " ");
        //    cleaned = cleaned.Replace("Hindi", "Urdu");
        //    cleaned = cleaned.Replace("English", string.Empty);
        //    cleaned = cleaned.Replace("Dubbed", string.Empty);

        //    return cleaned.Trim();
        //}
        public static string ExtractTitle(string input)
        {
            // Remove the year and format (e.g., BRRip, HDRip, WEBRip)
            //string pattern = @"-\d{4}\s*-\s*(BRRip|HDRip|WEBRip|CAMRip)";
            string pattern = @"(\d{4}(BDRip|DvdRip|DVDRip|BrRip|BRRip|HDRip|WEBRip|CAMRip))|(-\d{4}-(BDRip|DvdRip|DVDRip|BrRip|BRRip|HDRip|WEBRip|CAMRip))|(-\d{4}\s*-\s*(BDRip|DvdRip|DVDRip|BrRip|BRRip|HDRip|WEBRip|CAMRip))";
            string cleaned = Regex.Replace(input, pattern, "");

            // Replace hyphens with spaces
            cleaned = cleaned.Replace("-", " ");
            cleaned = cleaned.Replace("  ", " ");
            cleaned = cleaned.Replace("Hindi", string.Empty);
            cleaned = cleaned.Replace("English", string.Empty);
            cleaned = cleaned.Replace("Dubbed", string.Empty);

            return cleaned.Trim();
        }
        public static void ProcessHtml(string responseBody, string category,string path,string filename)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseBody);

            // Call parsing functions to extract metadata
            string title = filename; // RemoveInvalidCharacters(GetTitle(doc));
            string year = GetYear(doc);
            string plot = GetPlot(doc);
            string director = GetDirector(doc);
            List<string> actors = GetActors(doc);
            string genres = GetGenres(doc);
            string posterUrl = GetPosterUrl(doc);
            string fanartUrl = GetFanartUrl(doc);

            //// Determine base folder based on category (Movies, KidsMovies, TVShows)
            //string baseFolder = Path.Combine(Directory.GetCurrentDirectory(), category, "Images");

            //// Ensure the folder exists, if not, create it
            //if (!Directory.Exists(baseFolder))
            //{
            //    Directory.CreateDirectory(baseFolder);
            //}

            // create new string concat title with poster and fanart
            string moviePoster = title + "_poster.jpg";
            string movieFanart = title + "_fanart.jpg";

            // Download poster image and save it in the Images folder
            string posterFilePath = Path.Combine(path, moviePoster);
            DownloadImage(posterUrl, posterFilePath);

            // Download fanart image and save it in the Images folder
            string fanartFilePath = Path.Combine(path, movieFanart);
            DownloadImage(fanartUrl, fanartFilePath);

            // Create the .nfo file
            string nfoFilePath = Path.Combine(path, $"{title}.nfo");
            //GenerateNfoFile(nfoFilePath, title, year, plot, director, actors, genres, posterFilePath, fanartFilePath);
            GenerateNfoFile(nfoFilePath, title, year, plot, director, actors, genres, moviePoster, movieFanart);

            Console.WriteLine($"Processed: {title} ({year})");
        }
        // Helper function to download an image from a URL
        public static void DownloadImage(string imageUrl, string savePath)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                Console.WriteLine("Image URL is empty, skipping download.");
                return;
            }

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(new Uri(imageUrl), savePath);
                    Console.WriteLine($"Downloaded image to {savePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to download image: {ex.Message}");
                }
            }
        }
        // Helper function to generate the .nfo file
        public static void GenerateNfoFile(string filePath, string title, string year, string plot, string director, List<string> actors, string genres, string posterPath, string fanartPath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"<movie>");
                writer.WriteLine($"  <title>{title}</title>");
                writer.WriteLine($"  <year>{year}</year>");
                writer.WriteLine($"  <plot>{plot}</plot>");
                writer.WriteLine($"  <director>{director}</director>");
                writer.WriteLine($"  <cast>");
                foreach (var actor in actors)
                {
                    writer.WriteLine($"    <actor>{actor}</actor>");
                }
                writer.WriteLine($"  </cast>");
                writer.WriteLine($"  <genre>{genres}</genre>");
                writer.WriteLine($"  <thumb>{posterPath}</thumb>");
                writer.WriteLine($"  <fanart>{fanartPath}</fanart>");
                writer.WriteLine($"</movie>");
            }

            Console.WriteLine($"Generated NFO file: {filePath}");
        }
        /**/
        static string GetSeason(HtmlDocument doc)
        {
            var seasonNode = doc.DocumentNode.SelectSingleNode("//span[@class='season']");
            return seasonNode != null ? CleanString(seasonNode.InnerText.Trim().Replace(" ", "_")) : null;
        }

        static string GetEpisode(HtmlDocument doc)
        {
            var episodeNode = doc.DocumentNode.SelectSingleNode("//span[@class='episode']");
            return episodeNode != null ? CleanString(episodeNode.InnerText.Trim().Replace(" ", "_")) : null;
        }

        static string CleanString(string input)
        {
            return HttpUtility.HtmlDecode(input).Replace("&nbsp;", " ").Replace(" ", "_").Replace(":", "").Replace("/", "").Replace("\\", "");
        }

        static void SaveStreamFile(string folderPath, string fileName, string videoLink, string logFile, string url)
        {
            try
            {
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(Path.Combine(folderPath, fileName), videoLink);
                Console.WriteLine($"Saved: {Path.Combine(folderPath, fileName)}");
                File.AppendAllLines(logFile, new[] { url });
            }
            catch (Exception ex)
            {
                LogError("error_log.txt", $"Error creating .strm file for {videoLink}: {ex.Message}");
            }
        }

        static void LogError(string errorLogFile, string errorMessage)
        {
            Console.WriteLine(errorMessage);
            File.AppendAllLines(errorLogFile, new[] { errorMessage });
        }
    }

    class Category
    {
        public string Name { get; }
        public string BaseUrl { get; }
        public int StartId { get; }
        public int EndId { get; }

        public Category(string name, string baseUrl, int startId, int endId)
        {
            Name = name;
            BaseUrl = baseUrl;
            StartId = startId;
            EndId = endId;
        }
    }
}
