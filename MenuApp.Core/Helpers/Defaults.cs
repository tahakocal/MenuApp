namespace MenuApp.Core.Helpers;

public static class Defaults
{
    public static string LocaleStringResourcesPrefixCacheKey = "LocaleStringResourcesPrefixCacheKey.{0}";
    public static string LanguagesAllCacheKey => "MenuApp.languages.all";
    public static string DefaultLanguageCacheKey => "MenuApp.language.default";
    public static string LocaleStringResourcesAllCacheKey => "MenuApp.lsr.all-{0}";
    public static string AdminLocaleStringResourcesPrefix => "Admin.";
    public static string PluginNameLocaleStringResourcesPrefix => "Plugins.FriendlyName.";
    public static string LocalizedPropertyCacheKey => "MenuApp.localizedproperty.value-{0}-{1}-{2}-{3}";
    public static string LocalizedPropertyAllCacheKey => "MenuApp.localizedproperty.all";
    public static string LocalizedPropertyPrefixCacheKey => "MenuApp.localizedproperty.";
    public static string EnumLocaleStringResourcesPrefix => "Enums.";
    public static string SearchEngineCustomerName => "SearchEngine";
    public static string Prefix => ".MenuApp";
    public static string CustomerCookie => ".User";
    public static string XWafForHeader => "X-WAF-FOR";
    public static string IsPostBeingDoneRequestItem => "MenuApp.IsPOSTBeingDone";
    public static int CacheTime => 6000;
}