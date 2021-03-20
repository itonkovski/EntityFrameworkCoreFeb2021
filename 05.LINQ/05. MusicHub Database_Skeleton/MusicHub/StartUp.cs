namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here

            //02.ExportAlbumsInfo
            //Console.WriteLine(ExportAlbumsInfo(context, 9));

            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumsInfo = context
                .Producers
                .FirstOrDefault(x => x.Id == producerId)
                .Albums
                .Select(x => new
                {
                    albName = x.Name,
                    releaseDate = x.ReleaseDate,
                    prodName = x.Producer.Name,
                    albSongs = x.Songs.Select(s => new
                    {
                        songName = s.Name,
                        songPrice = s.Price,
                        songWriterName = s.Writer.Name
                    })
                    .OrderByDescending(s => s.songName)
                    .ThenBy(s => s.songWriterName)
                    .ToList(),
                    albumPrice = x.Price
                })
                .OrderByDescending(x => x.albumPrice)
                .ToList();

            var sb = new StringBuilder();

            foreach (var album in albumsInfo)
            {
                sb
                    .AppendLine($"-AlbumName: {album.albName}")
                    .AppendLine($"-ReleaseDate: {album.releaseDate.ToString("MM/dd/yyyy")}")
                    .AppendLine($"-ProducerName: {album.prodName}")
                    .AppendLine($"-Songs:");

                int songCounter = 1;

                foreach (var song in album.albSongs)
                {
                    sb
                        .AppendLine($"---#{songCounter++}")
                        .AppendLine($"---SongName: {song.songName}")
                        .AppendLine($"---Price: {song.songPrice:F2}")
                        .AppendLine($"---Writer: {song.songWriterName}");
                }

                sb
                    .AppendLine($"-AlbumPrice: {album.albumPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songsExport = context
                .Songs
                .AsEnumerable()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                {
                    songName = x.Name,
                    writerName = x.Writer.Name,
                    perfFullName = x.SongPerformers
                            .Select(x => x.Performer.FirstName + " " + x.Performer.LastName)
                            .FirstOrDefault(),
                    albumProducer = x.Album.Producer.Name,
                    duration = x.Duration
                })
                .OrderBy(x => x.songName)
                .ThenBy(x => x.writerName)
                .ThenBy(x => x.perfFullName)
                .ToList();

            var sb = new StringBuilder();

            int songCounter = 1;

            foreach (var song in songsExport)
            {
                sb
                    .AppendLine($"-Song #{songCounter++}")
                    .AppendLine($"---SongName: {song.songName}")
                    .AppendLine($"---Writer: {song.writerName}")
                    .AppendLine($"---Performer: {song.perfFullName}")
                    .AppendLine($"---AlbumProducer: {song.albumProducer}")
                    .AppendLine($"---Duration: {song.duration:c}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
