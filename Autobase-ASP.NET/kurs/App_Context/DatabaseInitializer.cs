﻿using Autobase.DAO;
using Autobase.Models;
using Autobase.Models.enums;
using Autobase.Utils;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Autobase.App_Context
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<ApplicationContext>
    {
        private const int ROW_COUNT = 100;
        private CarDAO carDAO;
        private AccountDAO accountDAO;
        private OrderDAO orderDAO;
        private TripDAO tripDAO;

        public CarDAO CarDAO
        {
            get
            {
                if (carDAO == null)
                {
                    carDAO = DependencyResolver.Current.GetService<CarDAO>();
                }
                return carDAO;
            }
        }
        public AccountDAO AccountDAO
        {
            get
            {
                if (accountDAO == null)
                {
                    accountDAO = DependencyResolver.Current.GetService<AccountDAO>();
                }
                return accountDAO;
            }
        }
        public OrderDAO OrderDAO
        {
            get
            {
                if (orderDAO == null)
                {
                    orderDAO = DependencyResolver.Current.GetService<OrderDAO>();
                }
                return orderDAO;
            }
        }
        public TripDAO TripDAO
        {
            get
            {
                if (tripDAO == null)
                {
                    tripDAO = DependencyResolver.Current.GetService<TripDAO>();
                }
                return tripDAO;
            }
        }

        protected override void Seed(ApplicationContext context)
        {
            FillTables();
        }

        protected void FillTables()
        {
            CreateCars();
            CreateAccounts();
            CreateOrders();
            CreateTrips();
        }

        private void CreateCars()
        {
            for (int i = 0; i < ROW_COUNT; i++)
            {
                Car car = new Car();
                car.CarName = RandomUtil.GetInstance.GetRandomString;
                car.CarSpeed = 100 * RandomUtil.GetInstance.GetRandomDouble;
                car.CarCapacity = 100 * RandomUtil.GetInstance.GetRandomDouble;
                car.IsHealthy = RandomUtil.GetInstance.GetRandomBool;
                CarDAO.Create(car);
            }
        }

        private void CreateAccounts()
        {
            for (int i = 0; i < ROW_COUNT; i++)
            {
                Account acc = new Account();
                acc.AccountName = RandomUtil.GetInstance.GetRandomString;
                acc.Role = RandomUtil.GetInstance.GetRandomBool ? Role.DISPATCHER : Role.DRIVER;
                acc.Password = RandomUtil.GetInstance.GetRandomString;
                if (Role.DRIVER.Equals(acc.Role))
                {
                    acc.CarId = RandomUtil.GetInstance.GetRandomId;
                }
                AccountDAO.Create(acc);
            }
            CreateAdmin();
            CreateDriver();
        }

        private void CreateOrders()
        {
            Order order = null;
            for (int i = 0; i < ROW_COUNT; i++)
            {
                order = new Order();
                order.OrderName = RandomUtil.GetInstance.GetRandomString;
                order.RequiredCarCapacity = RandomUtil.GetInstance.GetRandomDouble;
                order.RequiredCarSpeed = RandomUtil.GetInstance.GetRandomDouble;
                order.Status = RandomUtil.GetInstance.GetRandomStatus;
                OrderDAO.Create(order);
            }
        }

        private void CreateTrips()
        {
            Trip trip = null;
            for (int i = 0; i < ROW_COUNT; i++)
            {
                Order order = OrderDAO.GetOrderById(RandomUtil.GetInstance.GetRandomId);
                if (TripStatusEnum.IN_PROCESS.Equals(order.Status))
                {
                    trip = new Trip();
                    trip.OrderId = order.OrderId;
                    Account acc;
                    int j = 0;
                    do
                    {
                        acc = AccountDAO.GetAccountById(RandomUtil.GetInstance.GetRandomId);
                        j++;
                    }
                    while (acc.Role.Equals(Role.DISPATCHER)
                            || acc.Car?.CarSpeed <= order.RequiredCarSpeed
                            && acc.Car?.CarCapacity <= order.RequiredCarCapacity);

                    trip.AccountId = acc.AccountId;

                    trip.CarId = (int)acc.CarId;

                    trip.TripName = order.OrderName;
                    trip.TripDate = RandomUtil.GetInstance.GetRandomDate;
                    TripDAO.Create(trip);
                }
            }
        }

        private void CreateAdmin()
        {
            Account admin = new Account();
            admin.AccountName = "admin";
            admin.Password = "nimda";
            admin.Car = null;
            admin.Role = Role.DISPATCHER;
            AccountDAO.Create(admin);
        }

        private void CreateDriver()
        {
            Account admin = new Account();
            admin.AccountName = "driver";
            admin.Password = "driver";
            admin.CarId = RandomUtil.GetInstance.GetRandomId;
            admin.Role = Role.DRIVER;
            AccountDAO.Create(admin);
        }
    }
}