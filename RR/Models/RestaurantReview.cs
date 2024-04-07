using System.ComponentModel.DataAnnotations;

namespace RR.Models
{
    public class RestaurantReview
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name of the restaurant is required.")]
        [RegularExpression(@"^[a-zA-Z0-9 ]{3,60}$", ErrorMessage = "Name of the restaurant must be a 3 to 60 characters.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Name of the restaurant should be between 3 and 60 characters.")]
        [Display(Name = "Restaurant Name")]
        public required string RestaurantName { get; set; }

        [Required(ErrorMessage = "Name of food is required.")]
        [RegularExpression(@"^[a-zA-Z0-9 ]{3,60}$", ErrorMessage = "Name of food must be a 3 to 60 characters.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "Name of food should be between 3 and 60 characters.")]
        [Display(Name = "Food Name")]
        public required string FoodName { get; set; }

        [Required(ErrorMessage = "Price of food is required.")]
        [RegularExpression(@"^\d{1,7}(\.\d{2})?$", ErrorMessage = "Please enter a 7-digit number with 2 decimal places.")]
        [Range(0, 9999999.99, ErrorMessage = "Price should be a positive 7-digit number with up to 2 decimal places.")]
        [Display(Name = "Price")]
        public required decimal Price { get; set; }

        [Required(ErrorMessage = "Review Score is required.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Review Score must be a positive number with up to two decimal places.")]
        [Range(0.00, 5.00, ErrorMessage = "Review Score must be a 0 and 5.")]
        [Display(Name = "Review Score")]
        public required decimal ReviewScore { get; set; }

        [Required(ErrorMessage = "Date of publishing the review is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        [Display(Name = "Publishing Date")]
        public required DateTime PublishingDate { get; set; }

        [Required(ErrorMessage = "Image of the food is required.")]
        [Display(Name = "Food Image")]
        public required byte[] FoodImage { get; set; }
    }
}
