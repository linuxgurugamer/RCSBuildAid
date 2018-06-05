/* Copyright © 2013-2016, Elián Hanisch <lambdae2@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


 using System;
using UnityEngine;
using System.Collections.Generic;
using KSP.IO;
using System.Linq;


using KSP.UI.Screens;
using ToolbarControl_NS;

namespace RCSBuildAid
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(AppLauncher.MODID, AppLauncher.MODNAME);
        }
    }


    public class AppLauncher
    {
        public static AppLauncher instance;

        //static ApplicationLauncherButton button;
        
        const ApplicationLauncher.AppScenes visibleScenes = 
            ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB;

        internal static ToolbarControl toolbarControl;

        const string iconPath = "RCSBuildAid/Textures/iconAppLauncher";
        const string iconPathActive = "RCSBuildAid/Textures/iconAppLauncher_active";
        const string toolbarIconPath = "RCSBuildAid/Textures/iconToolbar";
        const string toolbarIconPathActive = "RCSBuildAid/Textures/iconToolbar_active";

        internal const string MODID = "RCSBuildAid_NS";
        internal const string MODNAME = "RCS Build Aid";

        public AppLauncher(GameObject gameObject)
        {
            Log.Info("AppLauncher instantiation");
            if (instance == null) {
                instance = this;

            }

            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(onTrue, onFalse,
                visibleScenes,
                MODID,
                "RCSBuildAidButton",
                iconPathActive,
                iconPath,
                toolbarIconPathActive,
                toolbarIconPath,
                MODNAME
            );

            if (RCSBuildAid.Enabled)
            {
                toolbarControl.SetTrue(false);
            }
            Events.PluginEnabled += onPluginEnable;
            Events.PluginDisabled += onPluginDisable;

        }

        void onTrue ()
        {
            RCSBuildAid.SetActive (true);
        }

        void onFalse ()
        {
            RCSBuildAid.SetActive (false);
        }

        void onPluginEnable(bool byUser) {
            if (byUser) {
                toolbarControl.SetTrue(false);
            }
        }

        void onPluginDisable(bool byUser) {
            if (byUser) {
                toolbarControl.SetFalse(false);
            }
        }
    }
}

