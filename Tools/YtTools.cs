using YouTubeSearch;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
using NovoBot;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

namespace MusicalBot.YTtools
{
    internal static class YtTools
    {
        public static async Task Download(string msg)
        {
            string path;
            

            YoutubeClient yt = new YoutubeClient();
            msg = msg.Trim();
            msg = msg.Replace(' ', '_');

            if (ConfigHandler.Linux)
            {
                path = $"musics/{msg}.mp3";
            }
            else {path = $@"musics\{msg}.mp3"; }
            

            var manifest = await yt.Videos.Streams.GetManifestAsync(msg);
            var video = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            await yt.Videos.Streams.DownloadAsync(video, path);

        }
        public static async ValueTask<List<string[]>> PlaylistToQueue(string msg)
        {
            YoutubeClient yt = new YoutubeClient();
            
            var Id = PlaylistId.Parse(msg);
            var playlist = await yt.Playlists.GetVideosAsync(Id);

            var videos = new List<string[]>();

            foreach ( var video in playlist)
            {
                string[] toQueue = new string[2] {video.Id, video.Title};
                videos.Add(toQueue);
            }   

            return videos;
        }

        public static async ValueTask<string[]> VideoToQueue(string msg)
        {
            YoutubeClient yt = new YoutubeClient();
            
            if (!msg.Contains("www.youtube"))
            {
                msg = await UrlByTitle(msg);
            }
            if (msg.Contains('&'))
            {
                msg = msg.Remove(msg.IndexOf('&'));
            }
            
            msg = msg.Remove(0, msg.IndexOf('=') + 1);
            var id = msg.Trim();

            VideoId videoId = VideoId.Parse(id);
            var video = await yt.Videos.GetAsync(id);
            
            var toQueue = new string[2] {video.Id,video.Title};
            return toQueue;

        }

        private static async Task<string> UrlByTitle(string title)
        {
            VideoSearch vs = new VideoSearch();
            var videos = await vs.GetVideosPaged(title, 1);
            var video = videos.FirstOrDefault();
            
            if (video != null)
            {
                return video.getUrl();
            }
            
            return "";
        }
    }
}
