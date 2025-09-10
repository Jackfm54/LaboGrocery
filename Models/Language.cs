using System.ComponentModel.DataAnnotations;

namespace LaboGrocery.Models
{
    public class Language
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Culture { get; set; } = "fr-FR";

        [Required, StringLength(100)]
        public string DisplayName { get; set; } = "Français (France)";
    }
}
