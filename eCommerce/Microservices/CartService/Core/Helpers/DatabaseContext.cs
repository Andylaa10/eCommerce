﻿using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using CartService.Core.Entities;
using ConfigurationManager = System.Configuration.ConfigurationManager;


namespace CartService.Core.Helpers;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigurationManager.AppSettings.Get("CART_SERVICE_CONNECTION_STRING")!;
        
        //optionsBuilder.UseMongoDB(connectionString, "CartDB");
        optionsBuilder.UseMongoDB("mongodb://host.docker.internal:27018", "CartDB");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cart>().ToCollection("Carts");
    }

    public DbSet<Cart> Carts { get; init; }
}