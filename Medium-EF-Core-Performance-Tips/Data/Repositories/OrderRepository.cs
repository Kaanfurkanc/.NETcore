﻿using Core.Entities;
using Core.Entity;
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
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _dataContext;
        private readonly DbSet<Order> _orders;
        public OrderRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _orders = _dataContext.Orders;
        }
        public async Task CreateAsync(Order order)
        {
           await _orders.AddAsync(order);
        }

        public void Delete(Order order)
        {
            _orders.Remove(order);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            var orders = await _dataContext.Orders
                    .Include(order => order.Products)
                    .AsSplitQuery()
                    .ToListAsync();

            return orders;
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _orders.FindAsync(id);
        }

        public void Update(Order order)
        {
            _orders.Update(order);
        }

        public async Task<IEnumerable<Order>> UseEagerLoadingAsync()
        {
            var orders = await _dataContext.Orders
                                .Include(order => order.Products)
                                .OrderBy(o =>  o.Code)
                                .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<Product>> UseLazyLoadingAsync(int id)
        {
            var order = await _dataContext.Orders.SingleOrDefaultAsync(_ => _.Id == id);

            var orderProducts = order.Products; 
            return orderProducts;
        }

        public IQueryable<Order> Where(Expression<Func<Order, bool>> expression)
        {
            return _orders.Where(expression).AsNoTracking();
        }
    }
}
