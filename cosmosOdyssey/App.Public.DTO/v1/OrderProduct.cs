﻿namespace App.Public.DTO.v1;

public class OrderProduct
{
    public Guid Id { get; set; }
    
    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Total { get; set; }

    public int Discount { get; set; }

    public Guid ProductId { get; set; }
    // public Product? Product { get; set; }
    
    public Guid? OrderId { get; set; }
    // public Order? Order { get; set; }
}