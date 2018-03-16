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

#if true
 using System;
using UnityEngine;
using System.Collections.Generic;
using KSP.IO;
using System.Linq;


using KSP.UI.Screens;
using ToolbarControl_NS;

namespace RCSBuildAid
{
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

        public AppLauncher(GameObject gameObject)
        {
            Log.Info("AppLauncher instantiation");
            if (instance == null) {
                instance = this;
#if false
                if (!Settings.toolbar_plugin_loaded) {
                    Settings.applauncher = true;
                }

                if (Settings.applauncher) {
                    addButton ();
                }
#endif
            }

#if true
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(onTrue, onFalse,
                visibleScenes,
                "RCSBuildAid",
                "RCSBuildAidButton",
                iconPathActive,
                iconPath,
                toolbarIconPathActive,
                toolbarIconPath,
                "RCS Build Aid"
            );
            toolbarControl.UseBlizzy(Settings.toolbar_plugin);

            if (RCSBuildAid.Enabled)
            {
                toolbarControl.SetTrue(false);
            }
            Events.PluginEnabled += onPluginEnable;
            Events.PluginDisabled += onPluginDisable;
#endif
        }
#if false
        public void addButton () {
            if (ApplicationLauncher.Ready) {
                _addButton ();
            }
            GameEvents.onGUIApplicationLauncherReady.Add(_addButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add (_removeButton);
        }

        public void removeButton () {
            if (ApplicationLauncher.Ready) {
                _removeButton ();
            }
            GameEvents.onGUIApplicationLauncherReady.Remove(_addButton);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(_removeButton);
        }

        void _addButton(){
            if (toolbarControl != null) {
                return;
            }
            

            button = ApplicationLauncher.Instance.AddModApplication (onTrue, onFalse, null, null,
                null, null, visibleScenes, GameDatabase.Instance.GetTexture(iconPath, false));
            if (RCSBuildAid.Enabled) {
                button.SetTrue (false);
            }
            Events.PluginEnabled += onPluginEnable;
            Events.PluginDisabled += onPluginDisable;
        }

        void _removeButton () {
            if (button != null) {
                ApplicationLauncher.Instance.RemoveModApplication (button);
                button = null;
                Events.PluginEnabled -= onPluginEnable;
                Events.PluginDisabled -= onPluginDisable;
            }
        }

        void _removeButton(GameScenes scene)
        {
            _removeButton ();
        }
#endif
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
#if false
                button.SetTrue (false);
#endif
            }
        }

        void onPluginDisable(bool byUser) {
            if (byUser) {
                toolbarControl.SetFalse(false);
#if false
                button.SetFalse (false);
#endif
            }
        }
    }
}

#endif