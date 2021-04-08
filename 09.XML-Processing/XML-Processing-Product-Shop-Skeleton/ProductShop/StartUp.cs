using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XmlHelper;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //01.ImportUsers
            //var usersXml = File.ReadAllText("./Datasets/users.xml");
            //System.Console.WriteLine(ImportUsers(context, usersXml));

            //02.ImportProducts
            //var usersXml = File.ReadAllText("./Datasets/users.xml");
            //var productsXml = File.ReadAllText("./Datasets/products.xml");
            //ImportUsers(context, usersXml);
            //System.Console.WriteLine(ImportProducts(context, productsXml));

            //03.ImportCategories
            //var usersXml = File.ReadAllText("./Datasets/users.xml");
            //var productsXml = File.ReadAllText("./Datasets/products.xml");
            //var categoriesXml = File.ReadAllText("./Datasets/categories.xml");
            //ImportUsers(context, usersXml);
            //ImportProducts(context, productsXml);
            //System.Console.WriteLine(ImportCategories(context, categoriesXml));

            //04.ImportCategoriesAndProducts
            //var usersXml = File.ReadAllText("./Datasets/users.xml");
            //var productsXml = File.ReadAllText("./Datasets/products.xml");
            //var categoriesXml = File.ReadAllText("./Datasets/categories.xml");
            //var categoriesProductsXml = File.ReadAllText("./Datasets/categories-products.xml");
            //ImportUsers(context, usersXml);
            //ImportProducts(context, productsXml);
            //ImportCategories(context, categoriesXml);
            //System.Console.WriteLine(ImportCategoryProducts(context, categoriesProductsXml));

            //05.ExportProductsInRange
            //System.Console.WriteLine(GetProductsInRange(context));

            //06.ExportSoldProducts
            //System.Console.WriteLine(GetSoldProducts(context));

            //07.ExportCategoriesByProductsCount
            //System.Console.WriteLine(GetCategoriesByProductsCount(context));

            //08.ExportUsersAndProducts
            System.Console.WriteLine(GetUsersWithProducts(context));
        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }

        //01.ImportUsers
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            //Basic way of deserializing the input
            //var xmlSerializer = new XmlSerializer(typeof(UsersInputModel[]),
            //    new XmlRootAttribute("Users"));

            //var textReader = new StringReader(inputXml);

            //var usersDto = xmlSerializer
            //    .Deserialize(textReader) as UsersInputModel[];

            //Using XmlConverter created by StoyanShopov
            const string root = "Users";

            var usersDto = XmlConverter
                .Deserializer<UsersInputModel>(inputXml, root);

            var users = usersDto
                .Select(u => new User
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Age = u.Age
            })
            .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        //02.ImportProducts
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            const string root = "Products";

            var productsDto = XmlConverter
                .Deserializer<ProductsInputModel>(inputXml, root);

            var products = productsDto
                .Select(p => new Product
            {
                Name = p.Name,
                Price = p.Price,
                SellerId = p.SellerId,
                BuyerId = p.BuyerId
            })
            .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        //03.ImportCategories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string root = "Categories";

            var categoriesDto = XmlConverter
                .Deserializer<CategoriesInputModel>(inputXml, root);

            var categories = categoriesDto
                .Where(c => c.Name != null)
                .Select(c => new Category
            {
                Name = c.Name
            })
            .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        //04.ImportCategoriesAndProducts
        //Using AutoMapper for this task
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();

            const string root = "CategoryProducts";

            var categoriesProductsDto = XmlConverter
                .Deserializer<CategoriesProductsInputModel>(inputXml, root);

            var categoriesProducts = mapper.Map<IEnumerable<CategoryProduct>>(categoriesProductsDto);

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        //05.ExportProductsInRange
        public static string GetProductsInRange(ProductShopContext context)
        {
            const string root = "Products";

            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExportProductsDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .Take(10)
                .ToList();

            var result = XmlConverter.Serialize(products, root);
            return result;
        }

        //06.ExportSoldProducts
        public static string GetSoldProducts(ProductShopContext context)
        {
            const string root = "Users";

            var users = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .Select(u => new ExportUsersSoldProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(x => new SoldProduct
                        {
                            Name = x.Name,
                            Price = x.Price
                        })
                        .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();

            var result = XmlConverter.Serialize(users, root);
            return result;
        }

        //07.ExportCategoriesByProductsCount
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            const string root = "Categories";

            var categories = context
                .Categories
                .Select(c => new ExportCategoriesByProductCountDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(x => x.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(x => x.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            var result = XmlConverter.Serialize(categories, root);
            return result;
        }

        //08.ExportUsersAndProducts
        //working solution form q2kPetrov
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            const string root = "Users";

            var users = context
                .Users
                .ToList()
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count())
                .Select(u => new ExportUserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsCountDto
                    {
                        Count = u.ProductsSold.Count(),
                        Products = u.ProductsSold
                                    .Select(x => new ProductToDTO
                                    {
                                        Name = x.Name,
                                        Price = x.Price
                                    })
                                    .OrderByDescending(y => y.Price)
                                    .ToList()
                    }

                })
                .Take(10)
                .ToList();

            var userProductWithCount = new UserProductWithCount
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = users

            };

            var result = XmlConverter.Serialize(userProductWithCount, root);
            return result;
        }
    }
}