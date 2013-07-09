using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using UTable.Objects;
using UTable.Input;
using UTable.Config;

namespace TangramTheatre
{
    /// <summary>
    /// This class encapsulates the environment for the whole platform
    /// </summary>
    public partial class App : UApplication
    {
        /// <summary>
        /// Get the configuration for the whole platform.
        /// This function will be called when the application is starting up, and <see cref="UTable.Objects.UTableHelper.Config"/> will be set, 
        /// so that you can get the config from this global variable later.
        /// </summary>
        /// <returns>The confiugration instance for the whole platform</returns>
        protected override UTableConfig GetConfig()
        {
            // Load the configuration from config file "UTable.config" in the project
            // You can change the configuration by editing the config file, 
            // or you can directly generate a UTableConfig instance, set its properties and return it. 
            return ConfigGenerator.GetConfigFromXml("UTable.config");
        }

        /// <summary>
        /// This function will be called when the whole platform has just started
        /// </summary>
        protected override void OnTabletopLoaded()
        {
            base.OnTabletopLoaded();

            // Add the main object to the tabletop, so that it can be displayed
            // Comment the following three lines you do not want to load the object when platform started up
            ObjectCreateParameter param = new ObjectCreateParameter(typeof(UObject1));
            IObject obj = UTableHelper.CreateObject(param);
            obj.Owner = this.Tabletop;

            // You can add more initialization code here when platform started up

        }
    }
}
