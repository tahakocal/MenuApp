using System.ComponentModel.DataAnnotations.Schema;
using MenuApp.Data.Entities.Base;

namespace MenuApp.Data.Entities.System;

public class Language : IEntity
{
    public string Name { get; set; }
    public string LanguageCulture { get; set; }
    public string UniqueSeoCode { get; set; }
    public string FlagImageFileName { get; set; }
    public string NormalizedName { get; set; }
    public bool Published { get; set; }

    public bool IsDefault { get; set; }

    // public ICollection<LocalizedProperty> LocalizedProperties { get; set; }
    public ICollection<LocaleStringResource> LocaleStringResource { get; set; }
}

[NotMapped]
public class LanguageForCaching : Language
{
    public LanguageForCaching()
    {
    }

    /// <summary>
    ///     Ctor
    /// </summary>
    /// <param name="l">Language to copy</param>
    public LanguageForCaching(Language l)
    {
        Id = l.Id;
        Name = l.Name;
        LanguageCulture = l.LanguageCulture;
        UniqueSeoCode = l.UniqueSeoCode;
        FlagImageFileName = l.FlagImageFileName;
        NormalizedName = l.NormalizedName;
        Published = l.Published;
        IsDefault = l.IsDefault;
    }
}