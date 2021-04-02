namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{

		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder sb = new StringBuilder();

			var gamesDtos = JsonConvert
				.DeserializeObject<IEnumerable<ImportGamesDto>>(jsonString);

            foreach (var gameDto in gamesDtos)
            {
                if (!IsValid(gameDto) ||
					gameDto.Tags.Count() == 0)
                {
					sb
						.AppendLine("Invalid Data");
					continue;
                }

				//Another way of validating the genre
				//var genre = context.Genres.FirstOrDefault(g => g.Name == gameDto.Genre);

				//if (genre == null)
				//{
				//	genre = new Genre { Name = gameDto.Genre };
				//}

				var genre = context.Genres.FirstOrDefault(g => g.Name == gameDto.Genre)
					?? new Genre { Name = gameDto.Genre };

				var developer = context.Developers.FirstOrDefault(d => d.Name == gameDto.Developer)
					?? new Developer { Name = gameDto.Developer };

				var game = new Game
				{
					Name = gameDto.Name,
					Price = gameDto.Price,
					ReleaseDate = gameDto.ReleaseDate.Value,
					Genre = genre,
					Developer = developer
				};

                foreach (var gameTag in gameDto.Tags)
                {
					var tag = context.Tags.FirstOrDefault(t => t.Name == gameTag)
						?? new Tag { Name = gameTag };
					game.GameTags.Add(new GameTag { Tag = tag });
                }

				context.Games.Add(game);
				context.SaveChanges();
				sb
					.AppendLine($"Added {gameDto.Name} ({gameDto.Genre}) with {gameDto.Tags.Count()} tags");
			}

			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			StringBuilder sb = new StringBuilder();

			var usersDto = JsonConvert
				.DeserializeObject<IEnumerable<ImportUsersCardsDto>>(jsonString);

            foreach (var userDto in usersDto)
            {
                if (!IsValid(userDto) ||
					!userDto.Cards.All(IsValid))
                {
					sb
						.AppendLine("Invalid Data");
					continue;
                }

				var user = new User
				{
					FullName = userDto.FullName,
					Username = userDto.Username,
					Email = userDto.Email,
					Age = userDto.Age,
					Cards = userDto.Cards.Select(c => new Card
					{
						Number = c.Number,
						Cvc = c.CVC,
						Type = c.Type.Value
					})
					.ToList()
				};

				context.Users.Add(user);
				sb
					.AppendLine($"Imported {userDto.Username} with {userDto.Cards.Count()} cards");
            }

			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var sb = new StringBuilder();

			var xmlSerializer = new XmlSerializer(
				typeof(ImportPurchasesDto[]),
				new XmlRootAttribute("Purchases"));
			var purchases =
				(ImportPurchasesDto[])xmlSerializer.Deserialize(
					new StringReader(xmlString));

            foreach (var currentPurchase in purchases)
            {
                if (!IsValid(currentPurchase))
                {
					sb
						.AppendLine("Invalid Data");
					continue;
                }

				bool parsedDate = DateTime.TryParseExact(
					currentPurchase.Date,
					"dd/MM/yyyy HH:mm",
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var date);

                if (!parsedDate)
                {
					sb
						.AppendLine("Invalid Data");
					continue;
				}

				var purchase = new Purchase
				{
					Type = currentPurchase.Type.Value,
					ProductKey = currentPurchase.Key,
					Card = context.Cards.FirstOrDefault(x => x.Number == currentPurchase.Card),
					Date = date,
					Game = context.Games.FirstOrDefault(x => x.Name == currentPurchase.GameName)
				};
				context.Purchases.Add(purchase);

				var userName = context.Users
					.Where(x => x.Id == purchase.Card.UserId)
					.Select(x => x.Username)
					.FirstOrDefault();

				sb
					.AppendLine($"Imported {currentPurchase.GameName} for {userName}");
            }
			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}