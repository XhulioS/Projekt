using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModel
{
    public class ContactModel
    {

        public int ContactID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        [NotMapped]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [NotMapped]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email1 { get; set; }
        [NotMapped]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Not less than 10 digits")]
        public int Phone { get; set; }
        [NotMapped]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Not less than 10 digits")]
        public int Phone1 { get; set; }

    }
}