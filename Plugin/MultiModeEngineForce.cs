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
using System.Collections.Generic;
using UnityEngine;

namespace RCSBuildAid
{
    /* Component for calculate and show forces in engines such as RAPIER */
    public class MultiModeEngineForce : EngineForce
    {
        MultiModeEngine module;
        Dictionary<string, ModuleEngines> modes = new Dictionary<string, ModuleEngines> ();

        ModuleEngines activeMode {
            get {
                // While most mod parts have the engineId being the same as the engine mode display name, at least one doesn't
                // The following logic checks for that 
                if (modes.ContainsKey(module.mode))
                    return modes[module.mode];
                if (module.mode == module.primaryEngineModeDisplayName)
                    return modes[module.primaryEngineID];
                if (module.mode == module.secondaryEngineModeDisplayName)
                    return modes[module.secondaryEngineID];
                Log.Info("No modes found, returning null");
                return null;
            }
        }

        protected override ModuleEngines Engine {
            get { return activeMode; }
        }

        protected override bool connectedToVessel {
            get { return RCSBuildAid.Engines.Contains (module); }
        }

        protected override void Init ()
        {
            Log.Info("MultiModeEngineForce");
            module = GetComponent<MultiModeEngine> ();
            if (module == null) {
                throw new Exception ("Missing MultiModeEngine component.");
            }
            var engines = module.GetComponents<ModuleEngines> ();
            foreach (var eng in engines) {
                Log.Info("mode[" + eng.engineID + "] = " + eng);
                modes [eng.engineID] = eng;
            }
            GimbalRotation.addTo (gameObject);
        }
    }
}
