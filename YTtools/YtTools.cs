using YouTubeSearch;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Converter;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;
using System.Collections.ObjectModel;
using NovoBot;

namespace MusicalBot.YTtools
{
    internal static class YtTools
    {
        public static async Task Download(string msg)
        {
            string path;
            
            string url = "";

            if (!msg.Contains("www.youtube"))
            {
                url = await UrlByTitle(msg);
            }
            else { url = msg; }

            YoutubeClient yt = new YoutubeClient();

            msg = msg.Replace(' ', '_');

            if (ConfigHandler.Linux)
            {
                path = $"musics/{msg}.mp3";
            }
            else {path = $@"musics\{msg}.mp3"; }

            var manifest = await yt.Videos.Streams.GetManifestAsync(url);
            var video = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            await yt.Videos.Streams.DownloadAsync(video, path);

        }
 
        private static async Task<string> UrlByTitle(string title)
        {
            VideoSearch vs = new VideoSearch();
            var videos = await vs.GetVideosPaged(title, 1);
            return videos.FirstOrDefault().getUrl();
        }
    }
}
