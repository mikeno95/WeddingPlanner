using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions; // Password regex

namespace WeddingPlanner.Models
{
    public class WeddingViewModel
    {
        [Required(ErrorMessage = "Wedder One's name is required")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        [DataType(DataType.Text)]
        [RegularExpression(@"(?!.*\s)^[a-zA-Z]+$", ErrorMessage = "Must be letters and cannot contain spaces")]
        public string WedderOne { get; set; }
        [Required(ErrorMessage = "Wedder Two's name is required")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        [DataType(DataType.Text)]
        [RegularExpression(@"(?!.*\s)^[a-zA-Z]+$", ErrorMessage = "Must be letters and cannot contain spaces")]
        public string WedderTwo { get; set; }
        [Required(ErrorMessage = "Wedding date is required")]
        [DataType(DataType.DateTime)]
        [ValidateDate]

        public DateTime Date { get;set; }
        [Required(ErrorMessage = "Street Address is required")]
        public string StreetAddress { get;set; }
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Zipcode is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter valid zipcode")]
        public string Zipcode { get; set; }
    }
    public class ValidateDate:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime Today = DateTime.Now; 
            if(value is DateTime)
            {
                DateTime InputDate = (DateTime)value;
                if (InputDate > Today)
                {
                    return ValidationResult.Success;
                } else {
                    return new ValidationResult("Cannot have your wedding in the past");
                }
            }
            
            return new ValidationResult("Please enter valid date");
        }
    }
}