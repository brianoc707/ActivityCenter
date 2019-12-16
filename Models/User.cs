    using System.ComponentModel.DataAnnotations;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    namespace ActivityCenter.Models
    
    {
        public class User
        {
            public int UserId { get; set; }
            [Required]
            [MinLength(2)]
            public string Name { get; set; }
            [EmailAddress]
            [Required]
            public string Email { get; set; }

            public List<Actividad> Planned {get;set;}

            public List <GuestList> Attending {get;set;}
            
            [Required]
            [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
            [DataType(DataType.Password)]
            [RegularExpression("(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
            public string Password { get; set; }
           
            public DateTime CreatedAt {get;set;} = DateTime.Now;
            public DateTime UpdatedAt {get;set;} = DateTime.Now;
            
            [NotMapped]
            [Compare("Password")]
            [DataType(DataType.Password)]
            public string ConfirmPW {get;set;}
        
        
        

        
        }

     }

