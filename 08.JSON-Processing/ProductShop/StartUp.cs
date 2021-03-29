using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObjects;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var productShopContext = new ProductShopContext();
            //productShopContext.Database.EnsureDeleted();
            //productShopContext.Database.EnsureCreated();

            //01.ImportUsers
            //string inputJson = File.ReadAllText("../../../Datasets/users.json");
            //var result = ImportUsers(productShopContext, inputJson);
            //Console.WriteLine(result);

            //02.ImportProducts
            //string usersJson = File.ReadAllText("../../../Datasets/users.json");
            //string productsJson = File.ReadAllText("../../../Datasets/products.json");
            //ImportUsers(productShopContext, usersJson);
            //var result = ImportProducts(productShopContext, productsJson);
            //Console.WriteLine(result);

            //03.ImportCategories
            //string usersJson = File.ReadAllText("../../../Datasets/users.json");
            //string productsJson = File.ReadAllText("../../../Datasets/products.json");
            //string categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //ImportUsers(productShopContext, usersJson);
            //ImportProducts(productShopContext, productsJson);
            //var result = ImportCategories(productShopContext, categoriesJson);
            //Console.WriteLine(result);

            //04.Import Categories and Products
            //string usersJson = File.ReadAllText("../../../Datasets/users.json");
            //string productsJson = File.ReadAllText("../../../Datasets/products.json");
            //string categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //string categoriesProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //ImportUsers(productShopContext, usersJson);
            //ImportProducts(productShopContext, productsJson);
            //ImportCategories(productShopContext, categoriesJson);
            //var result = ImportCategoryProducts(productShopContext, categoriesProductsJson);
            //Console.WriteLine(result);

            //05.Export Products In Range
            //var result = GetProductsInRange(productShopContext);
            //Console.WriteLine(result);

            //06.ExportSoldProducts
            //var result = GetSoldProducts(productShopContext);
            //Console.WriteLine(result);

            //07.ExportCategoriesByProductsCount
            //var result = GetCategoriesByProductsCount(productShopContext);
            //Console.WriteLine(result);

            //08.ExportUsersAndProducts
            //var result = GetUsersWithProducts(productShopContext);
            //Console.WriteLine(result);
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
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        //02.ImportProducts
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);

            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        //03.ImportCategories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoCategories = JsonConvert
                .DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson)
                .Where(x => x.Name != null)
                .ToList(); 

            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        //04.ImportCategoriesAndProducts
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoCategoriesProducts = JsonConvert
                .DeserializeObject<IEnumerable<CategoryProductInputModel>>(inputJson);

            var categoriesProducts = mapper.Map<IEnumerable<CategoryProduct>>(dtoCategoriesProducts);

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts .Count()}";
        }

        //05.ExportProductsInRange
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .OrderBy(p => p.price)
                .ToList();

            var result = JsonConvert.SerializeObject(products, Formatting.Indented);

            return result;
        }

        //06.ExportSoldProducts
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold.Where(c => c.Buyer != null)
                    .Select(c => new
                    {
                        name = c.Name,
                        price = c.Price,
                        buyerFirstName = c.Buyer.FirstName,
                        buyerLastName = c.Buyer.LastName
                    })
                })
                .OrderBy(z => z.lastName)
                .ThenBy(z => z.firstName)
                .ToList();

            var result = JsonConvert.SerializeObject(users, Formatting.Indented);

            return result;
        }

        //07.ExportCategoriesByProductsCount
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts
                                        .Select(cp => cp.Product)
                                        .Count(),
                    averagePrice = c.CategoryProducts
                                        .Average(cp => cp.Product.Price)
                                        .ToString("F2"),
                    totalRevenue = ((c.CategoryProducts
                                        .Select(cp => cp.Product)
                                        .Count())
                                        * (c.CategoryProducts
                                        .Average(cp => cp.Product.Price)))
                                        .ToString("F2")
                })
                .OrderByDescending(c => c.productsCount)
                .ToList();

            var result = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return result;
        }

        //08.ExportUsersAndProducts
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var allUsers = context
                .Users
                .Include(u => u.ProductsSold)
                .ToList()
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Where(x => x.BuyerId != null).Count(),
                        products = u.ProductsSold.Where(x => x.BuyerId != null)
                        .Select(n => new
                        {
                            name = n.Name,
                            price = n.Price
                        })
                    }
                })
                .OrderByDescending(u => u.soldProducts.products.Count())
                .ToList(); 

            var finalUsers = new
            {
                usersCount = allUsers.Count(),
                users = allUsers
            };

            var jSonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var result = JsonConvert
                .SerializeObject(finalUsers,Formatting.Indented, jSonSettings);

            return result;
        }
    }
}