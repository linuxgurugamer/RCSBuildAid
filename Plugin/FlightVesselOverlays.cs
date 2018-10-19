using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RCSBuildAid
{

    public class FlightMarker : MonoBehaviour
    {
        public GameObject posMarkerObject;

        public GameObject dirMarkerObject;

        private void Update()
        {
            if (!(bool)posMarkerObject)
            {
                posMarkerObject.transform.position = UpdatePosition();
            }
            if (!(bool)dirMarkerObject)
            {
                return;
            }

            dirMarkerObject.transform.position = UpdatePosition();

        }

        protected virtual Vector3 UpdatePosition()
        {
            return Vector3.zero;
        }

        protected virtual Vector3 UpdateDirection()
        {
            return Vector3.zero;
        }
    }

    public class FlightMarker_CoT : FlightMarker
    {
        private static Ray CoT;

        private static float t;

        private static CenterOfThrustQuery tQry = new CenterOfThrustQuery();

        public static Vector3 Pos => CoT.origin;

        public static Vector3 Dir => CoT.direction;

        private void Update()
        {
            CoT = FindCoT();
            if ((bool)base.posMarkerObject)
            {
                base.posMarkerObject.transform.position = CoT.origin;
            }
            if (!(bool)base.dirMarkerObject)
            {
                return;
            }

            base.dirMarkerObject.transform.forward = CoT.direction;

        }

        public static Ray FindCoT()
        {
            t = 0f;
            Vector3 vector = Vector3.zero;
            Vector3 vector2 = Vector3.zero;
            recurseParts(FlightGlobals.ActiveVessel.rootPart, ref vector, ref vector2, ref t);

            if (t != 0f)
            {
                float d = 1f / t;
                vector *= d;
                vector2 *= d;
                return new Ray(vector, vector2);

            }
            return new Ray(Vector3.zero, Vector3.zero);
        }

        private static void recurseParts(Part part, ref Vector3 origin, ref Vector3 direction, ref float t)
        {
            int num = part.Modules.Count;
            while (num-- > 0)
            {
                IThrustProvider thrustProvider = part.Modules[num] as IThrustProvider;
                if (thrustProvider != null)
                {
                    tQry.Reset();
                    thrustProvider.OnCenterOfThrustQuery(tQry);
                    origin += tQry.pos * tQry.thrust;
                    direction += tQry.dir * tQry.thrust;
                    t += tQry.thrust;
                }
            }

            for (int i = 0; i < part.children.Count; i++)
            {
                Part part2 = part.children[i];
                recurseParts(part2, ref origin, ref direction, ref t);
            }

        }
    }

    public class FlightMarker_CoL : FlightMarker
    {
        public Vector3 referenceVelocity = Vector3.up;

        public float referencePitch = 1f;

        public float referenceSpeed = 70f;

        public double refAlt;

        public double refStP;

        public double refDens;

        private Ray CoL;

        private double refTemp;

        private static CenterOfLiftQuery lQry = new CenterOfLiftQuery();

        private void Update()
        {

            // referenceVelocity = EditorLogic.VesselRotation * (Quaternion.AngleAxis(referencePitch, EditorLogic.RootPart.transform.right) * Vector3.up);
            referenceVelocity = FlightGlobals.ActiveVessel.velocityD; ;
            referenceVelocity *= referenceSpeed;
            refAlt = 100.0;
            refStP = FlightGlobals.getStaticPressure(refAlt, Planetarium.fetch.Home);
            refTemp = FlightGlobals.getExternalTemperature(refAlt, Planetarium.fetch.Home);
            refDens = FlightGlobals.getAtmDensity(refStP, refTemp, Planetarium.fetch.Home);
            CoL = FindCoL(referenceVelocity, refAlt, refStP, refDens);
            if ((bool)base.posMarkerObject)
            {
                base.posMarkerObject.transform.position = CoL.origin;
            }
            if (CoL.direction == Vector3.zero)
            {
                if (base.dirMarkerObject.activeInHierarchy)
                {
                    base.dirMarkerObject.SetActive(false);
                }
            }
            if (CoL.direction != Vector3.zero)
            {
                if (!base.dirMarkerObject.activeInHierarchy)
                {
                    base.dirMarkerObject.SetActive(true);
                }
            }
            if (!(bool)base.dirMarkerObject)
            {
                return;
            }

            if ((CoL.direction == Vector3.zero))
            {
                return;
            }

            base.dirMarkerObject.transform.forward = CoL.direction;
        }


        public static Ray FindCoL(Vector3 refVel, double refAlt, double refStp, double refDens)
        {
            Vector3 vector = Vector3.zero;
            Vector3 vector2 = Vector3.zero;
            float num = 0f;

            if (num != 0f)
            {

                float d = 1f / num;
                vector *= d;
                vector2 *= d;
                return new Ray(vector, vector2);

            }
            return new Ray(Vector3.zero, Vector3.zero);
        }

        private static void recurseParts(Part part, Vector3 refVel, ref Vector3 CoL, ref Vector3 DoL, ref float t, double refAlt, double refStp, double refDens)
        {
            int num = part.Modules.Count;
            while (num-- > 0)
            {
                ILiftProvider liftProvider = part.Modules[num] as ILiftProvider;
                if (liftProvider != null)
                {
                    lQry.Reset();
                    lQry.refVector = refVel;
                    lQry.refAltitude = refAlt;
                    lQry.refStaticPressure = refStp;
                    lQry.refAirDensity = refDens;
                    liftProvider.OnCenterOfLiftQuery(lQry);
                    CoL += lQry.pos * lQry.lift;
                    DoL += lQry.dir * lQry.lift;
                    t += lQry.lift;
                }
            }

            for (int i = 0; i < part.children.Count; i++)
            {
                Part part2 = part.children[i];
                recurseParts(part2, refVel, ref CoL, ref DoL, ref t, refAlt, refStp, refDens);
            }


        }
    }
    public class FlightMarker_CoM : FlightMarker  
    {
        public static Vector3 CraftCoM;

        protected override Vector3 UpdatePosition()
        {
            CraftCoM = findCenterOfMass(FlightGlobals.ActiveVessel.rootPart);
            return CraftCoM;
        }

        public static Vector3 findCenterOfMass(Part root)
        {
            Vector3 vector = Vector3.zero;
            float d = 0f;
            recurseParts(root, ref vector, ref d);

            vector /= d;
            return vector;
        }

        private static void recurseParts(Part part, ref Vector3 CoM, ref float m)
        {
            if (part.physicalSignificance == Part.PhysicalSignificance.FULL)
            {
                CoM += (part.transform.position + part.transform.rotation * part.CoMOffset) * (part.mass + part.GetResourceMass());
                m += part.mass + part.GetResourceMass();
            }
            else if ((UnityEngine.Object)part.parent != (UnityEngine.Object)null)
            {
                CoM += (part.parent.transform.position + part.parent.transform.rotation * part.parent.CoMOffset) * (part.mass + part.GetResourceMass());
                m += part.mass + part.GetResourceMass();
            }
            else if ((UnityEngine.Object)part.potentialParent != (UnityEngine.Object)null)
            {
                CoM += (part.potentialParent.transform.position + part.potentialParent.transform.rotation * part.potentialParent.CoMOffset) * (part.mass + part.GetResourceMass());
                m += part.mass + part.GetResourceMass();
            }
            for (int i = 0; i < part.children.Count; i++)
            {
                Part part2 = part.children[i];
                recurseParts(part2, ref CoM, ref m);
            }
        }
    }


    public class FlightVesselOverlays : MonoBehaviour
    {
        public static FlightVesselOverlays Instance;

#if false
        public Button toggleCoMbtn;

        public Button toggleCoTbtn;

        public Button toggleCoLbtn;
#endif
        public FlightMarker_CoM CoMmarker;

        public FlightMarker_CoT CoTmarker;

        public FlightMarker_CoL CoLmarker;

        public float referenceAirSpeed;

        public float referencePitch;

        internal void localAwake()
        {
            if ((bool)Instance)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        internal void localStart()
        {
#if false
            toggleCoMbtn.onClick.AddListener(ToggleCoM);
            toggleCoTbtn.onClick.AddListener(ToggleCoT);
            toggleCoLbtn.onClick.AddListener(ToggleCoL);
#endif

            //referencePitch = 1f;
            referencePitch = 0f;

            if (!(bool)CoMmarker)
            {

                CoMmarker = Instantiate(CoMmarker);
                CoMmarker.gameObject.SetActive(true);
            }
            if (!(bool)CoTmarker)
            {
                CoTmarker = Instantiate(CoTmarker);
                CoTmarker.gameObject.SetActive(true);
            }
            if (!(bool)CoLmarker)
            {
                CoLmarker = Instantiate(CoLmarker);
                CoLmarker.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }


        private void Update()
        {
            if (!(bool)CoLmarker)
            {
                return;
            }

            CoLmarker.referencePitch = referencePitch;
            CoLmarker.referenceSpeed = referenceAirSpeed;
        }

        private void DisableMarkers()
        {
            if ((bool)CoMmarker)
            {
                CoMmarker.gameObject.SetActive(false);
            }
            if ((bool)CoTmarker)
            {
                CoTmarker.gameObject.SetActive(false);
            }
            if ((bool)CoLmarker)
                CoLmarker.gameObject.SetActive(false);
        }

        public void ToggleCoM()
        {
            if ((bool)CoMmarker)
                CoMmarker.gameObject.SetActive(!CoMmarker.gameObject.activeInHierarchy);

        }

        public void ToggleCoT()
        {
            if ((bool)CoTmarker)
                CoTmarker.gameObject.SetActive(!CoTmarker.gameObject.activeInHierarchy);

        }

        public void ToggleCoL()
        {
            if ((bool)CoLmarker)
                CoLmarker.gameObject.SetActive(!CoLmarker.gameObject.activeInHierarchy);
        }
    }
}