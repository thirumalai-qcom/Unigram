//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------
// <auto-generatedInfo>
// 	This code was generated by ResW File Code Generator (http://bit.ly/reswcodegen)
// 	ResW File Code Generator was written by Christian Resma Helle
// 	and is under GNU General Public License version 2 (GPLv2)
// 
// 	This code contains a helper class exposing property representations
// 	of the string resources defined in the specified .ResW file
// 
// 	Generated: 11/11/2017 22:23:48
// </auto-generatedInfo>
// --------------------------------------------------------------------------------------------------
namespace Unigram.Strings
{
    using Windows.ApplicationModel.Resources;
    
    
    public sealed partial class Settings
    {
        
        private static ResourceLoader resourceLoader;
        
        static Settings()
        {
            string executingAssemblyName;
            executingAssemblyName = Windows.UI.Xaml.Application.Current.GetType().AssemblyQualifiedName;
            string[] executingAssemblySplit;
            executingAssemblySplit = executingAssemblyName.Split(',');
            executingAssemblyName = executingAssemblySplit[1];
            string currentAssemblyName;
            currentAssemblyName = typeof(Settings).AssemblyQualifiedName;
            string[] currentAssemblySplit;
            currentAssemblySplit = currentAssemblyName.Split(',');
            currentAssemblyName = currentAssemblySplit[1];
            if (executingAssemblyName.Equals(currentAssemblyName))
            {
                resourceLoader = ResourceLoader.GetForViewIndependentUse("Settings");
            }
            else
            {
                resourceLoader = ResourceLoader.GetForViewIndependentUse(currentAssemblyName + "/Settings");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Audio messages"
        /// </summary>
        public static string AutoDownload_Audio
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_Audio");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Files"
        /// </summary>
        public static string AutoDownload_Document
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_Document");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "GIFs"
        /// </summary>
        public static string AutoDownload_GIF
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_GIF");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Music"
        /// </summary>
        public static string AutoDownload_Music
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_Music");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "No media"
        /// </summary>
        public static string AutoDownload_None
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_None");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Photos"
        /// </summary>
        public static string AutoDownload_Photo
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_Photo");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Video messages"
        /// </summary>
        public static string AutoDownload_Round
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_Round");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Videos"
        /// </summary>
        public static string AutoDownload_Video
        {
            get
            {
                return resourceLoader.GetString("AutoDownload_Video");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Edit permissions"
        /// </summary>
        public static string ParticipantEdit
        {
            get
            {
                return resourceLoader.GetString("ParticipantEdit");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Promote"
        /// </summary>
        public static string ParticipantPromote
        {
            get
            {
                return resourceLoader.GetString("ParticipantPromote");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Restrict"
        /// </summary>
        public static string ParticipantRestrict
        {
            get
            {
                return resourceLoader.GetString("ParticipantRestrict");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Proxy Settings"
        /// </summary>
        public static string ProxySettingsShareTitle
        {
            get
            {
                return resourceLoader.GetString("ProxySettingsShareTitle");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "Forever"
        /// </summary>
        public static string UserRestrictionsUntilForever
        {
            get
            {
                return resourceLoader.GetString("UserRestrictionsUntilForever");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "When using mobile data"
        /// </summary>
        public static string WhenOnMobileData
        {
            get
            {
                return resourceLoader.GetString("WhenOnMobileData");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "When connected to Wi-Fi"
        /// </summary>
        public static string WhenOnWiFi
        {
            get
            {
                return resourceLoader.GetString("WhenOnWiFi");
            }
        }
        
        /// <summary>
        /// Localized resource similar to "When roaming"
        /// </summary>
        public static string WhenRoaming
        {
            get
            {
                return resourceLoader.GetString("WhenRoaming");
            }
        }
    }
}
