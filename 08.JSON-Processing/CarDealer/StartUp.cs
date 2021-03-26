using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            var context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //09.ImportSuppliers
            //string inputJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //var result = ImportSuppliers(context, inputJson);
            //Console.WriteLine(result);

            //10.ImportParts
            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //ImportSuppliers(context, suppliersJson);
            //var result = ImportParts(context, partsJson);
            //Console.WriteLine(result);

            //11.ImportCars
            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //ImportSuppliers(context, suppliersJson);
            //ImportParts(context, partsJson);
            //var result = ImportCars(context, carsJson);
            //Console.WriteLine(result);

            //12.ImportCustomers
            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //ImportSuppliers(context, suppliersJson);
            //ImportParts(context, partsJson);
            //ImportCars(context, carsJson);
            //var result = ImportCustomers(context, customersJson);
            //Console.WriteLine(result);

            //13.ImportSales
            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //string salesJson = File.ReadAllText("../../../Datasets/sales.json");
            //string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //ImportSuppliers(context, suppliersJson);
            //ImportParts(context, partsJson);
            //ImportCars(context, carsJson);
            //ImportCustomers(context, customersJson);
            //var result = ImportSales(context, salesJson);
            //Console.WriteLine(result);

            //14.ExportOrderedCustomers
            //Console.WriteLine(GetOrderedCustomers(context));

            //15.ExportCarsFromMakeToyota
            //Console.WriteLine(GetCarsFromMakeToyota(context));

            //16.ExportLocalSuppliers
            //Console.WriteLine(GetLocalSuppliers(context));

            //17.ExportCarsWithTheirListOfParts
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //18.ExportTotalSalesByCustomer
            //Console.WriteLine(GetTotalSalesByCustomer(context));

            //19.ExportSalesWithAppliedDiscount
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }

        //09.ImportSuppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoSuppliers = JsonConvert
                .DeserializeObject<IEnumerable<ImportSupplierInputModel>>(inputJson);

            var suppliers = mapper.Map<IEnumerable<Supplier>>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        //10.ImportParts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var supplierIds = context
                .Suppliers
                .Select(x => x.Id)
                .ToList();

            //var dtoParts = JsonConvert
            //    .DeserializeObject<IEnumerable<ImportSupplierInputModel>>(inputJson);

            //var parts = mapper.Map<IEnumerable<Part>>(dtoParts);

            var dtoParts = JsonConvert
                .DeserializeObject<IEnumerable<ImportPartsInputModel>>(inputJson)
                .Where(s => supplierIds.Contains(s.SupplierId));

            var parts = mapper.Map<IEnumerable<Part>>(dtoParts);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        //11.ImportCars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var json = JsonConvert.DeserializeObject<IEnumerable<ImportCarsInputModel>>(inputJson);

            foreach (var carDto in json)
            {
                Car car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                context.Cars.Add(car);

                foreach (var partId in carDto.PartsId)
                {
                    PartCar partCar = new PartCar
                    {
                        CarId = car.Id,
                        PartId = partId
                    };

                    if (car.PartCars.FirstOrDefault(p => p.PartId == partId) == null)
                    {
                        context.PartCars.Add(partCar);
                    }
                }
            }

            context.SaveChanges();

            return $"Successfully imported {json.Count()}.";
        }

        //12.ImportCustomers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoCustomers = JsonConvert
                .DeserializeObject<IEnumerable<ImportCustomersInputModel>>(inputJson);

            var customers = mapper.Map<IEnumerable<Customer>>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";

            //var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            //context.Customers.AddRange(customers);
            //context.SaveChanges();

            //return $"Successfully imported {customers.Length}.";
        }

        //13.ImportSales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            var dtoSales = JsonConvert
                .DeserializeObject<IEnumerable<ImportSalesInputModel>>(inputJson);

            var sales = mapper.Map<IEnumerable<Sale>>(dtoSales);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";

            //var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            //context.Sales.AddRange(sales);
            //context.SaveChanges();

            //return $"Successfully imported {sales.Length}.";
        }

        //14.ExportOrderedCustomers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context
                .Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }

        //15.ExportCarsFromMakeToyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToList();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        //16.ExportLocalSuppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context
                .Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToList();

            var result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return result;
        }

        //17.ExportCarsWithTheirListOfParts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context
                .Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    parts = c.PartCars.Select(x => new
                    {
                        Name = x.Part.Name,
                        Price = x.Part.Price.ToString("F2")
                    })
                    .ToList()
                })
                .ToList();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        //18.ExportTotalSalesByCustomer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
                .Customers
                .Where(c => c.Sales.Count() >= 1)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(x => x.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }

        //19.ExportSalesWithAppliedDiscount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context
                .Sales
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = s.Discount.ToString("F2"),
                    price = s.Car.PartCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    priceWithDiscount = (s.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100)).ToString("F2")

                })
                .Take(10)
                .ToList();

            var result = JsonConvert.SerializeObject(sales, Formatting.Indented);

            return result;
        }
    }
}