using System.ComponentModel.DataAnnotations;

namespace Concept.Models;

public sealed class CategoryForm
{
    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public int? AccountId { get; set; }
    public bool Active { get; set; } = true;
}
