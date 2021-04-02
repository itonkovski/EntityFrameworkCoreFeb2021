namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			var genres = context
				.Genres
				.ToList()
				.Where(g => genreNames.Contains(g.Name))
				.Select(g => new
				{
					Id = g.Id,
					Genre = g.Name,
					Games = g.Games.Select(game => new
					{
						Id = game.Id,
						Title = game.Name,
						Developer = game.Developer.Name,
						Tags = string.Join(", ", game.GameTags.Select(gt => gt.Tag.Name)),
						Players = game.Purchases.Count()
					})
					.Where(game => game.Players > 0)
					.OrderByDescending(game => game.Players)
					.ThenBy(game => game.Id),
					TotalPlayers = g.Games.Sum(g => g.Purchases.Count()),
				})
				.OrderByDescending(g => g.TotalPlayers)
				.ThenBy(g => g.Id);

			return JsonConvert.SerializeObject(genres, Formatting.Indented);
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
			var users = context
				.Users
				.ToList()
				.Where(u => u.Cards.Any(c => c.Purchases.Any(p => p.Type.ToString() == storeType)))
				.Select(u => new ExportUserDto
				{
					Username = u.Username,
					TotalSpent = u.Cards.Sum(c => c.Purchases.Where(p => p.Type.ToString() == storeType)
							  .Sum(p => p.Game.Price)),
					Purchases = u.Cards.SelectMany(c => c.Purchases)
					.Where(p => p.Type.ToString() == storeType)
					.Select(p => new ExportPurchaseDto
                    {
						Card = p.Card.Number,
						Cvc = p.Card.Cvc,
						Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
						Game = new ExportGameDto
                        {
							Title = p.Game.Name,
							Genre = p.Game.Genre.Name,
							Price = p.Game.Price
                        }
                    })
					.OrderBy(u => u.Date)
					.ToArray()
				})
				.OrderByDescending(u => u.TotalSpent)
				.ThenBy(u => u.Username).ToArray();

			XmlSerializer xmlSerializer =
				new XmlSerializer(typeof(ExportUserDto[]),
					new XmlRootAttribute("Users"));
			var sw = new StringWriter();
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");
			xmlSerializer.Serialize(sw, users, ns);
			return sw.ToString();
		}
	}
}