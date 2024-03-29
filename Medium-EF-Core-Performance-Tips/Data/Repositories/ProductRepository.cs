﻿using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _dataContext;

        //Compiled Query
        private static  Func<DataContext, int, Task<Product?>> GetByIdCompiledQuery = 
                 EF.CompileAsyncQuery(
                    (DataContext context, int id) => context.Products.AsNoTracking().FirstOrDefault(_ => _.Id == id));


        public async Task<Product?> GetProductByIdWCompiledQuery(int id)
        {
            return await GetByIdCompiledQuery.Invoke(_dataContext, id);
        }

        public IEnumerable<Product> GetExpensiveProductsWithCompiledQuery()
        {
            var compiledQuery = EF.CompileQuery((DataContext context) => context.Products.AsNoTracking().Where(p => p.Price > 1000));

            using (DataContext context = new DataContext())
            {
                var expensiveProducts = compiledQuery.Invoke(context);
                return expensiveProducts;
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _dataContext.Products.SingleOrDefaultAsync(_ => _.Id == id);
            // Not : If there are many datas by id , SingleOrDefault method throws exception .
        }

        public ProductRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task CreateAsync(Product product)
        {
            await _dataContext.Products.AddAsync(product);
        }

        public  void Delete(Product product)
        {
            _dataContext.Products.Remove(product);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize)
        {
            var products = await _dataContext.Products.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();
            return products;
            // Pagination
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            //return await _dataContext.Products.Where(_ => _.Stock > 1000).AsNoTracking().ToListAsync();  AsNoTracking Method 
            return await _dataContext.Products.ToListAsync();
        }

        public async void Update(Product product)
        {
            _dataContext.Products.Update(product);
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            return _dataContext.Products.Where(expression).AsNoTracking();
        }

        public async Task<IEnumerable<Product?>> GetPopularProductsAsync()
        {
            return await _dataContext.Products.FromSql($"EXECUTE dbo.GetMost").ToListAsync();
        }
    }
}

// WhenAll(); Method completes many tasks at same time .
//var task1 = _products.Where(p => p.Name == "Pencil").ToListAsync();
//var task2 = _products.Where(p => p.Name == "Book").ToListAsync();
//var task3 = _products.Where(p => p.Price > 1000 && p.Stock < 10).ToListAsync();

//await Task.WhenAll(task1, task2, task3);

//var products = new List<Product>();
//products.AddRange(task1.Result);
//products.AddRange(task2.Result);
//products.AddRange(task3.Result);
//return products;