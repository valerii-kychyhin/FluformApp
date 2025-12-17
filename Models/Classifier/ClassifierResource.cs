namespace FluformApp.Models.Classifier;

public class ClassifierResource
{
    public string Id { get; set; } = string.Empty;
    public List<string?> Name { get; set; } = new List<string?>();
    public string Slug { get; set; } = string.Empty;
    public object Content { get; set; } = new object(); // Подставь более точный тип, если знаешь структуру
}