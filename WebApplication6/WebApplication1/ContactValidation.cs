using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class ContactValidation
    {
         [Display(Name="First Name")]
       // [Required(ErrorMessage="Please provide First Name", AllowEmptyStrings=false)]
        public string Firstname { get; set; }

        [Display(Name="Last Name")]
        public string Lastname { get; set; }

        [Display(Name="Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        //[RegularExpression(@"^\d{10}$",ErrorMessage="Not less than 10 digits")]
        public int Phone { get; set; }

        [Display(Name="Address")]
        //[Required(ErrorMessage="Please provide Address", AllowEmptyStrings=false)]
        public string Address { get; set; }
    }
    
    [MetadataType(typeof(ContactValidation))]
    public partial class Contact
    {

    }
    }