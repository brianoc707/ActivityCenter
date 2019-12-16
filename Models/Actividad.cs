using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace ActivityCenter.Models

{
    public class Actividad
    {
    public int ActividadId {get;set;}
    [Required]
    public string Name {get; set;}
    [Required]
    public DateTime Date {get; set;}

    public int UserId {get;set;}
    [Required]
    public string Description {get;set;}
    public User Creator {get;set;}
    [Required]
    public string DurationString {get;set;}
    [Required]
    public int DurationInt {get;set;}
    
 
    public List<GuestList> Guests {get;set;}
    
    


    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;
            
    }
}