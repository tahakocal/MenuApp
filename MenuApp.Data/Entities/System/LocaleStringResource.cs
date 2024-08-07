using MenuApp.Data.Entities.Base;

namespace MenuApp.Data.Entities.System;

public class LocaleStringResource : IEntity
{
    public string ResourceName { get; set; }
    public string ResourceValue { get; set; }
    public int LanguageId { get; set; }
    public Language Language { get; set; }
}