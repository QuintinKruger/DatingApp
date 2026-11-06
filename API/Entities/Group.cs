using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class Group (string name)
{
    [Key] // group name is also primary key
    public string Name { get; set; } = name;

    // navigation properties
    public ICollection<Connection> Connections { get; set; } = [];

}
