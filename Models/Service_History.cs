using FixItFinderDemo.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace FixItFinderDemo.Models;

public class Service_History
{
    [Key]
    public int Id { get; set; }
    public int WorkerId { get; set; }
    [ForeignKey("WorkerId")]
    public Worker_Profile? Worker { get; set; }
    public int CustomerId { get; set; }
    [ForeignKey("CustomerId")]
    public Customer_Profile? Customer { get; set; }
    public string? Review { get; set; }
}
